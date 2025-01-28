// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Numerics;
using JetBrains.Annotations;

namespace SDL_GPU.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed unsafe class E007_InstancedIndex : ExampleGpu
{
    private SDL_GPUGraphicsPipeline* _pipeline;
    private SDL_GPUBuffer* _vertexBuffer;
    private SDL_GPUBuffer* _indexBuffer;

    private bool _isEnabledVertexOffset;
    private bool _isEnabledIndexOffset;
    private bool _isEnabledIndexBuffer = true;

    public override bool Initialize(INativeAllocator allocator)
    {
        if (!base.Initialize(allocator))
        {
            return false;
        }

        var vertexShader = CreateShader(allocator, "PositionColorInstanced.vert");
        if (vertexShader == null)
        {
            Console.Error.WriteLine("Failed to create vertex shader!");
            return false;
        }

        var fragmentShader = CreateShader(allocator, "SolidColor.frag");
        if (fragmentShader == null)
        {
            Console.Error.WriteLine("Failed to create fragment shader!");
            return false;
        }

        var pipelineCreateInfo = default(SDL_GPUGraphicsPipelineCreateInfo);
        pipelineCreateInfo.primitive_type = SDL_GPUPrimitiveType.SDL_GPU_PRIMITIVETYPE_TRIANGLELIST;
        pipelineCreateInfo.vertex_shader = vertexShader;
        pipelineCreateInfo.fragment_shader = fragmentShader;
        FillGraphicsPipelineSwapchainColorTarget(allocator, ref pipelineCreateInfo.target_info);
        FillGraphicsPipelineVertexAttributes<VertexPositionColor>(allocator, ref pipelineCreateInfo.vertex_input_state);
        FillGraphicsPipelineVertexBuffer<VertexPositionColor>(allocator, ref pipelineCreateInfo.vertex_input_state);

        _pipeline = SDL_CreateGPUGraphicsPipeline(Device, &pipelineCreateInfo);
        if (_pipeline == null)
        {
            Console.Error.WriteLine("Failed to create pipeline!");
            return false;
        }

        SDL_ReleaseGPUShader(Device, vertexShader);
        SDL_ReleaseGPUShader(Device, fragmentShader);

        _vertexBuffer = CreateVertexBuffer<VertexPositionColor>(9);
        if (_vertexBuffer == null)
        {
            Console.Error.WriteLine("Failed to create vertex buffer!");
            return false;
        }

        _indexBuffer = CreateIndexBuffer<VertexPositionColor>(6);
        if (_indexBuffer == null)
        {
            Console.Error.WriteLine("Failed to create index buffer!");
            return false;
        }

        var transferBufferCreateInfo = default(SDL_GPUTransferBufferCreateInfo);
        transferBufferCreateInfo.usage = SDL_GPUTransferBufferUsage.SDL_GPU_TRANSFERBUFFERUSAGE_UPLOAD;
        transferBufferCreateInfo.size = (uint)((sizeof(VertexPositionColor) * 9) + (sizeof(ushort) * 6));
        var transferBuffer = SDL_CreateGPUTransferBuffer(Device, &transferBufferCreateInfo);

        var data = SDL_MapGPUTransferBuffer(Device, transferBuffer, false);
        SDL_UnmapGPUTransferBuffer(Device, transferBuffer);

        var vertexData = (VertexPositionColor*)data;
        vertexData[0].Position = new Vector3(-1f, -1f, 0);
        vertexData[0].Color = Rgba8U.Red;

        vertexData[1].Position = new Vector3(1f, -1f, 0);
        vertexData[1].Color = Rgba8U.Lime;

        vertexData[2].Position = new Vector3(0, 1f, 0);
        vertexData[2].Color = Rgba8U.Blue;

        vertexData[3].Position = new Vector3(-1, -1, 0);
        vertexData[3].Color = new Rgba8U(255, 165, 0, 255);

        vertexData[4].Position = new Vector3(1, -1, 0);
        vertexData[4].Color = new Rgba8U(0, 128, 0, 255);

        vertexData[5].Position = new Vector3(0, 1, 0);
        vertexData[5].Color = Rgba8U.Cyan;

        vertexData[6].Position = new Vector3(-1, -1, 0);
        vertexData[6].Color = Rgba8U.White;

        vertexData[7].Position = new Vector3(1, -1, 0);
        vertexData[7].Color = Rgba8U.White;

        vertexData[8].Position = new Vector3(0, 1, 0);
        vertexData[8].Color = Rgba8U.White;

        var indexData = (ushort*)&vertexData[9];
        for (var i = 0; i < 6; i += 1)
        {
            indexData[i] = (ushort)i;
        }

        SDL_UnmapGPUTransferBuffer(Device, transferBuffer);

        var uploadCommandBuffer = SDL_AcquireGPUCommandBuffer(Device);
        var copyPass = SDL_BeginGPUCopyPass(uploadCommandBuffer);

        var bufferSourceVertex = default(SDL_GPUTransferBufferLocation);
        bufferSourceVertex.transfer_buffer = transferBuffer;
        bufferSourceVertex.offset = 0;
        var bufferDestinationVertex = default(SDL_GPUBufferRegion);
        bufferDestinationVertex.buffer = _vertexBuffer;
        bufferDestinationVertex.offset = 0;
        bufferDestinationVertex.size = (uint)(sizeof(VertexPositionColor) * 9);
        SDL_UploadToGPUBuffer(copyPass, &bufferSourceVertex, &bufferDestinationVertex, false);

        var bufferSourceIndex = default(SDL_GPUTransferBufferLocation);
        bufferSourceIndex.transfer_buffer = transferBuffer;
        bufferSourceIndex.offset = (uint)(sizeof(VertexPositionColor) * 9);
        var bufferDestinationIndex = default(SDL_GPUBufferRegion);
        bufferDestinationIndex.buffer = _indexBuffer;
        bufferDestinationIndex.offset = 0;
        bufferDestinationIndex.size = sizeof(ushort) * 6;
        SDL_UploadToGPUBuffer(copyPass, &bufferSourceIndex, &bufferDestinationIndex, false);

        SDL_EndGPUCopyPass(copyPass);
        SDL_SubmitGPUCommandBuffer(uploadCommandBuffer);
        SDL_ReleaseGPUTransferBuffer(Device, transferBuffer);

        // Finally, print instructions!
        Console.WriteLine("Press LEFT to enable/disable vertex offset");
        Console.WriteLine("Press RIGHT to enable/disable index offset");
        Console.WriteLine("Press UP to enable/disable index buffer");

        return true;
    }

    public override void Quit()
    {
        SDL_ReleaseGPUGraphicsPipeline(Device, _pipeline);
        SDL_ReleaseGPUBuffer(Device, _vertexBuffer);
        SDL_ReleaseGPUBuffer(Device, _indexBuffer);

        base.Quit();
    }

    public override void KeyboardEvent(SDL_KeyboardEvent e)
    {
        switch (e.scancode)
        {
            case SDL_Scancode.SDL_SCANCODE_LEFT:
                _isEnabledVertexOffset = !_isEnabledVertexOffset;
                Console.WriteLine("Using vertex offset: {0}", _isEnabledVertexOffset ? "true" : "false");
                break;
            case SDL_Scancode.SDL_SCANCODE_RIGHT:
                _isEnabledIndexOffset = !_isEnabledIndexOffset;
                Console.WriteLine("Using index offset: {0}", _isEnabledIndexOffset ? "true" : "false");
                break;
            case SDL_Scancode.SDL_SCANCODE_UP:
                _isEnabledIndexBuffer = !_isEnabledIndexBuffer;
                Console.WriteLine("Using index buffer: {0}", _isEnabledIndexBuffer ? "true" : "false");
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
        colorTargetInfo.clear_color = Rgba32F.Black;
        colorTargetInfo.load_op = SDL_GPULoadOp.SDL_GPU_LOADOP_CLEAR;
        colorTargetInfo.store_op = SDL_GPUStoreOp.SDL_GPU_STOREOP_STORE;

        var renderPass = SDL_BeginGPURenderPass(
            commandBuffer, &colorTargetInfo, 1, null);

        SDL_BindGPUGraphicsPipeline(renderPass, _pipeline);
        var bufferBindingVertex = default(SDL_GPUBufferBinding);
        bufferBindingVertex.buffer = _vertexBuffer;
        bufferBindingVertex.offset = 0;
        SDL_BindGPUVertexBuffers(renderPass, 0, &bufferBindingVertex, 1);

        var vertexOffset = (ushort)(_isEnabledVertexOffset ? 3 : 0);
        var indexOffset = (ushort)(_isEnabledIndexOffset ? 3 : 0);
        if (_isEnabledIndexBuffer)
        {
            var bufferBindingIndex = default(SDL_GPUBufferBinding);
            bufferBindingIndex.buffer = _indexBuffer;
            bufferBindingIndex.offset = 0;
            SDL_BindGPUIndexBuffer(renderPass, &bufferBindingIndex, SDL_GPUIndexElementSize.SDL_GPU_INDEXELEMENTSIZE_16BIT);
            SDL_DrawGPUIndexedPrimitives(renderPass, 3, 16, indexOffset, vertexOffset, 0);
        }
        else
        {
            SDL_DrawGPUPrimitives(renderPass, 3, 16, vertexOffset, 0);
        }

        SDL_EndGPURenderPass(renderPass);
        SDL_SubmitGPUCommandBuffer(commandBuffer);
        return true;
    }
}
