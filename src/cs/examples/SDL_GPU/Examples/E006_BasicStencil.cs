// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Numerics;
using JetBrains.Annotations;

namespace SDL_GPU.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed unsafe class E006_BasicStencil : ExampleGpu
{
    private bool _isSupported = true;
    private SDL_GPUGraphicsPipeline* _pipelineMasker;
    private SDL_GPUGraphicsPipeline* _pipelineMaskee;
    private SDL_GPUBuffer* _vertexBuffer;
    private SDL_GPUTexture* _textureDepthStencilTarget;

    public override bool Initialize(IAllocator allocator)
    {
        if (!base.Initialize(allocator))
        {
            return false;
        }

        var depthStencilFormat = GetSupportedDepthStencilFormat();
        if (depthStencilFormat == SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_INVALID)
        {
            Console.WriteLine("Stencil formats not supported!");
            _isSupported = false;
            return true;
        }

        int width, height;
        SDL_GetWindowSizeInPixels(Window, &width, &height);
        var textureCreateInfo = default(SDL_GPUTextureCreateInfo);
        textureCreateInfo.type = SDL_GPUTextureType.SDL_GPU_TEXTURETYPE_2D;
        textureCreateInfo.width = (uint)width;
        textureCreateInfo.height = (uint)height;
        textureCreateInfo.layer_count_or_depth = 1;
        textureCreateInfo.num_levels = 1;
        textureCreateInfo.sample_count = SDL_GPUSampleCount.SDL_GPU_SAMPLECOUNT_1;
        textureCreateInfo.format = depthStencilFormat;
        textureCreateInfo.usage = SDL_GPU_TEXTUREUSAGE_DEPTH_STENCIL_TARGET;
        _textureDepthStencilTarget = SDL_CreateGPUTexture(Device, &textureCreateInfo);
        if (_textureDepthStencilTarget == null)
        {
            Console.Error.WriteLine("Failed to create texture!");
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

        var pipelineCreateInfo = default(SDL_GPUGraphicsPipelineCreateInfo);
        pipelineCreateInfo.primitive_type = SDL_GPUPrimitiveType.SDL_GPU_PRIMITIVETYPE_TRIANGLELIST;
        pipelineCreateInfo.vertex_shader = vertexShader;
        pipelineCreateInfo.fragment_shader = fragmentShader;
        FillGraphicsPipelineSwapchainColorTarget(allocator, ref pipelineCreateInfo.target_info);
        FillGraphicsPipelineVertexAttributes<VertexPositionColor>(allocator, ref pipelineCreateInfo.vertex_input_state);
        FillGraphicsPipelineVertexBuffer<VertexPositionColor>(allocator, ref pipelineCreateInfo.vertex_input_state);

        ref var targetInfo = ref pipelineCreateInfo.target_info;
        targetInfo.has_depth_stencil_target = true;
        targetInfo.depth_stencil_format = depthStencilFormat;

        ref var depthStencilState = ref pipelineCreateInfo.depth_stencil_state;
        depthStencilState.enable_stencil_test = true;
        depthStencilState.write_mask = 0xFF;
        ref var frontStencilState = ref depthStencilState.front_stencil_state;
        frontStencilState.compare_op = SDL_GPUCompareOp.SDL_GPU_COMPAREOP_NEVER;
        frontStencilState.fail_op = SDL_GPUStencilOp.SDL_GPU_STENCILOP_REPLACE;
        frontStencilState.pass_op = SDL_GPUStencilOp.SDL_GPU_STENCILOP_KEEP;
        frontStencilState.depth_fail_op = SDL_GPUStencilOp.SDL_GPU_STENCILOP_KEEP;
        ref var backStencilState = ref depthStencilState.back_stencil_state;
        backStencilState.compare_op = SDL_GPUCompareOp.SDL_GPU_COMPAREOP_NEVER;
        backStencilState.fail_op = SDL_GPUStencilOp.SDL_GPU_STENCILOP_REPLACE;
        backStencilState.pass_op = SDL_GPUStencilOp.SDL_GPU_STENCILOP_KEEP;
        backStencilState.depth_fail_op = SDL_GPUStencilOp.SDL_GPU_STENCILOP_KEEP;

        ref var rasterizerState = ref pipelineCreateInfo.rasterizer_state;
        rasterizerState.cull_mode = SDL_GPUCullMode.SDL_GPU_CULLMODE_NONE;
        rasterizerState.fill_mode = SDL_GPUFillMode.SDL_GPU_FILLMODE_FILL;
        rasterizerState.front_face = SDL_GPUFrontFace.SDL_GPU_FRONTFACE_COUNTER_CLOCKWISE;

        _pipelineMasker = SDL_CreateGPUGraphicsPipeline(Device, &pipelineCreateInfo);
        if (_pipelineMasker == null)
        {
            Console.Error.WriteLine("Failed to create pipeline!");
            return false;
        }

        depthStencilState = default; // reset
        depthStencilState.enable_stencil_test = true;
        depthStencilState.compare_mask = 0xFF;
        depthStencilState.write_mask = 0;
        frontStencilState.compare_op = SDL_GPUCompareOp.SDL_GPU_COMPAREOP_EQUAL;
        frontStencilState.fail_op = SDL_GPUStencilOp.SDL_GPU_STENCILOP_KEEP;
        frontStencilState.pass_op = SDL_GPUStencilOp.SDL_GPU_STENCILOP_KEEP;
        frontStencilState.depth_fail_op = SDL_GPUStencilOp.SDL_GPU_STENCILOP_KEEP;
        backStencilState.compare_op = SDL_GPUCompareOp.SDL_GPU_COMPAREOP_NEVER;
        backStencilState.fail_op = SDL_GPUStencilOp.SDL_GPU_STENCILOP_KEEP;
        backStencilState.pass_op = SDL_GPUStencilOp.SDL_GPU_STENCILOP_KEEP;
        backStencilState.depth_fail_op = SDL_GPUStencilOp.SDL_GPU_STENCILOP_KEEP;

        _pipelineMaskee = SDL_CreateGPUGraphicsPipeline(Device, &pipelineCreateInfo);
        if (_pipelineMaskee == null)
        {
            Console.Error.WriteLine("Failed to create pipeline!");
            return false;
        }

        SDL_ReleaseGPUShader(Device, vertexShader);
        SDL_ReleaseGPUShader(Device, fragmentShader);

        _vertexBuffer = CreateVertexBuffer<VertexPositionColor>(6);
        if (_vertexBuffer == null)
        {
            Console.Error.WriteLine("Failed to create vertex buffer!");
            return false;
        }

        var transferBuffer = CreateTransferBuffer<VertexPositionColor>(
            6, static data =>
            {
                data[0].Position = new Vector3(-0.5f, -0.5f, 0);
                data[0].Color = Rgba8U.Yellow;

                data[1].Position = new Vector3(0.5f, -0.5f, 0);
                data[1].Color = Rgba8U.Yellow;

                data[2].Position = new Vector3(0, 0.5f, 0);
                data[2].Color = Rgba8U.Yellow;

                data[3].Position = new Vector3(-1, -1, 0);
                data[3].Color = Rgba8U.Red;

                data[4].Position = new Vector3(1, -1, 0);
                data[4].Color = Rgba8U.Lime;

                data[5].Position = new Vector3(0, 1, 0);
                data[5].Color = Rgba8U.Blue;
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
        bufferDestination.size = (uint)(sizeof(VertexPositionColor) * 6);
        SDL_UploadToGPUBuffer(copyPass, &bufferSource, &bufferDestination, false);

        SDL_EndGPUCopyPass(copyPass);
        SDL_SubmitGPUCommandBuffer(uploadCommandBuffer);
        SDL_ReleaseGPUTransferBuffer(Device, transferBuffer);

        return true;
    }

    public override void Quit()
    {
        if (_isSupported)
        {
            SDL_ReleaseGPUGraphicsPipeline(Device, _pipelineMasker);
            SDL_ReleaseGPUGraphicsPipeline(Device, _pipelineMaskee);

            SDL_ReleaseGPUTexture(Device, _textureDepthStencilTarget);
            SDL_ReleaseGPUBuffer(Device, _vertexBuffer);
        }

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
        if (!_isSupported)
        {
            return true;
        }

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

        var depthStencilTargetInfo = default(SDL_GPUDepthStencilTargetInfo);
        depthStencilTargetInfo.texture = _textureDepthStencilTarget;
        depthStencilTargetInfo.cycle = true;
        depthStencilTargetInfo.clear_depth = 0;
        depthStencilTargetInfo.clear_stencil = 0;
        depthStencilTargetInfo.load_op = SDL_GPULoadOp.SDL_GPU_LOADOP_CLEAR;
        depthStencilTargetInfo.store_op = SDL_GPUStoreOp.SDL_GPU_STOREOP_DONT_CARE;
        depthStencilTargetInfo.stencil_load_op = SDL_GPULoadOp.SDL_GPU_LOADOP_CLEAR;
        depthStencilTargetInfo.stencil_store_op = SDL_GPUStoreOp.SDL_GPU_STOREOP_DONT_CARE;

        var renderPass = SDL_BeginGPURenderPass(
            commandBuffer, &colorTargetInfo, 1, &depthStencilTargetInfo);

        SDL_BindGPUGraphicsPipeline(renderPass, _pipelineMasker);
        var bufferBinding = default(SDL_GPUBufferBinding);
        bufferBinding.buffer = _vertexBuffer;
        bufferBinding.offset = 0;
        SDL_BindGPUVertexBuffers(renderPass, 0, &bufferBinding, 1);

        SDL_SetGPUStencilReference(renderPass, 1);
        SDL_BindGPUGraphicsPipeline(renderPass, _pipelineMasker);
        SDL_DrawGPUPrimitives(renderPass, 3, 1, 0, 0);

        SDL_SetGPUStencilReference(renderPass, 0);
        SDL_BindGPUGraphicsPipeline(renderPass, _pipelineMaskee);
        SDL_DrawGPUPrimitives(renderPass, 3, 1, 3, 0);

        SDL_EndGPURenderPass(renderPass);
        SDL_SubmitGPUCommandBuffer(commandBuffer);
        return true;
    }
}
