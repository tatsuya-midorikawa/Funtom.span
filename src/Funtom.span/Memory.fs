namespace Funtom.span

#nowarn "9"

open System
open System.Runtime.InteropServices
open FSharp.NativeInterop

[<Measure>] type byte

module Memory =
  let mutable threshold = 128<byte>

  let inline stackalloc<'a when 'a: unmanaged> (size: int<byte>) : nativeptr<'a> =
    NativePtr.stackalloc<'a> (int size)

  let inline stackalloca<'a when 'a: unmanaged> (size: int<byte>) : Span<'a> =
    let p = NativePtr.stackalloc<'a> (int size) |> NativePtr.toVoidPtr
    Span<'a>(p, (int size))

  let inline alloc<'a when 'a: unmanaged> (size: int<byte>) = 
    if size <= threshold then stackalloca size else Array.zeroCreate<'a>(int size).AsSpan()

  let inline ref (span: Span<'a>) = &(MemoryMarshal.GetReference span)
