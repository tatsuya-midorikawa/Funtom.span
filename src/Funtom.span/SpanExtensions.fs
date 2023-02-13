namespace Funtom.span

open System

[<AutoOpen>]
module SpanExtensions =
  type ReadOnlySpan<'T> with
    member __.GetSlice(start_idx, end_idx) =
      __.Slice(start_idx, end_idx)

  type Span<'T> with
    member __.GetSlice(start_idx, end_idx) =
      __.Slice(start_idx, end_idx)