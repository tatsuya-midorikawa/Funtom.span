namespace Funtom.span

#nowarn "9"

open System
open System.Runtime.InteropServices
open FSharp.NativeInterop

module Memory =
  let inline stackalloca<'a when 'a: unmanaged> size : Span<'a> =
    let p = NativePtr.stackalloc<'a> size |> NativePtr.toVoidPtr
    Span<'a>(p, size)

  let inline ref (x: Span<'a>) = MemoryMarshal.GetReference x
