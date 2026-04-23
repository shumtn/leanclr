# LeanCLR

Language: [中文](./README.md) | [English](./README_EN.md)

[![GitHub](https://img.shields.io/badge/GitHub-Repository-181717?logo=github)](https://github.com/focus-creative-games/leanclr) [![Gitee](https://img.shields.io/badge/Gitee-Repository-C71D23?logo=gitee)](https://gitee.com/focus-creative-games/leanclr)

[![license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/focus-creative-games/leanclr/blob/main/LICENSE) [![DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/focus-creative-games/leanclr) [![Discord](https://img.shields.io/badge/Discord-Join-7289DA?logo=discord&logoColor=white)](https://discord.gg/esAYcM6RDQ)

LeanCLR is a lean, cross-platform implementation of the CLR (Common Language Runtime). It is designed for strong alignment with **ECMA-335** while remaining **compact**, **easy to embed**, and **low on memory**, targeting resource-constrained environments such as mobile clients, HTML5 runtimes, and mini-game platforms.

## Why LeanCLR?

Mature runtimes already exist (CoreCLR, Mono, IL2CPP). LeanCLR exists to address a different set of constraints:

1. **Shipping size** — CoreCLR, Mono, and IL2CPP often produce multi-megabyte binaries (sometimes tens of MB), which is a poor fit for mobile apps, H5, or mini-game runtimes.
2. **IL2CPP trade-offs** — IL2CPP is closed-source, AOT-only in practice, and does not offer especially complete ECMA-335 coverage.
3. **Maintainability and porting** — CoreCLR is very large; Mono’s architecture is dated. Both are difficult to customize or port to new platforms.

LeanCLR is built from the ground up around these goals:

- **Minimal footprint** — Single-threaded builds are under **600 KB** on x64/WebAssembly; aggressive trimming can reach **~300 KB**.
- **Strong portability** — Standard **C++17**, no platform-specific dependencies. **AOT + interpreter** hybrid execution (no JIT) keeps porting cost low across any toolchain that supports C++17.
- **Straightforward integration** — Simple build flow; add LeanCLR as a **CMakeLists.txt** module. The embedding surface is intentionally smaller than CoreCLR/Mono.
- **Maintainable codebase** — Clear structure for reading, customizing, and extending.

Typical use cases:

- Mobile games and applications (iOS/Android)
- H5 and mini-game platforms (e.g. WeChat, Douyin/TikTok mini games)
- Game clients that need hot-update friendly deployment models
- Embedded systems and IoT devices

## Features

### Standards and language surface

- **High ECMA-335 coverage** — Broad spec compliance; overall completeness is higher than `IL2CPP + HybridCLR`, and below Mono’s full surface area.
- **Modern C# essentials** — Generics, exceptions, reflection, delegates, LINQ-related patterns, and other core features are supported.
- **CoreCLR extensions** (planned) — e.g. static abstract members in interfaces (.NET 7+).
- **Intentionally trimmed** — Deprecated or rarely used features are dropped (e.g. `arglist`, `jmp`).

### Footprint and memory

- **Small binaries** — ~**600 KB** single-threaded; trimming the IR interpreter and non-essential icalls can shrink toward **300 KB** or lower.
- **Metadata-oriented memory design** — Metadata representations are optimized; method bodies can be reclaimed when no longer needed.
- **Alignment-aware allocation** — Separate pools tuned to real alignment requirements instead of paying a uniform 8-byte tax everywhere.
- **Compact headers** — Single-threaded builds use a one-pointer object header.

### Execution model

- **AOT + interpreter hybrid** — Balance cold-start, steady-state performance, and portability.
- **IL → C++ AOT** — IL bodies can be lowered to generated C++.
- **Dual interpreters** — IL interpreter for cold paths; IR interpreter for hot code, trading compile work for runtime speed.
- **Per-function AOT control** — Choose AOT only where it matters to keep overall binary size down.
- **Interpreter-backed exception paths** — Centralizing parts of exception machinery reduces generated AOT bulk.

### Portability

- **Portable C++17** — No hard dependency on OS-specific APIs inside the core runtime story.
- **No mandatory C++ exceptions** — Can be built with exceptions disabled (e.g. certain WASM/embedded configurations).
- **Broad target reach** — Windows, Linux, macOS, iOS, Android, WebAssembly, and other C++17-capable platforms.

## Documentation

The [docs](./docs) directory contains deeper material:

- [Documentation index](./docs/README.md)
- [Build docs overview](./docs/build/README.md)
- [Building the runtime](./docs/build/build_runtime.md)
- [Embedding LeanCLR](./docs/build/embed_leanclr.md)
- [AOT](./docs/aot.md)
- [Tests](./src/tests/README.md)

## Engine and platform integrations

LeanCLR’s **core runtime** is platform-agnostic. For convenience, some **pre-built integrations** exist:

| Platform | Status | Notes |
|----------|--------|-------|
| WeChat mini games / mini apps | Partial | [leanclr-sdk](https://github.com/focus-creative-games/leanclr-sdk) wraps a subset of APIs for C#-centric workflows. |
| **Unity & Tuanjie (WebGL / mini games)** | Available | [leanclr4unity](https://github.com/focus-creative-games/leanclr4unity) is a Unity package that replaces IL2CPP with LeanCLR when publishing games—not limited to WebGL or mini-game runtimes (e.g. Win64 and other targets). |
| **Unreal Engine (all platforms)** | In development | Release timeline TBD. |
| **Godot (all platforms)** | In development | Release timeline TBD. |

## Project status

### Components

| Module | Status | Notes |
|--------|--------|-------|
| **Metadata** | Done | PE/COFF and CLI metadata tables |
| **Type system** | Done | Classes, interfaces, generics, arrays, value types, … |
| **IR interpreter** | Done | Hot-path oriented execution |
| **Exceptions** | Done | try/catch/finally, nested cases |
| **Reflection** | Done | Type, MethodInfo, FieldInfo, … |
| **Delegates** | Done | Unicast / multicast, generic delegates |
| **Internal calls (icall)** | Done | Core profile icalls |
| **P/Invoke** | Done | Manual registration today; automation ties into the AOT toolchain |
| **GC** | In progress | Framework in place |
| **AOT compiler** | Done | IL → C++ pipeline |
| **Multithreading** | Planned | Threads, synchronization primitives, … |

### Stability

The tree is **production-grade stable** for the scenarios exercised in CI and partner testing:

- **Unity** — BCL matches Unity **2019.4.x – 6000.3.x LTS** IL2CPP expectations; **thousands** of tests pass.
- **Mono** — **99.95%** BCL compatibility on Mono **4.8**; **one** known failing test.

## Editions

LeanCLR ships in two conceptual editions: **Core** (available) and **Standard** (planned).

### Core edition

**Goal:** smallest binary and RAM footprint, maximum portability.

- Single-threaded execution model
- AOT + interpreter hybrid
- Precise, cooperative GC with accurate reference tracking
- No OS-specific icall layer — e.g. `System.IO.File` must be bridged or implemented in managed code
- Portable C++17

**Best for:** mobile, WASM, embedded/IoT, or any project that prioritizes deterministic footprint over full BCL surface.

### Standard edition (planned)

**Goal:** broader, production-style desktop/server feature set.

- Multithreading and synchronization primitives
- AOT + interpreter hybrid (same architectural idea)
- Conservative GC for wider compatibility
- Richer platform icalls (`System.IO`, `System.Net`, …)
- Still C++17-first, with a small set of explicit platform shims per OS

**Best for:** desktop apps, large mobile titles, anything that needs closer parity with “full” .NET base libraries.

## Demos

### leanclr-demo

[leanclr-demo](https://github.com/focus-creative-games/leanclr-demo) hosts two quickstarts:

| Sample | Description |
|--------|-------------|
| **win64** | Windows x64 — run `run.bat`. |
| **h5** | Browser WASM — serve `index.html` over HTTP. |

### leanclr4unity-demo

[leanclr4unity-demo](https://github.com/focus-creative-games/leanclr4unity_demo) demonstrates the **leanclr4unity** plugin: when publishing WebGL, mini-game, Win64, or other targets, swap IL2CPP for the LeanCLR runtime.

## Contact

- Email: `leanclr#code-philosophy.com` (replace `#` with `@`)
- Discord: <https://discord.gg/esAYcM6RDQ>
- QQ group: **1047250380**
