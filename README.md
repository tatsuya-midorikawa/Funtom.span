# Funtom.span

![img](https://raw.githubusercontent.com/tatsuya-midorikawa/Funtom.span/main/assets/phantom.png)

## 🔷 What's this?
Funtom.span is a library that provides functions related to memory allocation.

## 🔷 How to use?

### 1️⃣ Memory module

#### `Memory.stackalloc<'a> size`: int -> nativeptr<'a>

Allocates a region of memory on the stack.

```fsharp
let span: nativeptr<char> = Memory.stackalloc<char> 256
```

#### `Memory.stackalloca<'a> size`: int -> Span<'a>

Allocates a region of memory on the stack and returns value as Span<'a>.

```fsharp
let span: Span<char> = Memory.stackalloca<char> 256
```

#### `Memory.alloc<'a> size`: int -> Span<'a>

Allocates a memory area on the stack or heap according to the specified size and returns the value as Span<'a>.
The threshold can be changed under Memory.threshold.

```fsharp
Memory.threshold <- 256
let span: Span<char> = Memory.alloc<char> 256
```

#### `Memory.ref<'a> span`: Span<'a> -> byref<'a>

Returns a reference to the element of the span at index 0.

```fsharp
let span: Span<char> = Memory.stackalloca<char> 256
let r: byref<char> = Memory.ref span
```

#### `Memory.rent<'a> size`: int -> array<'a>

Retrieves a buffer that is at least the requested length.

```fsharp
let buf: array<char> = Memory.rent<char> 256
```

#### `Memory.returns<'a> buffer`: array<'a> -> unit

Returns an array to the pool that was previously obtained using the Memory.rent<'a> method on the same System.Buffers.ArrayPool instance.

```fsharp
let buf: array<char> = Memory.rent<char> 256
Memory.returns buf
```

### 2️⃣ SpanExtensions

This extension to allow Span<'a> to be F# sliced.

```fsharp
let span: Span<char> = Memory.stackalloca<char> 256
let sliced: Span<char> = span[..128]
```

### ⏱️Performance

code:
```fsharp
#nowarn "9"

open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open System
open System.Text
open System.Runtime.InteropServices
open Microsoft.FSharp.NativeInterop
open Funtom.span

module Win32Api =
  [<DllImport("kernel32.dll", EntryPoint = "GetComputerNameW", CharSet = CharSet.Unicode)>]
  extern bool GetComputerNameW(StringBuilder lpBuffer, uint& lpnSize)
  [<DllImport("kernel32.dll", EntryPoint = "GetComputerNameW", CharSet = CharSet.Unicode)>]
  extern bool GetComputerNameZ(char& lpBuffer, uint& lpnSize)

type Benchmarks() =
  [<Benchmark>]
  member __.GetComputerNameW() =
    let mutable size = 256u
    let buf = StringBuilder(int size)
    Win32Api.GetComputerNameW(buf, &size) |> ignore
    buf.ToString(0, int size)

  [<Benchmark>]
  member __.GetComputerNameZ_Normal() =
    let buf =
      let p = NativePtr.stackalloc<char>(128) |> NativePtr.toVoidPtr
      Span<char>(p, 128)
    let mutable size = uint buf.Length;
    Win32Api.GetComputerNameZ(&(MemoryMarshal.GetReference(buf)), &size) |> ignore
    buf.Slice(0, int size).ToString()

  [<Benchmark>]
  member __.GetComputerNameZ_Funtom_span() =
    let buf = Memory.stackalloca<char> 128
    let mutable size = uint buf.Length;
    Win32Api.GetComputerNameZ(&(Memory.ref buf), &size) |> ignore
    buf[..int size].ToString()

BenchmarkRunner.Run<Benchmarks>() |> ignore
```

result:
```
// * Summary *

BenchmarkDotNet=v0.13.4, OS=Windows 11 (10.0.22621.1344)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.201
  [Host]     : .NET 7.0.3 (7.0.323.6910), X64 RyuJIT AVX2 DEBUG [AttachedDebugger]
  DefaultJob : .NET 7.0.3 (7.0.323.6910), X64 RyuJIT AVX2


|                       Method |      Mean |    Error |   StdDev |
|----------------------------- |----------:|---------:|---------:|
|             GetComputerNameW | 139.64 ns | 2.205 ns | 2.540 ns |
|      GetComputerNameZ_Normal |  96.04 ns | 1.972 ns | 2.025 ns |
| GetComputerNameZ_Funtom_span |  97.92 ns | 0.882 ns | 0.782 ns |
```