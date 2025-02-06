// <auto-generated>
//  This code was generated by the following tool on 2025-02-06 00:45:33 GMT+00:00:
//      https://github.com/bottlenoselabs/c2cs (v2025.1.28.0)
//
//  Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
// ReSharper disable All

// To disable generating this file set `isEnabledGeneratingRuntimeCode` to `false` in the config file for generating C# code.

using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Interop.Runtime;

/// <summary>
///     A boolean value type with the same memory layout as a <see cref="byte" /> in both managed and unmanaged contexts;
///     equivalent to a standard bool found in C/C++/ObjC where <c>0</c> is <c>false</c> and any other value is
///     <c>true</c>.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct CBool : IEquatable<CBool>
{
    /// <summary>
    ///     The value.
    /// </summary>
    public readonly byte Value;

    private CBool(bool value)
    {
        Value = Convert.ToByte(value);
    }

    /// <summary>
    ///     Converts the specified <see cref="bool" /> to a <see cref="CBool" />.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="CBool" />.</returns>
    public static implicit operator CBool(bool value)
    {
        return FromBoolean(value);
    }

    /// <summary>
    ///     Converts the specified <see cref="bool" /> to a <see cref="CBool" />.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="CBool" />.</returns>
    public static CBool FromBoolean(bool value)
    {
        return new CBool(value);
    }

    /// <summary>
    ///     Converts the specified <see cref="CBool" /> to a <see cref="bool" />.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="bool" />.</returns>
    public static implicit operator bool(CBool value)
    {
        return ToBoolean(value);
    }

    /// <summary>
    ///     Converts the specified <see cref="CBool" /> to a <see cref="bool" />.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="bool" />.</returns>
    public static bool ToBoolean(CBool value)
    {
        return Convert.ToBoolean(value.Value);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return ToBoolean(this).ToString();
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is CBool b && Equals(b);
    }

    /// <inheritdoc />
    public bool Equals(CBool other)
    {
        return Value == other.Value;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    /// <summary>
    ///     Returns a value that indicates whether two specified <see cref="CBool" /> structures are equal.
    /// </summary>
    /// <param name="left">The first <see cref="CBool" /> to compare.</param>
    /// <param name="right">The second <see cref="CBool" /> to compare.</param>
    /// <returns><c>true</c> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(CBool left, CBool right)
    {
        return left.Value == right.Value;
    }

    /// <summary>
    ///     Returns a value that indicates whether two specified <see cref="CBool" /> structures are not equal.
    /// </summary>
    /// <param name="left">The first <see cref="CBool" /> to compare.</param>
    /// <param name="right">The second <see cref="CBool" /> to compare.</param>
    /// <returns><c>true</c> if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(CBool left, CBool right)
    {
        return !(left == right);
    }

    /// <summary>
    ///     Returns a value that indicates whether two specified <see cref="CBool" /> structures are equal.
    /// </summary>
    /// <param name="left">The first <see cref="CBool" /> to compare.</param>
    /// <param name="right">The second <see cref="CBool" /> to compare.</param>
    /// <returns><c>true</c> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, <c>false</c>.</returns>
    public static bool Equals(CBool left, CBool right)
    {
        return left.Value == right.Value;
    }
}

/// <summary>
///     A value type with the same memory layout as a <see cref="byte" /> in a managed context and <c>char</c> in
///     an unmanaged context.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct CChar : IEquatable<byte>, IEquatable<CChar>
{
    /// <summary>
    ///     The value.
    /// </summary>
    public readonly byte Value;

    private CChar(byte value)
    {
        Value = Convert.ToByte(value);
    }

    /// <summary>
    ///     Converts the specified <see cref="byte" /> to a <see cref="CChar" />.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="CChar" />.</returns>
    public static implicit operator CChar(byte value)
    {
        return FromByte(value);
    }

    /// <summary>
    ///     Converts the specified <see cref="byte" /> to a <see cref="CChar" />.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="CChar" />.</returns>
    public static CChar FromByte(byte value)
    {
        return new CChar(value);
    }

    /// <summary>
    ///     Converts the specified <see cref="CChar" /> to a <see cref="byte" />.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="byte" />.</returns>
    public static implicit operator byte(CChar value)
    {
        return ToByte(value);
    }

    /// <summary>
    ///     Converts the specified <see cref="CChar" /> to a <see cref="byte" />.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="byte" />.</returns>
    public static byte ToByte(CChar value)
    {
        return value.Value;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is CChar value && Equals(value);
    }

    /// <inheritdoc />
    public bool Equals(byte other)
    {
        return Value == other;
    }

    /// <inheritdoc />
    public bool Equals(CChar other)
    {
        return Value == other.Value;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    /// <summary>
    ///     Returns a value that indicates whether two specified <see cref="CChar" /> structures are equal.
    /// </summary>
    /// <param name="left">The first <see cref="CChar" /> to compare.</param>
    /// <param name="right">The second <see cref="CChar" /> to compare.</param>
    /// <returns><c>true</c> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(CChar left, CChar right)
    {
        return left.Value == right.Value;
    }

    /// <summary>
    ///     Returns a value that indicates whether two specified <see cref="CChar" /> structures are not equal.
    /// </summary>
    /// <param name="left">The first <see cref="CChar" /> to compare.</param>
    /// <param name="right">The second <see cref="CChar" /> to compare.</param>
    /// <returns><c>true</c> if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(CChar left, CChar right)
    {
        return !(left == right);
    }

    /// <summary>
    ///     Returns a value that indicates whether two specified <see cref="CChar" /> structures are equal.
    /// </summary>
    /// <param name="left">The first <see cref="CChar" /> to compare.</param>
    /// <param name="right">The second <see cref="CChar" /> to compare.</param>
    /// <returns><c>true</c> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, <c>false</c>.</returns>
    public static bool Equals(CChar left, CChar right)
    {
        return left.Value == right.Value;
    }
}

/// <summary>
///     A pointer value type of bytes that represent a string; the C type `char*`.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly unsafe struct CString : IEquatable<CString>
{
    /// <summary>
    ///     The pointer.
    /// </summary>
    public readonly IntPtr Pointer;

    /// <summary>
    ///     Gets a value indicating whether this <see cref="CString" /> is a null pointer.
    /// </summary>
    public bool IsNull => Pointer == IntPtr.Zero;

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    /// <summary>
    ///     Initializes a new instance of the <see cref="CString" /> struct.
    /// </summary>
    /// <param name="value">The span.</param>
    public CString(ReadOnlySpan<byte> value)
    {
#pragma warning disable CS8500
        fixed (byte* pointer = value)
        {
            Pointer = (IntPtr)pointer;
        }
#pragma warning restore CS8500
    }
#endif

    /// <summary>
    ///     Initializes a new instance of the <see cref="CString" /> struct.
    /// </summary>
    /// <param name="value">The pointer value.</param>
    public CString(byte* value)
    {
        Pointer = (IntPtr)value;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CString" /> struct.
    /// </summary>
    /// <param name="value">The pointer value.</param>
    public CString(IntPtr value)
    {
        Pointer = value;
    }

    /// <summary>
    ///     Performs an explicit conversion from an <see cref="IntPtr" /> to a <see cref="CString" />.
    /// </summary>
    /// <param name="value">The pointer value.</param>
    /// <returns>
    ///     The resulting <see cref="CString" />.
    /// </returns>
    public static explicit operator CString(IntPtr value)
    {
        return FromIntPtr(value);
    }

    /// <summary>
    ///     Performs a conversion from an <see cref="IntPtr" /> to a <see cref="CString" />.
    /// </summary>
    /// <param name="value">The pointer value.</param>
    /// <returns>
    ///     The resulting <see cref="CString" />.
    /// </returns>
    public static CString FromIntPtr(IntPtr value)
    {
        return new CString(value);
    }

    /// <summary>
    ///     Performs an implicit conversion from a byte pointer to a <see cref="CString" />.
    /// </summary>
    /// <param name="value">The pointer value.</param>
    /// <returns>
    ///     The resulting <see cref="CString" />.
    /// </returns>
    public static implicit operator CString(byte* value)
    {
        return From(value);
    }

    /// <summary>
    ///     Performs an implicit conversion from a byte pointer to a <see cref="CString" />.
    /// </summary>
    /// <param name="value">The pointer value.</param>
    /// <returns>
    ///     The resulting <see cref="CString" />.
    /// </returns>
    public static CString From(byte* value)
    {
        return new CString((IntPtr)value);
    }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    /// <summary>
    ///     Performs an explicit conversion from a <see cref="ReadOnlySpan{T}" /> to a <see cref="CString" />.
    /// </summary>
    /// <param name="value">The pointer.</param>
    /// <returns>
    ///     The resulting <see cref="IntPtr" />.
    /// </returns>
    public static implicit operator CString(ReadOnlySpan<byte> value)
    {
        return new CString(value);
    }

    /// <summary>
    ///     Performs a conversion from a <see cref="ReadOnlySpan{T}" /> to a <see cref="CString" />.
    /// </summary>
    /// <param name="value">The pointer value.</param>
    /// <returns>
    ///     The resulting <see cref="CString" />.
    /// </returns>
    public static CString FromReadOnlySpan(ReadOnlySpan<byte> value)
    {
        return new CString(value);
    }
#endif

    /// <summary>
    ///     Performs an implicit conversion from a <see cref="CString" /> to a <see cref="IntPtr" />.
    /// </summary>
    /// <param name="value">The pointer.</param>
    /// <returns>
    ///     The resulting <see cref="IntPtr" />.
    /// </returns>
    public static implicit operator IntPtr(CString value)
    {
        return value.Pointer;
    }

    /// <summary>
    ///     Performs a conversion from a <see cref="CString" /> to a <see cref="IntPtr" />.
    /// </summary>
    /// <param name="value">The pointer.</param>
    /// <returns>
    ///     The resulting <see cref="IntPtr" />.
    /// </returns>
    public static IntPtr ToIntPtr(CString value)
    {
        return value.Pointer;
    }

    /// <summary>
    ///     Converts a C style string (ANSI or UTF-8) of type `char` (one dimensional byte array
    ///     terminated by a <c>0x0</c>) to a UTF-16 <see cref="string" /> by allocating managed memory and copying.
    /// </summary>
    /// <param name="value">A pointer to the C string.</param>
    /// <returns>A <see cref="string" /> equivalent of <paramref name="value" />.</returns>
    public static string ToString(CString value)
    {
        if (value.IsNull)
        {
            return string.Empty;
        }

        var end = (byte*)value.Pointer;
        while (*end != 0)
        {
            end++;
        }

        var result = new string(
            (sbyte*)value.Pointer,
            0,
            (int)(end - (byte*)value.Pointer),
            System.Text.Encoding.UTF8);

        return result;
    }

    /// <summary>
    ///     Converts a UTF-16 <see cref="string" /> to a C style string (one dimensional byte array terminated by a
    ///     <c>0x0</c>) by allocating native memory and copying.
    /// </summary>
    /// <param name="allocator">The <see cref="INativeAllocator" /> to use for allocating native memory.</param>
    /// <param name="str">The <see cref="string" />.</param>
    /// <returns>A C string pointer.</returns>
    public static CString FromString(INativeAllocator allocator, string str)
    {
        if (str == null)
        {
            return default;
        }

        var size = (str.Length * 4) + 1;
        var buffer = allocator.AllocateArray<byte>(size);
        fixed (char* stringPointer = str)
        {
            System.Text.Encoding.UTF8.GetBytes(stringPointer, str.Length + 1, buffer, size);
        }

        return new CString(buffer);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is CString value && Equals(value);
    }

    /// <inheritdoc />
    public bool Equals(CString other)
    {
        return Pointer == other.Pointer;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Pointer.GetHashCode();
    }

    /// <summary>
    ///     Returns a value that indicates whether two specified <see cref="CString" /> structures are equal.
    /// </summary>
    /// <param name="left">The first <see cref="CString" /> to compare.</param>
    /// <param name="right">The second <see cref="CString" /> to compare.</param>
    /// <returns><c>true</c> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(CString left, CString right)
    {
        return left.Pointer == right.Pointer;
    }

    /// <summary>
    ///     Returns a value that indicates whether two specified <see cref="CBool" /> structures are not equal.
    /// </summary>
    /// <param name="left">The first <see cref="CString" /> to compare.</param>
    /// <param name="right">The second <see cref="CString" /> to compare.</param>
    /// <returns><c>true</c> if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(CString left, CString right)
    {
        return !(left == right);
    }
}

/// <summary>
///     Utility methods for interoperability with C style strings in C#.
/// </summary>
public static unsafe class CStrings
{
    /// <summary>
    ///     Converts an array of strings to an array of C strings of type `char` (multi-dimensional array of one
    ///     dimensional byte arrays each terminated by a <c>0x0</c>) by allocating and copying.
    /// </summary>
    /// <remarks>
    ///     <para>Calls <see cref="CString" />.</para>
    /// </remarks>
    /// <param name="allocator">The <see cref="INativeAllocator" /> to use.</param>
    /// <param name="values">The strings.</param>
    /// <returns>An array pointer of C string pointers. You are responsible for freeing the returned pointer.</returns>
    public static CString* CStringArray(INativeAllocator allocator, string[] values)
    {
        if (allocator == null)
        {
            throw new ArgumentNullException(nameof(allocator));
        }

        var result = allocator.AllocateArray<CString>(values.Length);
        for (var i = 0; i < values.Length; ++i)
        {
            var @string = values[i];
            var cString = CString.FromString(allocator, @string);
            result[i] = cString;
        }

        return result;
    }
}

/// <summary>
///     Defines methods for allocating and freeing native memory.
/// </summary>
public interface INativeAllocator
{
    /// <summary>
    ///     Tries to allocate a block of memory.
    /// </summary>
    /// <param name="byteCount">The number of bytes to allocate.</param>
    /// <returns>If the memory was allocated successfully, a pointer to the allocated block of memory; otherwise, <c>null</c>.</returns>
    IntPtr Allocate(int byteCount);

    /// <summary>
    ///     Tries to free the specified block of memory previously allocated.
    /// </summary>
    /// <param name="pointer">The pointer to the block of memory.</param>
    void Free(IntPtr pointer);
}

public static unsafe class AllocatorExtensions
{
    public static T* AllocateSingle<T>(this INativeAllocator allocator)
        where T : unmanaged
    {
        var size = sizeof(T);
        return (T*)allocator.Allocate(size);
    }

    public static T* AllocateArray<T>(this INativeAllocator allocator, int elementCount)
        where T : unmanaged
    {
        var size = sizeof(T) * elementCount;
        return (T*)allocator.Allocate(size);
    }

    public static Span<T> AllocateSpan<T>(this INativeAllocator allocator, int elementCount)
        where T : unmanaged
    {
        var size = sizeof(T) * elementCount;
        return new Span<T>((void*)allocator.Allocate(size), elementCount);
    }

    public static CString AllocateCString(this INativeAllocator allocator, string str)
    {
        return CString.FromString(allocator, str);
    }
}

/// <summary>
///     An allocator that uses a single block of native memory which can be re-used.
/// </summary>
/// <remarks>
///     <p>
///         The <see cref="ArenaNativeAllocator" /> can be useful in native interoperability situations when you need to
///         temporarily allocate memory. For example, when calling native functions sometimes memory needs be available
///         for one or more calls but is no longer used after.
///     </p>
/// </remarks>
public sealed unsafe class ArenaNativeAllocator
    : INativeAllocator, IDisposable
{
    private IntPtr _buffer;

    /// <summary>
    ///     Gets the total byte count of the underlying block of native memory.
    /// </summary>
    public int Capacity { get; private set; }

    /// <summary>
    ///     Gets the used byte count of the underlying block of native memory.
    /// </summary>
    public int Used { get; private set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ArenaNativeAllocator" /> class.
    /// </summary>
    /// <param name="capacity">The number of bytes to allocate from native memory.</param>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="capacity" /> is negative or zero.</exception>
    /// <exception cref="OutOfMemoryException">Allocating <paramref name="capacity" /> of memory failed.</exception>
    public ArenaNativeAllocator(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }

#if NET6_0_OR_GREATER
        // malloc
        _buffer = (IntPtr)NativeMemory.Alloc((UIntPtr)capacity);
#else
        _buffer = Marshal.AllocHGlobal(capacity);
#endif
        Capacity = capacity;
    }

    /// <summary>
    ///     Frees the the underlying single block of native memory.
    /// </summary>
    public void Dispose()
    {
#if NET6_0_OR_GREATER
        NativeMemory.Free((void*)_buffer);
#else
        Marshal.FreeHGlobal(_buffer);
#endif
        _buffer = IntPtr.Zero;
        Capacity = 0;
    }

    /// <summary>
    ///     Uses the next immediate available specified number of bytes from the underlying block of native memory.
    /// </summary>
    /// <param name="byteCount">The number of bytes.</param>
    /// <returns>A pointer to the bytes.</returns>
    /// <exception cref="ObjectDisposedException">The underlying block of native memory is freed.</exception>
    /// <exception cref="InvalidOperationException">The underlying block of native memory is too small.</exception>
    public IntPtr Allocate(int byteCount)
    {
        if (_buffer == IntPtr.Zero)
        {
            throw new ObjectDisposedException(GetType().FullName);
        }

        if (Used + byteCount > Capacity)
        {
            throw new InvalidOperationException(
                $"Cannot allocate more than {Capacity} bytes with this instance of {nameof(ArenaNativeAllocator)}.");
        }

        var pointer = _buffer + Used;
        Used += byteCount;
        return pointer;
    }

    /// <summary>
    ///     Does nothing.
    /// </summary>
    /// <param name="pointer">The pointer to the bytes.</param>
    public void Free(IntPtr pointer)
    {
        // Do nothing
    }

    /// <summary>
    ///     Resets <see cref="Used" /> to zero and clears the entire underlying block of native memory with zeroes.
    /// </summary>
    /// <remarks>
    ///     <p>
    ///         Any pointers returned from <see cref="Allocate" /> before calling <see cref="Reset" /> must not be used or else
    ///         there is a risk that the pointers point to unexpected bytes of data.
    ///     </p>
    /// </remarks>
    public void Reset()
    {
        new Span<byte>((void*)_buffer, Used).Clear();
        Used = 0;
    }
}
