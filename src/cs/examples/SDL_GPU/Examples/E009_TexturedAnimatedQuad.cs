// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Numerics;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace SDL_GPU.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed unsafe class E008_TexturedAnimatedQuad : ExampleGpu
{
    private SDL_GPUGraphicsPipeline* _pipeline;
    private SDL_GPUBuffer* _vertexBuffer;
    private SDL_GPUBuffer* _indexBuffer;
    private SDL_GPUTexture* _texture;
    private SDL_GPUSampler* _sampler;

    private float _t;

    public override bool Initialize(INativeAllocator allocator)
    {
        if (!base.Initialize(allocator))
        {
            return false;
        }

        var vertexShader = CreateShader(
            allocator, "TexturedQuadWithMatrix.vert", uniformBufferCount: 1);
        if (vertexShader == null)
        {
            Console.Error.WriteLine("Failed to create vertex shader!");
            return false;
        }

        var fragmentShader = CreateShader(
            allocator, "TexturedQuadWithMultiplyColor.frag", samplerCount: 1, uniformBufferCount: 1);
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
        ref var blendState = ref pipelineCreateInfo.target_info.color_target_descriptions[0].blend_state;
        blendState.enable_blend = true;
        blendState.alpha_blend_op = SDL_GPUBlendOp.SDL_GPU_BLENDOP_ADD;
        blendState.color_blend_op = SDL_GPUBlendOp.SDL_GPU_BLENDOP_ADD;
        blendState.src_color_blendfactor = SDL_GPUBlendFactor.SDL_GPU_BLENDFACTOR_SRC_ALPHA;
        blendState.src_alpha_blendfactor = SDL_GPUBlendFactor.SDL_GPU_BLENDFACTOR_SRC_ALPHA;
        blendState.dst_color_blendfactor = SDL_GPUBlendFactor.SDL_GPU_BLENDFACTOR_ONE_MINUS_SRC_ALPHA;
        blendState.dst_alpha_blendfactor = SDL_GPUBlendFactor.SDL_GPU_BLENDFACTOR_ONE_MINUS_SRC_ALPHA;

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

        // PointClamp
        var samplerCreateInfo = default(SDL_GPUSamplerCreateInfo);
        samplerCreateInfo.min_filter = SDL_GPUFilter.SDL_GPU_FILTER_NEAREST;
        samplerCreateInfo.mag_filter = SDL_GPUFilter.SDL_GPU_FILTER_NEAREST;
        samplerCreateInfo.mipmap_mode = SDL_GPUSamplerMipmapMode.SDL_GPU_SAMPLERMIPMAPMODE_NEAREST;
        samplerCreateInfo.address_mode_u = SDL_GPUSamplerAddressMode.SDL_GPU_SAMPLERADDRESSMODE_CLAMP_TO_EDGE;
        samplerCreateInfo.address_mode_v = SDL_GPUSamplerAddressMode.SDL_GPU_SAMPLERADDRESSMODE_CLAMP_TO_EDGE;
        samplerCreateInfo.address_mode_w = SDL_GPUSamplerAddressMode.SDL_GPU_SAMPLERADDRESSMODE_CLAMP_TO_EDGE;
        _sampler = SDL_CreateGPUSampler(Device, &samplerCreateInfo);

        _vertexBuffer = CreateVertexBuffer<VertexPositionTexture>(4);
        if (_vertexBuffer == null)
        {
            Console.Error.WriteLine("Failed to create vertex buffer!");
            return false;
        }

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
        vertexData[0].Position = new Vector3(-0.5f, -0.5f, 0);
        vertexData[0].TextureCoordinates = new Vector2(0, 0);

        vertexData[1].Position = new Vector3(0.5f, -0.5f, 0);
        vertexData[1].TextureCoordinates = new Vector2(1, 0);

        vertexData[2].Position = new Vector3(0.5f, 0.5f, 0);
        vertexData[2].TextureCoordinates = new Vector2(1, 1);

        vertexData[3].Position = new Vector3(-0.5f, 0.5f, 0);
        vertexData[3].TextureCoordinates = new Vector2(0, 1);

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

        SDL_DestroySurface(imageData);
        SDL_EndGPUCopyPass(copyPass);
        SDL_SubmitGPUCommandBuffer(uploadCommandBuffer);
        SDL_ReleaseGPUTransferBuffer(Device, transferBufferVertexIndex);
        SDL_ReleaseGPUTransferBuffer(Device, transferBufferTexture);

        return true;
    }

    public override void Quit()
    {
        SDL_ReleaseGPUGraphicsPipeline(Device, _pipeline);
        SDL_ReleaseGPUBuffer(Device, _vertexBuffer);
        SDL_ReleaseGPUBuffer(Device, _indexBuffer);
        SDL_ReleaseGPUTexture(Device, _texture);
        SDL_ReleaseGPUSampler(Device, _sampler);

        _t = 0;

        base.Quit();
    }

    public override void KeyboardEvent(SDL_KeyboardEvent e)
    {
    }

    public override bool Update(float deltaTime)
    {
        _t += deltaTime;
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
        samplerBinding.sampler = _sampler;
        SDL_BindGPUFragmentSamplers(renderPass, 0, &samplerBinding, 1);

        // Bottom-left
        var uniformMatrix =
            Matrix4x4.CreateRotationZ(_t) *
            Matrix4x4.CreateTranslation(-0.5f, -0.5f, 0);
        SDL_PushGPUVertexUniformData(commandBuffer, 0, &uniformMatrix, (uint)sizeof(Matrix4x4));
        Rgba32F uniformColor;
        uniformColor.R = 1.0f;
        uniformColor.G = 0.5f + ((float)Math.Sin(_t) * 0.5f);
        uniformColor.B = 1.0f;
        uniformColor.A = 1.0f;
        SDL_PushGPUFragmentUniformData(commandBuffer, 0, &uniformColor, (uint)sizeof(Rgba32F));
        SDL_DrawGPUIndexedPrimitives(renderPass, 6, 1, 0, 0, 0);

        // Bottom-right
        uniformMatrix =
            Matrix4x4.CreateRotationZ(_t) *
            Matrix4x4.CreateTranslation(0.5f, -0.5f, 0);
        SDL_PushGPUVertexUniformData(commandBuffer, 0, &uniformMatrix, (uint)sizeof(Matrix4x4));
        uniformColor.R = 1.0f;
        uniformColor.G = 0.5f + ((float)Math.Cos(_t) * 0.5f);
        uniformColor.B = 1.0f;
        uniformColor.A = 1.0f;
        SDL_PushGPUFragmentUniformData(commandBuffer, 0, &uniformColor, (uint)sizeof(Rgba32F));
        SDL_DrawGPUIndexedPrimitives(renderPass, 6, 1, 0, 0, 0);

        // Top-left
        uniformMatrix =
            Matrix4x4.CreateRotationZ(_t) *
            Matrix4x4.CreateTranslation(-0.5f, 0.5f, 0);
        SDL_PushGPUVertexUniformData(commandBuffer, 0, &uniformMatrix, (uint)sizeof(Matrix4x4));
        uniformColor.R = 1.0f;
        uniformColor.G = 0.5f + ((float)Math.Cos(_t) * 0.2f);
        uniformColor.B = 1.0f;
        uniformColor.A = 1.0f;
        SDL_PushGPUFragmentUniformData(commandBuffer, 0, &uniformColor, (uint)sizeof(Rgba32F));
        SDL_DrawGPUIndexedPrimitives(renderPass, 6, 1, 0, 0, 0);

        // Top-right
        uniformMatrix =
            Matrix4x4.CreateRotationZ(_t) *
            Matrix4x4.CreateTranslation(0.5f, 0.5f, 0);
        SDL_PushGPUVertexUniformData(commandBuffer, 0, &uniformMatrix, (uint)sizeof(Matrix4x4));
        uniformColor.R = 1.0f;
        uniformColor.G = 0.5f + ((float)Math.Cos(_t) * 1.0f);
        uniformColor.B = 1.0f;
        uniformColor.A = 1.0f;
        SDL_PushGPUFragmentUniformData(commandBuffer, 0, &uniformColor, (uint)sizeof(Rgba32F));
        SDL_DrawGPUIndexedPrimitives(renderPass, 6, 1, 0, 0, 0);

        SDL_EndGPURenderPass(renderPass);
        SDL_SubmitGPUCommandBuffer(commandBuffer);
        return true;
    }
}
