// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using JetBrains.Annotations;

namespace SDL_GPU.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed unsafe class E002_ClearScreenMultipleWindow : ExampleGpu
{
    private SDL_Window* _secondWindow;

    public override bool Initialize(IAllocator allocator)
    {
        if (!base.Initialize(allocator))
        {
            return false;
        }

        using var windowNameCString = (CString)$"{Name} (2)";
        _secondWindow = SDL_CreateWindow(windowNameCString, 640, 480, 0);
        if (_secondWindow == null)
        {
            Console.Error.WriteLine("CreateWindow failed: " + SDL_GetError());
            return false;
        }

        SDL_SetWindowPosition(_secondWindow, 0, 0);

        if (!SDL_ClaimWindowForGPUDevice(Device, _secondWindow))
        {
            Console.Error.WriteLine("GPUClaimWindow failed");
            return false;
        }

        return true;
    }

    public override void Quit()
    {
        SDL_ReleaseWindowFromGPUDevice(Device, _secondWindow);
        SDL_DestroyWindow(_secondWindow);
        _secondWindow = null;

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

        SDL_GPUTexture* textureSwapchainMainWindow;
        if (!SDL_WaitAndAcquireGPUSwapchainTexture(
                commandBuffer,
                Window,
                &textureSwapchainMainWindow,
                null,
                null))
        {
            Console.Error.WriteLine("WaitAndAcquireGPUSwapchainTexture failed: " + SDL_GetError());
            return false;
        }

        if (textureSwapchainMainWindow != null)
        {
            SDL_GPUColorTargetInfo colorTargetInfo = default;
            colorTargetInfo.texture = textureSwapchainMainWindow;
            colorTargetInfo.clear_color = Rgba32F.CornflowerBlue;
            colorTargetInfo.load_op = SDL_GPULoadOp.SDL_GPU_LOADOP_CLEAR;
            colorTargetInfo.store_op = SDL_GPUStoreOp.SDL_GPU_STOREOP_STORE;

            var renderPass = SDL_BeginGPURenderPass(
                commandBuffer, &colorTargetInfo, 1, null);
            // No rendering in this example!
            SDL_EndGPURenderPass(renderPass);
        }

        SDL_GPUTexture* textureSwapchainSecondWindow;
        if (!SDL_WaitAndAcquireGPUSwapchainTexture(
                commandBuffer,
                _secondWindow,
                &textureSwapchainSecondWindow,
                null,
                null))
        {
            Console.Error.WriteLine("WaitAndAcquireGPUSwapchainTexture failed: " + SDL_GetError());
            return false;
        }

        if (textureSwapchainSecondWindow != null)
        {
            SDL_GPUColorTargetInfo colorTargetInfo = default;
            colorTargetInfo.texture = textureSwapchainSecondWindow;
            colorTargetInfo.clear_color = Rgba32F.Indigo;
            colorTargetInfo.load_op = SDL_GPULoadOp.SDL_GPU_LOADOP_CLEAR;
            colorTargetInfo.store_op = SDL_GPUStoreOp.SDL_GPU_STOREOP_STORE;

            var renderPass = SDL_BeginGPURenderPass(
                commandBuffer, &colorTargetInfo, 1, null);
            SDL_EndGPURenderPass(renderPass);
        }

        SDL_SubmitGPUCommandBuffer(commandBuffer);
        return true;
    }
}
