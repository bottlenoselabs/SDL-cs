// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Numerics;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace SDL_GPU.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed unsafe class E008_TexturedQuad : ExampleGpu
{
    private static readonly string[] SamplerNames =
    {
        "PointClamp",
        "PointWrap",
        "LinearClamp",
        "LinearWrap",
        "AnisotropicClamp",
        "AnisotropicWrap",
    };

    private SDL_GPUGraphicsPipeline* _pipeline;
    private SDL_GPUBuffer* _vertexBuffer;
    private SDL_GPUBuffer* _indexBuffer;
    private SDL_GPUTexture* _texture;
    private readonly SDL_GPUSampler*[] _samplers = new SDL_GPUSampler*[SamplerNames.Length];

    private int _currentSamplerIndex;

    public override bool Initialize(INativeAllocator allocator)
    {
        if (!base.Initialize(allocator))
        {
            return false;
        }

        var vertexShader = CreateShader(allocator, "TexturedQuad.vert");
        if (vertexShader == null)
        {
            Console.Error.WriteLine("Failed to create vertex shader!");
            return false;
        }

        var fragmentShader = CreateShader(
            allocator, "TexturedQuad.frag", samplerCount: 1);
        if (fragmentShader == null)
        {
            Console.Error.WriteLine("Failed to create fragment shader!");
            return false;
        }

        var imageData = CreateImage(
            allocator, "ravioli.bmp", desiredChannels: 4);
        if (imageData == null)
        {
            Console.Error.WriteLine("Failed to load image data!");
            return false;
        }

        var pipelineCreateInfo = default(SDL_GPUGraphicsPipelineCreateInfo);
        pipelineCreateInfo.primitive_type = SDL_GPUPrimitiveType.SDL_GPU_PRIMITIVETYPE_TRIANGLELIST;
        pipelineCreateInfo.vertex_shader = vertexShader;
        pipelineCreateInfo.fragment_shader = fragmentShader;
        FillGraphicsPipelineSwapchainColorTarget(allocator, ref pipelineCreateInfo.target_info);
        FillGraphicsPipelineVertexAttributes<VertexPositionTexture>(allocator, ref pipelineCreateInfo.vertex_input_state);
        FillGraphicsPipelineVertexBuffer<VertexPositionTexture>(allocator, ref pipelineCreateInfo.vertex_input_state);

        _pipeline = SDL_CreateGPUGraphicsPipeline(Device, &pipelineCreateInfo);
        if (_pipeline == null)
        {
            Console.Error.WriteLine("Failed to create pipeline!");
            return false;
        }

        SDL_ReleaseGPUShader(Device, vertexShader);
        SDL_ReleaseGPUShader(Device, fragmentShader);

        var textureCreateInfo = default(SDL_GPUTextureCreateInfo);
        textureCreateInfo.type = SDL_GPUTextureType.SDL_GPU_TEXTURETYPE_2D;
        textureCreateInfo.format = SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_R8G8B8A8_UNORM;
        textureCreateInfo.width = (uint)imageData->w;
        textureCreateInfo.height = (uint)imageData->h;
        textureCreateInfo.layer_count_or_depth = 1;
        textureCreateInfo.num_levels = 1;
        textureCreateInfo.usage = SDL_GPU_TEXTUREUSAGE_SAMPLER;

        _texture = SDL_CreateGPUTexture(Device, &textureCreateInfo);
        if (_texture == null)
        {
            Console.Error.WriteLine("Failed to create texture!");
            return false;
        }

        SDL_SetGPUTextureName(Device, _texture, "Ravioli Texture 🖼️"u8);

        // PointClamp
        var samplerCreateInfo = default(SDL_GPUSamplerCreateInfo);
        samplerCreateInfo.min_filter = SDL_GPUFilter.SDL_GPU_FILTER_NEAREST;
        samplerCreateInfo.mag_filter = SDL_GPUFilter.SDL_GPU_FILTER_NEAREST;
        samplerCreateInfo.mipmap_mode = SDL_GPUSamplerMipmapMode.SDL_GPU_SAMPLERMIPMAPMODE_NEAREST;
        samplerCreateInfo.address_mode_u = SDL_GPUSamplerAddressMode.SDL_GPU_SAMPLERADDRESSMODE_CLAMP_TO_EDGE;
        samplerCreateInfo.address_mode_v = SDL_GPUSamplerAddressMode.SDL_GPU_SAMPLERADDRESSMODE_CLAMP_TO_EDGE;
        samplerCreateInfo.address_mode_w = SDL_GPUSamplerAddressMode.SDL_GPU_SAMPLERADDRESSMODE_CLAMP_TO_EDGE;
        _samplers[0] = SDL_CreateGPUSampler(Device, &samplerCreateInfo);

        // PointWrap
        samplerCreateInfo = default;
        samplerCreateInfo.min_filter = SDL_GPUFilter.SDL_GPU_FILTER_NEAREST;
        samplerCreateInfo.mag_filter = SDL_GPUFilter.SDL_GPU_FILTER_NEAREST;
        samplerCreateInfo.mipmap_mode = SDL_GPUSamplerMipmapMode.SDL_GPU_SAMPLERMIPMAPMODE_NEAREST;
        samplerCreateInfo.address_mode_u = SDL_GPUSamplerAddressMode.SDL_GPU_SAMPLERADDRESSMODE_REPEAT;
        samplerCreateInfo.address_mode_v = SDL_GPUSamplerAddressMode.SDL_GPU_SAMPLERADDRESSMODE_REPEAT;
        samplerCreateInfo.address_mode_w = SDL_GPUSamplerAddressMode.SDL_GPU_SAMPLERADDRESSMODE_REPEAT;
        _samplers[1] = SDL_CreateGPUSampler(Device, &samplerCreateInfo);

        // LinearClamp
        samplerCreateInfo = default;
        samplerCreateInfo.min_filter = SDL_GPUFilter.SDL_GPU_FILTER_LINEAR;
        samplerCreateInfo.mag_filter = SDL_GPUFilter.SDL_GPU_FILTER_LINEAR;
        samplerCreateInfo.mipmap_mode = SDL_GPUSamplerMipmapMode.SDL_GPU_SAMPLERMIPMAPMODE_LINEAR;
        samplerCreateInfo.address_mode_u = SDL_GPUSamplerAddressMode.SDL_GPU_SAMPLERADDRESSMODE_CLAMP_TO_EDGE;
        samplerCreateInfo.address_mode_v = SDL_GPUSamplerAddressMode.SDL_GPU_SAMPLERADDRESSMODE_CLAMP_TO_EDGE;
        samplerCreateInfo.address_mode_w = SDL_GPUSamplerAddressMode.SDL_GPU_SAMPLERADDRESSMODE_CLAMP_TO_EDGE;
        _samplers[2] = SDL_CreateGPUSampler(Device, &samplerCreateInfo);

        // LinearWrap
        samplerCreateInfo = default;
        samplerCreateInfo.min_filter = SDL_GPUFilter.SDL_GPU_FILTER_LINEAR;
        samplerCreateInfo.mag_filter = SDL_GPUFilter.SDL_GPU_FILTER_LINEAR;
        samplerCreateInfo.mipmap_mode = SDL_GPUSamplerMipmapMode.SDL_GPU_SAMPLERMIPMAPMODE_LINEAR;
        samplerCreateInfo.address_mode_u = SDL_GPUSamplerAddressMode.SDL_GPU_SAMPLERADDRESSMODE_REPEAT;
        samplerCreateInfo.address_mode_v = SDL_GPUSamplerAddressMode.SDL_GPU_SAMPLERADDRESSMODE_REPEAT;
        samplerCreateInfo.address_mode_w = SDL_GPUSamplerAddressMode.SDL_GPU_SAMPLERADDRESSMODE_REPEAT;
        _samplers[3] = SDL_CreateGPUSampler(Device, &samplerCreateInfo);

        // AnisotropicClamp
        samplerCreateInfo = default;
        samplerCreateInfo.min_filter = SDL_GPUFilter.SDL_GPU_FILTER_LINEAR;
        samplerCreateInfo.mag_filter = SDL_GPUFilter.SDL_GPU_FILTER_LINEAR;
        samplerCreateInfo.mipmap_mode = SDL_GPUSamplerMipmapMode.SDL_GPU_SAMPLERMIPMAPMODE_LINEAR;
        samplerCreateInfo.address_mode_u = SDL_GPUSamplerAddressMode.SDL_GPU_SAMPLERADDRESSMODE_CLAMP_TO_EDGE;
        samplerCreateInfo.address_mode_v = SDL_GPUSamplerAddressMode.SDL_GPU_SAMPLERADDRESSMODE_CLAMP_TO_EDGE;
        samplerCreateInfo.address_mode_w = SDL_GPUSamplerAddressMode.SDL_GPU_SAMPLERADDRESSMODE_CLAMP_TO_EDGE;
        samplerCreateInfo.enable_anisotropy = true;
        samplerCreateInfo.max_anisotropy = 4;
        _samplers[4] = SDL_CreateGPUSampler(Device, &samplerCreateInfo);

        // AnisotropicWrap
        samplerCreateInfo = default;
        samplerCreateInfo.min_filter = SDL_GPUFilter.SDL_GPU_FILTER_LINEAR;
        samplerCreateInfo.mag_filter = SDL_GPUFilter.SDL_GPU_FILTER_LINEAR;
        samplerCreateInfo.mipmap_mode = SDL_GPUSamplerMipmapMode.SDL_GPU_SAMPLERMIPMAPMODE_LINEAR;
        samplerCreateInfo.address_mode_u = SDL_GPUSamplerAddressMode.SDL_GPU_SAMPLERADDRESSMODE_REPEAT;
        samplerCreateInfo.address_mode_v = SDL_GPUSamplerAddressMode.SDL_GPU_SAMPLERADDRESSMODE_REPEAT;
        samplerCreateInfo.address_mode_w = SDL_GPUSamplerAddressMode.SDL_GPU_SAMPLERADDRESSMODE_REPEAT;
        samplerCreateInfo.enable_anisotropy = true;
        samplerCreateInfo.max_anisotropy = 4;
        _samplers[5] = SDL_CreateGPUSampler(Device, &samplerCreateInfo);

        _vertexBuffer = CreateVertexBuffer<VertexPositionTexture>(4);
        if (_vertexBuffer == null)
        {
            Console.Error.WriteLine("Failed to create vertex buffer!");
            return false;
        }

        SDL_SetGPUBufferName(Device, _vertexBuffer, "Ravioli Vertex Buffer 🥣"u8);

        _indexBuffer = CreateIndexBuffer<ushort>(6);
        if (_indexBuffer == null)
        {
            Console.Error.WriteLine("Failed to create index buffer!");
            return false;
        }

        var transferBufferCreateInfoVertexIndex = default(SDL_GPUTransferBufferCreateInfo);
        transferBufferCreateInfoVertexIndex.usage = SDL_GPUTransferBufferUsage.SDL_GPU_TRANSFERBUFFERUSAGE_UPLOAD;
        transferBufferCreateInfoVertexIndex.size = (uint)((sizeof(VertexPositionTexture) * 4) + (sizeof(ushort) * 6));
        var transferBufferVertexIndex = SDL_CreateGPUTransferBuffer(Device, &transferBufferCreateInfoVertexIndex);

        var dataVertexIndex = SDL_MapGPUTransferBuffer(Device, transferBufferVertexIndex, false);
        SDL_UnmapGPUTransferBuffer(Device, transferBufferVertexIndex);

        var vertexData = (VertexPositionTexture*)dataVertexIndex;
        vertexData[0].Position = new Vector3(-1f, 1f, 0); // top-left
        vertexData[0].TextureCoordinates = new Vector2(0, 0);

        vertexData[1].Position = new Vector3(1f, 1f, 0); // top-right
        vertexData[1].TextureCoordinates = new Vector2(4, 0);

        vertexData[2].Position = new Vector3(1, -1f, 0); // bottom-right
        vertexData[2].TextureCoordinates = new Vector2(4, 4);

        vertexData[3].Position = new Vector3(-1, -1, 0); // bottom-left
        vertexData[3].TextureCoordinates = new Vector2(0, 4);

        var indexData = (ushort*)&vertexData[4];
        indexData[0] = 0;
        indexData[1] = 1;
        indexData[2] = 2;
        indexData[3] = 0;
        indexData[4] = 2;
        indexData[5] = 3;

        SDL_UnmapGPUTransferBuffer(Device, transferBufferVertexIndex);

        // Set up texture data
        var transferBufferCreateInfoTexture = default(SDL_GPUTransferBufferCreateInfo);
        transferBufferCreateInfoTexture.usage = SDL_GPUTransferBufferUsage.SDL_GPU_TRANSFERBUFFERUSAGE_UPLOAD;
        transferBufferCreateInfoTexture.size = (uint)(imageData->w * imageData->h * 4);
        var transferBufferTexture = SDL_CreateGPUTransferBuffer(Device, &transferBufferCreateInfoTexture);

        var dataTexture = SDL_MapGPUTransferBuffer(Device, transferBufferTexture, false);
        NativeMemory.Copy(imageData->pixels, dataTexture, (UIntPtr)(imageData->w * imageData->h * 4));
        SDL_UnmapGPUTransferBuffer(Device, transferBufferTexture);

        var uploadCommandBuffer = SDL_AcquireGPUCommandBuffer(Device);
        var copyPass = SDL_BeginGPUCopyPass(uploadCommandBuffer);

        var bufferSourceVertex = default(SDL_GPUTransferBufferLocation);
        bufferSourceVertex.transfer_buffer = transferBufferVertexIndex;
        bufferSourceVertex.offset = 0;
        var bufferDestinationVertex = default(SDL_GPUBufferRegion);
        bufferDestinationVertex.buffer = _vertexBuffer;
        bufferDestinationVertex.offset = 0;
        bufferDestinationVertex.size = (uint)(sizeof(VertexPositionTexture) * 4);
        SDL_UploadToGPUBuffer(copyPass, &bufferSourceVertex, &bufferDestinationVertex, false);

        var bufferSourceIndex = default(SDL_GPUTransferBufferLocation);
        bufferSourceIndex.transfer_buffer = transferBufferVertexIndex;
        bufferSourceIndex.offset = (uint)(sizeof(VertexPositionTexture) * 4);
        var bufferDestinationIndex = default(SDL_GPUBufferRegion);
        bufferDestinationIndex.buffer = _indexBuffer;
        bufferDestinationIndex.offset = 0;
        bufferDestinationIndex.size = sizeof(ushort) * 6;
        SDL_UploadToGPUBuffer(copyPass, &bufferSourceIndex, &bufferDestinationIndex, false);

        var bufferSourceTexture = default(SDL_GPUTextureTransferInfo);
        bufferSourceTexture.transfer_buffer = transferBufferTexture;
        bufferSourceTexture.offset = 0; /* Zeros out the rest */
        var bufferDestinationTexture = default(SDL_GPUTextureRegion);
        bufferDestinationTexture.texture = _texture;
        bufferDestinationTexture.w = (uint)imageData->w;
        bufferDestinationTexture.h = (uint)imageData->h;
        bufferDestinationTexture.d = 1;
        SDL_UploadToGPUTexture(copyPass, &bufferSourceTexture, &bufferDestinationTexture, false);

        SDL_EndGPUCopyPass(copyPass);
        SDL_SubmitGPUCommandBuffer(uploadCommandBuffer);
        SDL_DestroySurface(imageData);
        SDL_ReleaseGPUTransferBuffer(Device, transferBufferVertexIndex);
        SDL_ReleaseGPUTransferBuffer(Device, transferBufferTexture);

        // Finally, print instructions!
        Console.WriteLine("Press LEFT/RIGHT to switch between sampler states");
        Console.WriteLine("Setting sampler state to: {0}", SamplerNames[_currentSamplerIndex]);

        return true;
    }

    public override void Quit()
    {
        SDL_ReleaseGPUGraphicsPipeline(Device, _pipeline);
        SDL_ReleaseGPUBuffer(Device, _vertexBuffer);
        SDL_ReleaseGPUBuffer(Device, _indexBuffer);
        SDL_ReleaseGPUTexture(Device, _texture);

        for (var i = 0; i < SamplerNames.Length; i += 1)
        {
            SDL_ReleaseGPUSampler(Device, _samplers[i]);
        }

        _currentSamplerIndex = 0;

        base.Quit();
    }

    public override void KeyboardEvent(SDL_KeyboardEvent e)
    {
        switch (e.scancode)
        {
            case SDL_Scancode.SDL_SCANCODE_LEFT:
                _currentSamplerIndex -= 1;
                if (_currentSamplerIndex < 0)
                {
                    _currentSamplerIndex = SamplerNames.Length - 1;
                }

                Console.WriteLine("Setting sampler state to: {0}", SamplerNames[_currentSamplerIndex]);
                break;
            case SDL_Scancode.SDL_SCANCODE_RIGHT:
                _currentSamplerIndex = (_currentSamplerIndex + 1) % SamplerNames.Length;
                Console.WriteLine("Setting sampler state to: {0}", SamplerNames[_currentSamplerIndex]);
                break;
        }
    }

    public override bool Update(float deltaTime)
    {
        return true;
    }

    public override bool Draw(float deltaTime)
    {
        var commandBuffer = SDL_AcquireGPUCommandBuffer(Device);
        if (commandBuffer == null)
        {
            Console.Error.WriteLine("AcquireGPUCommandBuffer failed: " + SDL_GetError());
            return false;
        }

        SDL_GPUTexture* textureSwapchain;
        if (!SDL_WaitAndAcquireGPUSwapchainTexture(
                commandBuffer,
                Window,
                &textureSwapchain,
                null,
                null))
        {
            Console.Error.WriteLine("WaitAndAcquireGPUSwapchainTexture failed: " + SDL_GetError());
            return false;
        }

        if (textureSwapchain == null)
        {
            SDL_SubmitGPUCommandBuffer(commandBuffer);
            return true;
        }

        var colorTargetInfo = default(SDL_GPUColorTargetInfo);
        colorTargetInfo.texture = textureSwapchain;
        colorTargetInfo.clear_color = Rgba32F.CornflowerBlue;
        colorTargetInfo.load_op = SDL_GPULoadOp.SDL_GPU_LOADOP_CLEAR;
        colorTargetInfo.store_op = SDL_GPUStoreOp.SDL_GPU_STOREOP_STORE;

        var renderPass = SDL_BeginGPURenderPass(
            commandBuffer, &colorTargetInfo, 1, null);

        SDL_BindGPUGraphicsPipeline(renderPass, _pipeline);
        var bufferBindingVertex = default(SDL_GPUBufferBinding);
        bufferBindingVertex.buffer = _vertexBuffer;
        bufferBindingVertex.offset = 0;
        SDL_BindGPUVertexBuffers(renderPass, 0, &bufferBindingVertex, 1);
        var bufferBindingIndex = default(SDL_GPUBufferBinding);
        bufferBindingIndex.buffer = _indexBuffer;
        bufferBindingIndex.offset = 0;
        SDL_BindGPUIndexBuffer(renderPass, &bufferBindingIndex, SDL_GPUIndexElementSize.SDL_GPU_INDEXELEMENTSIZE_16BIT);
        var samplerBinding = default(SDL_GPUTextureSamplerBinding);
        samplerBinding.texture = _texture;
        samplerBinding.sampler = _samplers[_currentSamplerIndex];
        SDL_BindGPUFragmentSamplers(renderPass, 0, &samplerBinding, 1);
        SDL_DrawGPUIndexedPrimitives(renderPass, 6, 1, 0, 0, 0);

        SDL_EndGPURenderPass(renderPass);
        SDL_SubmitGPUCommandBuffer(commandBuffer);
        return true;
    }
}
