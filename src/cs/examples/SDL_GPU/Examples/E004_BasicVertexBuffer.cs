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

    public E004_BasicVertexBuffer()
        : base("4 - Basic Vertex Buffer")
    {
    }

    public override bool Initialize()
    {
        if (!base.Initialize())
        {
            return false;
        }

        var vertexShader = LoadShader(Device, "PositionColor.vert");
        if (vertexShader == null)
        {
            Console.Error.WriteLine("Failed to create vertex shader!");
            return false;
        }

        var fragmentShader = LoadShader(Device, "SolidColor.frag");
        if (fragmentShader == null)
        {
            Console.Error.WriteLine("Failed to create fragment shader!");
            return false;
        }

        _pipeline = CreatePipeline(vertexShader, fragmentShader);
        SDL_ReleaseGPUShader(Device, vertexShader);
        SDL_ReleaseGPUShader(Device, fragmentShader);
        if (_pipeline == null)
        {
            Console.Error.WriteLine("Failed to create pipeline!");
            return false;
        }

        _vertexBuffer = CreateVertexBuffer();
        if (_vertexBuffer == null)
        {
            Console.Error.WriteLine("Failed to create vertex buffer!");
            return false;
        }

        return true;
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
            Console.Error.WriteLine("Texture swapchain null!");
            return false;
        }

        var colorTargetInfo = default(SDL_GPUColorTargetInfo);
        colorTargetInfo.texture = textureSwapchain;
        colorTargetInfo.clear_color = Rgba32F.White;
        colorTargetInfo.load_op = SDL_GPULoadOp.SDL_GPU_LOADOP_CLEAR;
        colorTargetInfo.store_op = SDL_GPUStoreOp.SDL_GPU_STOREOP_STORE;

        var renderPass = SDL_BeginGPURenderPass(
            commandBuffer, &colorTargetInfo, 1, null);
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

    private SDL_GPUGraphicsPipeline* CreatePipeline(SDL_GPUShader* vertexShader, SDL_GPUShader* fragmentShader)
    {
        SDL_GPUColorTargetDescription colorTargetDescription = default;
        colorTargetDescription.format = SDL_GetGPUSwapchainTextureFormat(Device, Window);

        SDL_GPUGraphicsPipelineCreateInfo pipelineCreateInfo = default;
        pipelineCreateInfo.target_info.num_color_targets = 1;
        pipelineCreateInfo.target_info.color_target_descriptions = &colorTargetDescription;

        ref var vertexInputState = ref pipelineCreateInfo.vertex_input_state;

        vertexInputState.num_vertex_buffers = 1;
        var vertexBufferDescriptions = stackalloc SDL_GPUVertexBufferDescription[1];
        vertexInputState.vertex_buffer_descriptions = vertexBufferDescriptions;
        ref var vertexBufferDescription = ref vertexBufferDescriptions[0];
        vertexBufferDescription.slot = 0;
        vertexBufferDescription.input_rate = SDL_GPUVertexInputRate.SDL_GPU_VERTEXINPUTRATE_VERTEX;
        vertexBufferDescription.instance_step_rate = 0;
        vertexBufferDescription.pitch = (uint)sizeof(PositionColorVertex);

        vertexInputState.num_vertex_attributes = 2;
        var vertexAttributes = stackalloc SDL_GPUVertexAttribute[2];
        vertexInputState.vertex_attributes = vertexAttributes;

        ref var vertexAttribute1 = ref vertexAttributes[0];
        vertexAttribute1.buffer_slot = 0;
        vertexAttribute1.format = SDL_GPUVertexElementFormat.SDL_GPU_VERTEXELEMENTFORMAT_FLOAT3;
        vertexAttribute1.location = 0;
        vertexAttribute1.offset = 0;

        ref var vertexAttribute2 = ref vertexAttributes[1];
        vertexAttribute2.buffer_slot = 0;
        vertexAttribute2.format = SDL_GPUVertexElementFormat.SDL_GPU_VERTEXELEMENTFORMAT_UBYTE4_NORM;
        vertexAttribute2.location = 1;
        vertexAttribute2.offset = (uint)sizeof(Vector3);

        pipelineCreateInfo.primitive_type = SDL_GPUPrimitiveType.SDL_GPU_PRIMITIVETYPE_TRIANGLELIST;
        pipelineCreateInfo.vertex_shader = vertexShader;
        pipelineCreateInfo.fragment_shader = fragmentShader;

        pipelineCreateInfo.rasterizer_state.fill_mode = SDL_GPUFillMode.SDL_GPU_FILLMODE_FILL;
        return SDL_CreateGPUGraphicsPipeline(Device, &pipelineCreateInfo);
    }

    private SDL_GPUBuffer* CreateVertexBuffer()
    {
        var vertexBufferCreateInfo = default(SDL_GPUBufferCreateInfo);
        vertexBufferCreateInfo.usage = SDL_GPU_BUFFERUSAGE_VERTEX;
        vertexBufferCreateInfo.size = (uint)(sizeof(PositionColorVertex) * 3);
        var vertexBuffer = SDL_CreateGPUBuffer(Device, &vertexBufferCreateInfo);

        // To get data into the vertex buffer, we have to use a transfer buffer
        var transferBufferCreateInfo = default(SDL_GPUTransferBufferCreateInfo);
        transferBufferCreateInfo.usage = SDL_GPUTransferBufferUsage.SDL_GPU_TRANSFERBUFFERUSAGE_UPLOAD;
        transferBufferCreateInfo.size = (uint)(sizeof(PositionColorVertex) * 3);
        var transferBuffer = SDL_CreateGPUTransferBuffer(Device, &transferBufferCreateInfo);

        var transferData = (PositionColorVertex*)SDL_MapGPUTransferBuffer(Device, transferBuffer, false);

        transferData[0].Position = new Vector3(-1, -1, 0);
        transferData[0].Color = Rgba8U.Red;

        transferData[1].Position = new Vector3(1, -1, 0);
        transferData[1].Color = Rgba8U.Blue;

        transferData[2].Position = new Vector3(0, 1, 0);
        transferData[2].Color = Rgba8U.Lime;

        SDL_UnmapGPUTransferBuffer(Device, transferBuffer);

        // Upload the transfer data to the vertex buffer
        var uploadCommandBuffer = SDL_AcquireGPUCommandBuffer(Device);
        var copyPass = SDL_BeginGPUCopyPass(uploadCommandBuffer);

        var bufferSource = default(SDL_GPUTransferBufferLocation);
        bufferSource.transfer_buffer = transferBuffer;
        bufferSource.offset = 0;
        var bufferDestination = default(SDL_GPUBufferRegion);
        bufferDestination.buffer = vertexBuffer;
        bufferDestination.offset = 0;
        bufferDestination.size = (uint)(sizeof(PositionColorVertex) * 3);
        SDL_UploadToGPUBuffer(copyPass, &bufferSource, &bufferDestination, false);

        SDL_EndGPUCopyPass(copyPass);
        SDL_SubmitGPUCommandBuffer(uploadCommandBuffer);
        SDL_ReleaseGPUTransferBuffer(Device, transferBuffer);

        return vertexBuffer;
    }
}
