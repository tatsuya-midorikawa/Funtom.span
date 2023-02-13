namespace Funtom.span

open System
open System.Runtime.CompilerServices

[<Extension>]
type SpanExtensions =
  [<Extension>]
  static member inline GetSlice(span: ReadOnlySpan<'T>, start_idx, end_idx) =
    let s = defaultArg start_idx 0
    let e = defaultArg end_idx span.Length
    span.Slice(s, e - s)
      
  [<Extension>]
  static member inline GetSlice(span: Span<'T>, start_idx, end_idx) =
    let s = defaultArg start_idx 0
    let e = defaultArg end_idx span.Length
    span.Slice(s, e - s)