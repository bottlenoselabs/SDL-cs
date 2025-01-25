// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Numerics;
using JetBrains.Annotations;

namespace SDL_GPU.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed unsafe class E005_CullMode : ExampleGpu
{
    private static readonly string[] ModeNames =
    {
        "CW_CullNone",
        "CW_CullFront",
        "CW_CullBack",
        "CCW_CullNone",
        "CCW_CullFront",
        "CCW_CullBack"
    };

    private SDL_GPUGraphicsPipeline*[] _pipelines = new SDL_GPUGraphicsPipeline*[ModeNames.Length];
    private int _currentModeIndex;
    private SDL_GPUBuffer* _vertexBufferCw;
    private SDL_GPUBuffer* _vertexBufferCcw;

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

        _pipelines = CreatePipelines(vertexShader, fragmentShader);
        foreach (var pipeline in _pipelines)
        {
            if (pipeline == null)
            {
                Console.Error.WriteLine("Failed to create pipeline!");
                return false;
            }
        }

        // Clean up shader resources
        SDL_ReleaseGPUShader(Device, vertexShader);
        SDL_ReleaseGPUShader(Device, fragmentShader);

        // Create the vertex buffers. They're the same except for the vertex order.
        CreateVertexBuffers(out _vertexBufferCw, out _vertexBufferCcw);
        if (_vertexBufferCw == null || _vertexBufferCcw == null)
        {
            Console.Error.WriteLine("Failed to create vertex buffer!");
            return false;
        }

        // Finally, print instructions!
        Console.WriteLine("Press Left/Right to switch between modes");
        Console.WriteLine("Current Mode: " + ModeNames[0]);

        return true;
    }

    public override void KeyboardEvent(SDL_KeyboardEvent e)
    {
        if (e.scancode == SDL_Scancode.SDL_SCANCODE_LEFT)
        {
            _currentModeIndex -= 1;
            if (_currentModeIndex < 0)
            {
                _currentModeIndex = ModeNames.Length - 1;
            }

            Console.WriteLine("Current Mode: " + ModeNames[_currentModeIndex]);
        }
        else if (e.scancode == SDL_Scancode.SDL_SCANCODE_RIGHT)
        {
            _currentModeIndex = (_currentModeIndex + 1) % ModeNames.Length;
            Console.WriteLine("Current Mode: " + ModeNames[_currentModeIndex]);
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
            Console.Error.WriteLine("Texture swapchain null!");
            return false;
        }

        var colorTargetInfo = default(SDL_GPUColorTargetInfo);
        colorTargetInfo.texture = textureSwapchain;
        colorTargetInfo.clear_color = Rgba32F.Black;
        colorTargetInfo.load_op = SDL_GPULoadOp.SDL_GPU_LOADOP_CLEAR;
        colorTargetInfo.store_op = SDL_GPUStoreOp.SDL_GPU_STOREOP_STORE;

        var renderPass = SDL_BeginGPURenderPass(
            commandBuffer, &colorTargetInfo, 1, null);
        SDL_BindGPUGraphicsPipeline(renderPass, _pipelines[_currentModeIndex]);

        var viewport1 = new SDL_GPUViewport { x = 0, y = 0, w = 320, h = 480 };
        SDL_SetGPUViewport(renderPass, &viewport1);
        var bufferBinding1 = default(SDL_GPUBufferBinding);
        bufferBinding1.buffer = _vertexBufferCw;
        bufferBinding1.offset = 0;
        SDL_BindGPUVertexBuffers(renderPass, 0, &bufferBinding1, 1);
        SDL_DrawGPUPrimitives(renderPass, 3, 1, 0, 0);

        var viewport2 = new SDL_GPUViewport { x = 320, y = 0, w = 320, h = 480 };
        SDL_SetGPUViewport(renderPass, &viewport2);
        var bufferBinding2 = default(SDL_GPUBufferBinding);
        bufferBinding2.buffer = _vertexBufferCcw;
        bufferBinding2.offset = 0;
        SDL_BindGPUVertexBuffers(renderPass, 0, &bufferBinding2, 1);
        SDL_DrawGPUPrimitives(renderPass, 3, 1, 0, 0);

        SDL_EndGPURenderPass(renderPass);
        SDL_SubmitGPUCommandBuffer(commandBuffer);
        return true;
    }

    private SDL_GPUGraphicsPipeline*[] CreatePipelines(SDL_GPUShader* vertexShader, SDL_GPUShader* fragmentShader)
    {
        var colorTargetDescriptions = stackalloc SDL_GPUColorTargetDescription[1];
        ref var colorTargetDescription1 = ref colorTargetDescriptions[0];
        colorTargetDescription1.format = SDL_GetGPUSwapchainTextureFormat(Device, Window);

        SDL_GPUGraphicsPipelineCreateInfo pipelineCreateInfo = default;
        pipelineCreateInfo.target_info.num_color_targets = 1;
        pipelineCreateInfo.target_info.color_target_descriptions = colorTargetDescriptions;

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

        var pipelineCount = ModeNames.Length;
        var pipelines = new SDL_GPUGraphicsPipeline*[pipelineCount];
        for (var i = 0; i < pipelineCount; i += 1)
        {
            pipelineCreateInfo.rasterizer_state.cull_mode = (SDL_GPUCullMode)(i % 3);
            pipelineCreateInfo.rasterizer_state.front_face = i > 2 ?
                SDL_GPUFrontFace.SDL_GPU_FRONTFACE_CLOCKWISE :
                SDL_GPUFrontFace.SDL_GPU_FRONTFACE_COUNTER_CLOCKWISE;

            pipelines[i] = SDL_CreateGPUGraphicsPipeline(Device, &pipelineCreateInfo);
        }

        return pipelines;
    }

    private void CreateVertexBuffers(out SDL_GPUBuffer* vertexBufferCw, out SDL_GPUBuffer* vertexBufferCcw)
    {
        var vertexBufferCreateInfo = default(SDL_GPUBufferCreateInfo);
        vertexBufferCreateInfo.usage = SDL_GPU_BUFFERUSAGE_VERTEX;
        vertexBufferCreateInfo.size = (uint)(sizeof(PositionColorVertex) * 3);
        vertexBufferCw = SDL_CreateGPUBuffer(Device, &vertexBufferCreateInfo);
        vertexBufferCcw = SDL_CreateGPUBuffer(Device, &vertexBufferCreateInfo);

        // To get data into the vertex buffer, we have to use a transfer buffer
        var transferBufferCreateInfo = default(SDL_GPUTransferBufferCreateInfo);
        transferBufferCreateInfo.usage = SDL_GPUTransferBufferUsage.SDL_GPU_TRANSFERBUFFERUSAGE_UPLOAD;
        transferBufferCreateInfo.size = (uint)(sizeof(PositionColorVertex) * 6);
        var transferBuffer = SDL_CreateGPUTransferBuffer(Device, &transferBufferCreateInfo);

        var transferData = (PositionColorVertex*)SDL_MapGPUTransferBuffer(Device, transferBuffer, false);

        transferData[0].Position = new Vector3(-1, -1, 0);
        transferData[0].Color = Rgba8U.Red;

        transferData[1].Position = new Vector3(1, -1, 0);
        transferData[1].Color = Rgba8U.Lime;

        transferData[2].Position = new Vector3(0, 1, 0);
        transferData[2].Color = Rgba8U.Blue;

        transferData[3].Position = new Vector3(0, 1, 0);
        transferData[3].Color = Rgba8U.Red;

        transferData[4].Position = new Vector3(1, -1, 0);
        transferData[4].Color = Rgba8U.Lime;

        transferData[5].Position = new Vector3(-1, -1, 0);
        transferData[5].Color = Rgba8U.Blue;

        SDL_UnmapGPUTransferBuffer(Device, transferBuffer);

        // Upload the transfer data to the vertex buffer
        var uploadCommandBuffer = SDL_AcquireGPUCommandBuffer(Device);
        var copyPass = SDL_BeginGPUCopyPass(uploadCommandBuffer);

        var bufferSourceCw = default(SDL_GPUTransferBufferLocation);
        bufferSourceCw.transfer_buffer = transferBuffer;
        bufferSourceCw.offset = 0;
        var bufferDestinationCw = default(SDL_GPUBufferRegion);
        bufferDestinationCw.buffer = vertexBufferCw;
        bufferDestinationCw.offset = 0;
        bufferDestinationCw.size = (uint)(sizeof(PositionColorVertex) * 3);
        SDL_UploadToGPUBuffer(copyPass, &bufferSourceCw, &bufferDestinationCw, false);

        var bufferSourceCww = default(SDL_GPUTransferBufferLocation);
        bufferSourceCww.transfer_buffer = transferBuffer;
        bufferSourceCww.offset = (uint)(sizeof(PositionColorVertex) * 3);
        var bufferDestinationCcw = default(SDL_GPUBufferRegion);
        bufferDestinationCcw.buffer = vertexBufferCcw;
        bufferDestinationCcw.offset = 0;
        bufferDestinationCcw.size = (uint)(sizeof(PositionColorVertex) * 3);
        SDL_UploadToGPUBuffer(copyPass, &bufferSourceCww, &bufferDestinationCcw, false);

        SDL_EndGPUCopyPass(copyPass);
        SDL_SubmitGPUCommandBuffer(uploadCommandBuffer);
        SDL_ReleaseGPUTransferBuffer(Device, transferBuffer);
    }
}
