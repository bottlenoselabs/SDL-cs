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
}
