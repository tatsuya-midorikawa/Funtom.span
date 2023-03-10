namespace Funtom.span

#nowarn "9"

open System
open System.Buffers
open System.Runtime.InteropServices
open FSharp.NativeInterop

module Memory =
  let mutable threshold = 128

  let inline stackalloc<'a when 'a: unmanaged> (size: int) : nativeptr<'a> =
    NativePtr.stackalloc<'a> size

  let inline stackalloca<'a when 'a: unmanaged> (size: int) : Span<'a> =
    let p = NativePtr.stackalloc<'a> (int size) |> NativePtr.toVoidPtr
    Span<'a>(p, size)

  let inline alloc<'a when 'a: unmanaged> (size: int) = 
    if size <= threshold then stackalloca size else Array.zeroCreate<'a>(size).AsSpan()
    
  let inline rent<'a when 'a: unmanaged> (size: int) =
    ArrayPool<'a>.Shared.Rent size
    
  let inline returns<'a when 'a: unmanaged> (ary: array<'a>) =
    ArrayPool<'a>.Shared.Return ary

  let inline ref (span: Span<'a>) = &(MemoryMarshal.GetReference span)
