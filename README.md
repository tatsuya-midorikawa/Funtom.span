# Funtom.span

![img](https://raw.githubusercontent.com/tatsuya-midorikawa/Funtom.span/main/assets/phantom.png)

## 🔷 What's this?
Funtom.span is a library that provides functions related to memory allocation.

## 🔷 How to use?

### 1️⃣ Memory module

#### `Memory.stackalloc<'a> size`: int<byte> -> nativeptr<'a>

Allocates a region of memory on the stack.

```fsharp
let span: nativeptr<char> = Memory.stackalloc<char> 256<byte>
```

#### `Memory.stackalloca<'a> size`: int<byte> -> Span<'a>

Allocates a region of memory on the stack and returns value as Span<'a>.

```fsharp
let span: Span<char> = Memory.stackalloca<char> 256<byte>
```

#### `Memory.ref<'a> span`: Span<'a> -> byref<'a>

Returns a reference to the element of the span at index 0.

```fsharp
let span: Span<char> = Memory.stackalloca<char> 256<byte>
let r: byref<char> = Memory.ref span
```

### 2️⃣ SpanExtensions

This extension to allow Span<'a> to be F# sliced.

```fsharp
let span: Span<char> = Memory.stackalloca<char> 256<byte>
let sliced: Span<char> = span[..128]
```