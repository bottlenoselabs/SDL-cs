// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Collections.Immutable;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Common;

namespace SDL_GPU;

public abstract unsafe partial class ExampleGpu : ExampleBase
{
    public SDL_GPUDevice* Device { get; private set; }

    protected ExampleGpu(WindowOptions? windowOptions = null)
        : base(windowOptions)
    {
        Name = RegexExampleTypeName().Replace(GetType().Name, match =>
        {
            var number = match.Groups[1].Value.TrimStart('0');

            // Insert spaces between camel case
            var words = RegexWords().Replace(match.Groups[2].Value, "$1 $2");
            return $"{number} - {words}";
        });

        AssetsDirectory = Path.Combine(AppContext.BaseDirectory, "Assets");
    }

    public override bool Initialize(INativeAllocator allocator)
    {
        Device = SDL_CreateGPUDevice(
            SDL_GPU_SHADERFORMAT_SPIRV | SDL_GPU_SHADERFORMAT_DXIL | SDL_GPU_SHADERFORMAT_MSL,
            false,
            null);

        if (Device == null)
        {
            Console.Error.WriteLine("GPUCreateDevice failed");
            return false;
        }

        if (!SDL_ClaimWindowForGPUDevice(Device, Window))
        {
            Console.Error.WriteLine("GPUClaimWindow failed");
            return false;
        }

        return true;
    }

    public override void Quit()
    {
        SDL_ReleaseWindowFromGPUDevice(Device, Window);
        SDL_DestroyGPUDevice(Device);
    }

    protected SDL_GPUShader* CreateShader(
        INativeAllocator allocator,
        string fileName,
        uint samplerCount = 0,
        uint uniformBufferCount = 0,
        uint storageBufferCount = 0,
        uint storageTextureCount = 0)
    {
        // Auto-detect the shader stage from the file name for convenience
        SDL_GPUShaderStage stage;

        if (fileName.EndsWith(".vert", StringComparison.CurrentCultureIgnoreCase))
        {
            stage = SDL_GPUShaderStage.SDL_GPU_SHADERSTAGE_VERTEX;
        }
        else if (fileName.EndsWith(".frag", StringComparison.CurrentCultureIgnoreCase))
        {
            stage = SDL_GPUShaderStage.SDL_GPU_SHADERSTAGE_FRAGMENT;
        }
        else
        {
            Console.Error.WriteLine("Invalid shader stage!");
            return null;
        }

        var backendFormats = SDL_GetGPUShaderFormats(Device);
        SDL_GPUShaderFormat format;
        string entryPoint;
        string filePath;

        if ((backendFormats & SDL_GPU_SHADERFORMAT_SPIRV) != 0)
        {
            filePath = Path.Combine(AssetsDirectory, "Shaders/Compiled/SPIRV", fileName + ".spv");
            format = SDL_GPU_SHADERFORMAT_SPIRV;
            entryPoint = "main";
        }
        else if ((backendFormats & SDL_GPU_SHADERFORMAT_MSL) != 0)
        {
            filePath = Path.Combine(AssetsDirectory, "Shaders/Compiled/MSL", fileName + ".msl");
            format = SDL_GPU_SHADERFORMAT_MSL;
            entryPoint = "main0";
        }
        else if ((backendFormats & SDL_GPU_SHADERFORMAT_DXIL) != 0)
        {
            filePath = Path.Combine(AssetsDirectory, "Shaders/Compiled/DXIL", fileName + ".dxil");
            format = SDL_GPU_SHADERFORMAT_DXIL;
            entryPoint = "main";
        }
        else
        {
            Console.Error.WriteLine("Unrecognized backend shader format!");
            return null;
        }

        ulong codeSize;
        var filePathC = allocator.AllocateCString(filePath);
        var code = SDL_LoadFile(filePathC, &codeSize);
        if (code == null)
        {
            Console.Error.WriteLine($"Failed to load shader '{filePath}' from disk!");
            return null;
        }

        SDL_GPUShaderCreateInfo shaderInfo = default;

        shaderInfo.code = (byte*)code;
        shaderInfo.code_size = codeSize;
        shaderInfo.entrypoint = allocator.AllocateCString(entryPoint);
        shaderInfo.format = format;
        shaderInfo.stage = stage;
        shaderInfo.num_samplers = samplerCount;
        shaderInfo.num_uniform_buffers = uniformBufferCount;
        shaderInfo.num_storage_buffers = storageBufferCount;
        shaderInfo.num_storage_textures = storageTextureCount;

        var shader = SDL_CreateGPUShader(Device, &shaderInfo);
        if (shader == null)
        {
            Console.Error.WriteLine("Failed to create shader!");
            SDL_free(code);
            return null;
        }

        SDL_free(code);
        return shader;
    }

    protected SDL_GPUBuffer* CreateVertexBuffer<TVertex>(int elementCount)
        where TVertex : unmanaged
    {
        var vertexBufferCreateInfo = default(SDL_GPUBufferCreateInfo);
        vertexBufferCreateInfo.usage = SDL_GPU_BUFFERUSAGE_VERTEX;
        vertexBufferCreateInfo.size = (uint)(sizeof(TVertex) * elementCount);
        var vertexBuffer = SDL_CreateGPUBuffer(Device, &vertexBufferCreateInfo);
        return vertexBuffer;
    }

    protected SDL_GPUTransferBuffer* CreateTransferBuffer<TElement>(int elementCount, Action<Span<TElement>> map)
        where TElement : unmanaged
    {
        var transferBufferCreateInfo = default(SDL_GPUTransferBufferCreateInfo);
        transferBufferCreateInfo.usage = SDL_GPUTransferBufferUsage.SDL_GPU_TRANSFERBUFFERUSAGE_UPLOAD;
        transferBufferCreateInfo.size = (uint)(sizeof(TElement) * elementCount);
        var transferBuffer = SDL_CreateGPUTransferBuffer(Device, &transferBufferCreateInfo);

        var dataPointer = (TElement*)SDL_MapGPUTransferBuffer(Device, transferBuffer, false);
        var data = new Span<TElement>(dataPointer, elementCount);
        map(data);
        SDL_UnmapGPUTransferBuffer(Device, transferBuffer);

        return transferBuffer;
    }

    protected void FillGraphicsPipelineSwapchainColorTarget(
        INativeAllocator allocator,
        ref SDL_GPUGraphicsPipelineTargetInfo targetInfo)
    {
        targetInfo.num_color_targets = 1;
        targetInfo.color_target_descriptions = allocator
            .AllocateArray<SDL_GPUColorTargetDescription>(1);

        ref var colorTargetDescription = ref targetInfo.color_target_descriptions[0];
        colorTargetDescription.format = SDL_GetGPUSwapchainTextureFormat(Device, Window);
    }

    protected void FillGraphicsPipelineVertexAttributes<TVertex>(
        INativeAllocator allocator,
        ref SDL_GPUVertexInputState vertexInputState)
        where TVertex : unmanaged
    {
        var vertexType = typeof(TVertex);
        var vertexFields = vertexType.GetFields().ToImmutableArray();

        vertexInputState.num_vertex_attributes = (uint)vertexFields.Length;
        vertexInputState.vertex_attributes = allocator.AllocateArray<SDL_GPUVertexAttribute>(vertexFields.Length);

        for (var i = 0; i < vertexFields.Length; i++)
        {
            var vertexField = vertexFields[i];
            ref var vertexAttribute = ref vertexInputState.vertex_attributes[i];
            vertexAttribute.location = (uint)i;

            if (vertexField.FieldType == typeof(Vector3))
            {
                vertexAttribute.format = SDL_GPUVertexElementFormat.SDL_GPU_VERTEXELEMENTFORMAT_FLOAT3;
            }
            else if (vertexField.FieldType == typeof(Rgba8U))
            {
                vertexAttribute.format = SDL_GPUVertexElementFormat.SDL_GPU_VERTEXELEMENTFORMAT_UBYTE4_NORM;
            }
            else
            {
                throw new NotImplementedException();
            }

            vertexAttribute.offset = (uint)Marshal.OffsetOf(vertexType, vertexField.Name);
        }
    }

    protected void FillGraphicsPipelineVertexBuffer<TVertex>(
        INativeAllocator allocator,
        ref SDL_GPUVertexInputState vertexInputState)
        where TVertex : unmanaged
    {
        vertexInputState.num_vertex_buffers = 1;
        vertexInputState.vertex_buffer_descriptions = allocator
            .AllocateArray<SDL_GPUVertexBufferDescription>(1);

        ref var vertexBufferDescription = ref vertexInputState.vertex_buffer_descriptions[0];
        vertexBufferDescription.slot = 0;
        vertexBufferDescription.input_rate = SDL_GPUVertexInputRate.SDL_GPU_VERTEXINPUTRATE_VERTEX;
        vertexBufferDescription.instance_step_rate = 0;
        vertexBufferDescription.pitch = (uint)sizeof(TVertex);
    }

    protected SDL_GPUTextureFormat GetSupportedDepthStencilFormat()
    {
        if (SDL_GPUTextureSupportsFormat(
                Device,
                SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_D24_UNORM_S8_UINT,
                SDL_GPUTextureType.SDL_GPU_TEXTURETYPE_2D,
                SDL_GPU_TEXTUREUSAGE_DEPTH_STENCIL_TARGET))
        {
            return SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_D24_UNORM_S8_UINT;
        }

        if (SDL_GPUTextureSupportsFormat(
                Device,
                SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_D32_FLOAT_S8_UINT,
                SDL_GPUTextureType.SDL_GPU_TEXTURETYPE_2D,
                SDL_GPU_TEXTUREUSAGE_DEPTH_STENCIL_TARGET))
        {
            return SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_D32_FLOAT_S8_UINT;
        }

        return SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_INVALID;
    }

    [GeneratedRegex(@"E(\d+)_(\w+)")]
    private static partial Regex RegexExampleTypeName();

    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex RegexWords();
}
