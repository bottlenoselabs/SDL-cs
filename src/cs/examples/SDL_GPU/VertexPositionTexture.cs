// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Numerics;

namespace SDL_GPU;

public struct VertexPositionTexture : IEquatable<VertexPositionTexture>
{
    public Vector3 Position;
    public Vector2 TextureCoordinates;

    public override bool Equals(object? obj)
    {
        return obj is VertexPositionTexture other && Equals(other);
    }

    public bool Equals(VertexPositionTexture other)
    {
        return Position == other.Position && TextureCoordinates == other.TextureCoordinates;
    }

    public readonly override int GetHashCode()
    {
        return HashCode.Combine(Position, TextureCoordinates);
    }

    public static bool operator ==(VertexPositionTexture left, VertexPositionTexture right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(VertexPositionTexture left, VertexPositionTexture right)
    {
        return !(left == right);
    }
}
