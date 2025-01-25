// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using Common;

namespace SDL_GPU;

public abstract unsafe class ExampleGpu : ExampleBase
{
    public SDL_GPUDevice* Device { get; private set; }

    protected ExampleGpu(string name, WindowOptions? windowOptions = null)
        : base(name, windowOptions)
    {
        AssetsDirectory = Path.Combine(AppContext.BaseDirectory, "Assets");
    }

    public override bool Initialize()
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

    protected SDL_GPUShader* LoadShader(
        SDL_GPUDevice* device,
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

        var backendFormats = SDL_GetGPUShaderFormats(device);
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
        using var filePathC = (CString)filePath;
        var code = SDL_LoadFile(filePathC, &codeSize);
        if (code == null)
        {
            Console.Error.WriteLine($"Failed to load shader '{filePath}' from disk!");
            return null;
        }

        SDL_GPUShaderCreateInfo shaderInfo = default;

        shaderInfo.code = (byte*)code;
        shaderInfo.code_size = codeSize;
        shaderInfo.entrypoint = entryPoint;
        shaderInfo.format = format;
        shaderInfo.stage = stage;
        shaderInfo.num_samplers = samplerCount;
        shaderInfo.num_uniform_buffers = uniformBufferCount;
        shaderInfo.num_storage_buffers = storageBufferCount;
        shaderInfo.num_storage_textures = storageTextureCount;

        var shader = SDL_CreateGPUShader(device, &shaderInfo);
        if (shader == null)
        {
            Console.Error.WriteLine("Failed to create shader!");
            SDL_free(code);
            shaderInfo._entrypoint.Dispose();
            return null;
        }

        SDL_free(code);
        shaderInfo._entrypoint.Dispose();
        return shader;
    }
}
