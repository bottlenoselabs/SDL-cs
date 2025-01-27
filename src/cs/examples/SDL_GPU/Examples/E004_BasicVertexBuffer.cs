// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Numerics;
using JetBrains.Annotations;

namespace SDL_GPU.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed unsafe class E004_BasicVertexBuffer : ExampleGpu
{
    private SDL_GPUGraphicsPipeline* _pipeline;
    private SDL_GPUBuffer* _vertexBuffer;

    public override bool Initialize(IAllocator allocator)
    {
        if (!base.Initialize(allocator))
        {
            return false;
        }

        var vertexShader = CreateShader("PositionColor.vert");
        if (vertexShader == null)
        {
            Console.Error.WriteLine("Failed to create vertex shader!");
            return false;
        }

        var fragmentShader = CreateShader("SolidColor.frag");
        if (fragmentShader == null)
        {
            Console.Error.WriteLine("Failed to create fragment shader!");
            return false;
        }

        // Create the pipeline
        var createInfo = default(SDL_GPUGraphicsPipelineCreateInfo);
        createInfo.primitive_type = SDL_GPUPrimitiveType.SDL_GPU_PRIMITIVETYPE_TRIANGLELIST;
        createInfo.vertex_shader = vertexShader;
        createInfo.fragment_shader = fragmentShader;
        createInfo.rasterizer_state.fill_mode = SDL_GPUFillMode.SDL_GPU_FILLMODE_FILL;
        FillGraphicsPipelineSwapchainColorTarget(allocator, ref createInfo.target_info);
        FillGraphicsPipelineVertexAttributes<VertexPositionColor>(allocator, ref createInfo.vertex_input_state);
        FillGraphicsPipelineVertexBuffer<VertexPositionColor>(allocator, ref createInfo.vertex_input_state);

        _pipeline = SDL_CreateGPUGraphicsPipeline(Device, &createInfo);
        if (_pipeline == null)
        {
            Console.Error.WriteLine("Failed to create pipeline!");
            return false;
        }

        SDL_ReleaseGPUShader(Device, vertexShader);
        SDL_ReleaseGPUShader(Device, fragmentShader);

        _vertexBuffer = CreateVertexBuffer<VertexPositionColor>(3);
        if (_vertexBuffer == null)
        {
            Console.Error.WriteLine("Failed to create vertex buffer!");
            return false;
        }

        var transferBuffer = CreateTransferBuffer<VertexPositionColor>(
            3, static data =>
        {
            data[0].Position = new Vector3(-1, -1, 0);
            data[0].Color = Rgba8U.Red;

            data[1].Position = new Vector3(1, -1, 0);
            data[1].Color = Rgba8U.Lime;

            data[2].Position = new Vector3(0, 1, 0);
            data[2].Color = Rgba8U.Blue;
        });

        // Upload transfer data to vertex buffer
        var uploadCommandBuffer = SDL_AcquireGPUCommandBuffer(Device);
        var copyPass = SDL_BeginGPUCopyPass(uploadCommandBuffer);

        var bufferSource = default(SDL_GPUTransferBufferLocation);
        bufferSource.transfer_buffer = transferBuffer;
        bufferSource.offset = 0;
        var bufferDestination = default(SDL_GPUBufferRegion);
        bufferDestination.buffer = _vertexBuffer;
        bufferDestination.offset = 0;
        bufferDestination.size = (uint)(sizeof(VertexPositionColor) * 3);
        SDL_UploadToGPUBuffer(copyPass, &bufferSource, &bufferDestination, false);

        SDL_EndGPUCopyPass(copyPass);
        SDL_SubmitGPUCommandBuffer(uploadCommandBuffer);
        SDL_ReleaseGPUTransferBuffer(Device, transferBuffer);

        return true;
    }

    public override void Quit()
    {
        SDL_ReleaseGPUGraphicsPipeline(Device, _pipeline);
        SDL_ReleaseGPUBuffer(Device, _vertexBuffer);

        base.Quit();
    }

    public override void KeyboardEvent(SDL_KeyboardEvent e)
    {
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
            commandBuffer,
            (SDL_GPUColorTargetInfo*)Unsafe.AsPointer(ref colorTargetInfo),
            1,
            null);
        SDL_BindGPUGraphicsPipeline(renderPass, _pipeline);

        var bufferBinding = default(SDL_GPUBufferBinding);
        bufferBinding.buffer = _vertexBuffer;
        bufferBinding.offset = 0;
        SDL_BindGPUVertexBuffers(renderPass, 0, &bufferBinding, 1);
        SDL_DrawGPUPrimitives(renderPass, 3, 1, 0, 0);

        SDL_EndGPURenderPass(renderPass);
        SDL_SubmitGPUCommandBuffer(commandBuffer);
        return true;
    }
}
