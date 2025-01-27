// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using JetBrains.Annotations;

namespace SDL_GPU;

[PublicAPI]
public static unsafe class AllocatorExtensions
{
    public static T* AllocateSingle<T>(this IAllocator allocator)
        where T : unmanaged
    {
        var size = sizeof(T);
        return (T*)allocator.Allocate(size);
    }

    public static T* AllocateArray<T>(this IAllocator allocator, int elementCount)
        where T : unmanaged
    {
        var size = sizeof(T) * elementCount;
        return (T*)allocator.Allocate(size);
    }
}
