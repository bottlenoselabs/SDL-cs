// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Numerics;

namespace SDL_GPU;

public struct PositionColorVertex : IEquatable<PositionColorVertex>
{
    public Vector3 Position;
    public Rgba8U Color;

    public override bool Equals(object? obj)
    {
        return obj is PositionColorVertex other && Equals(other);
    }

    public bool Equals(PositionColorVertex other)
    {
        return Position == other.Position && Color == other.Color;
    }

    public readonly override int GetHashCode()
    {
        return HashCode.Combine(Position, Color);
    }

    public static bool operator ==(PositionColorVertex left, PositionColorVertex right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(PositionColorVertex left, PositionColorVertex right)
    {
        return !(left == right);
    }
}
