// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace SDL_GPU.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed unsafe class E003_BasicTriangle : ExampleGpu
{
    private SDL_GPUGraphicsPipeline* _pipelineFill;
    private SDL_GPUGraphicsPipeline* _pipelineLine;
    private SDL_GPUViewport _viewportSmall = new()
    {
        x = 160, y = 120, w = 320, h = 240, min_depth = 0.1f, max_depth = 1.0f
    };

    private SDL_Rect _rectangleScissor = new()
    {
        x = 320, y = 240, w = 320, h = 240
    };

    private bool _isEnabledWireframeMode;
    private bool _isEnabledSmallViewport;
    private bool _isEnabledScissorRectangle;

    public E003_BasicTriangle()
        : base("3 - Basic Triangle")
    {
    }

    public override bool Initialize()
    {
        if (!base.Initialize())
        {
            return false;
        }

        // Create the shaders
        var vertexShader = LoadShader(Device, "RawTriangle.vert");
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

        // Create the pipelines
        SDL_GPUColorTargetDescription targetDescription = default;
        targetDescription.format = SDL_GetGPUSwapchainTextureFormat(Device, Window);

        SDL_GPUGraphicsPipelineCreateInfo pipelineCreateInfo = default;
        pipelineCreateInfo.target_info.num_color_targets = 1;
        pipelineCreateInfo.target_info.color_target_descriptions = &targetDescription;
        pipelineCreateInfo.primitive_type = SDL_GPUPrimitiveType.SDL_GPU_PRIMITIVETYPE_TRIANGLELIST;
        pipelineCreateInfo.vertex_shader = vertexShader;
        pipelineCreateInfo.fragment_shader = fragmentShader;

        pipelineCreateInfo.rasterizer_state.fill_mode = SDL_GPUFillMode.SDL_GPU_FILLMODE_FILL;
        _pipelineFill = SDL_CreateGPUGraphicsPipeline(Device, &pipelineCreateInfo);
        if (_pipelineFill == null)
        {
            Console.Error.WriteLine("Failed to create fill pipeline!");
            return false;
        }

        pipelineCreateInfo.rasterizer_state.fill_mode = SDL_GPUFillMode.SDL_GPU_FILLMODE_LINE;
        _pipelineLine = SDL_CreateGPUGraphicsPipeline(Device, &pipelineCreateInfo);
        if (_pipelineLine == null)
        {
            Console.Error.WriteLine("Failed to create line pipeline!");
            return false;
        }

        // Clean up shader resources
        SDL_ReleaseGPUShader(Device, vertexShader);
        SDL_ReleaseGPUShader(Device, fragmentShader);

        // Finally, print instructions!
        Console.WriteLine("Press Left to toggle wireframe mode");
        Console.WriteLine("Press Down to toggle small viewport");
        Console.WriteLine("Press Right to toggle scissor rect");

        return true;
    }

    public override void KeyboardEvent(SDL_KeyboardEvent e)
    {
        if (e.scancode == SDL_Scancode.SDL_SCANCODE_LEFT)
        {
            _isEnabledWireframeMode = !_isEnabledWireframeMode;
        }

        if (e.scancode == SDL_Scancode.SDL_SCANCODE_DOWN)
        {
            _isEnabledSmallViewport = !_isEnabledSmallViewport;
        }

        if (e.scancode == SDL_Scancode.SDL_SCANCODE_RIGHT)
        {
            _isEnabledScissorRectangle = !_isEnabledScissorRectangle;
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

        if (textureSwapchain != null)
        {
            SDL_GPUColorTargetInfo colorTargetInfo = default;
            colorTargetInfo.texture = textureSwapchain;
            colorTargetInfo.clear_color = Rgba32F.Black;
            colorTargetInfo.load_op = SDL_GPULoadOp.SDL_GPU_LOADOP_CLEAR;
            colorTargetInfo.store_op = SDL_GPUStoreOp.SDL_GPU_STOREOP_STORE;

            var renderPass = SDL_BeginGPURenderPass(
                commandBuffer, &colorTargetInfo, 1, null);
            SDL_BindGPUGraphicsPipeline(renderPass, _isEnabledWireframeMode ? _pipelineLine : _pipelineFill);
            if (_isEnabledSmallViewport)
            {
                ref var viewportSmall = ref _viewportSmall;
                SDL_SetGPUViewport(renderPass, (SDL_GPUViewport*)Unsafe.AsPointer(ref viewportSmall));
            }

            if (_isEnabledScissorRectangle)
            {
                ref var rectangleScissor = ref _rectangleScissor;
                SDL_SetGPUScissor(renderPass, (SDL_Rect*)Unsafe.AsPointer(ref rectangleScissor));
            }

            SDL_DrawGPUPrimitives(renderPass, 3, 1, 0, 0);
            SDL_EndGPURenderPass(renderPass);
        }

        SDL_SubmitGPUCommandBuffer(commandBuffer);
        return true;
    }
}
