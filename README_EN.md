# LeanCLR

Language: [中文](./README.md) | [English](./README_EN.md)

[![GitHub](https://img.shields.io/badge/GitHub-Repository-181717?logo=github)](https://github.com/focus-creative-games/leanclr) [![Gitee](https://img.shields.io/badge/Gitee-Repository-C71D23?logo=gitee)](https://gitee.com/focus-creative-games/leanclr)

[![license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/focus-creative-games/leanclr/blob/main/LICENSE) [![DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/focus-creative-games/leanclr) [![Discord](https://img.shields.io/badge/Discord-Join-7289DA?logo=discord&logoColor=white)](https://discord.gg/esAYcM6RDQ)

LeanCLR is a cross-platform, lightweight implementation of the Common Language Runtime (CLR). Designed for high ECMA-335 compliance, LeanCLR delivers a compact, embeddable, and low-memory runtime, making it ideal for resource-constrained platforms such as mobile, H5, and mini-game environments.

## Why LeanCLR?

While mature CLR implementations like CoreCLR, Mono, and IL2CPP exist, LeanCLR addresses several key pain points:

1. CoreCLR, Mono, and IL2CPP produce large binaries (several MBs to tens of MBs), making them unsuitable for mobile apps, H5, or mini-game platforms.
2. IL2CPP is closed-source, supports only AOT, and has limited ECMA-335 compliance.
3. CoreCLR is complex and large; Mono’s codebase is dated and difficult to modify or port to new platforms.

LeanCLR is designed from scratch with the following core goals:

- **Ultra-compact** — Single-threaded builds are under **600 KB** on x64/WebAssembly, and can be trimmed to **300 KB**
- **Excellent cross-platform support** — Standard C++17 implementation with zero platform dependencies. Uses an AOT + Interpreter hybrid execution model (no JIT), ensuring seamless portability to any C++17-compliant platform.
- **Easy integration** — Simple build process; just include LeanCLR’s CMakeLists.txt. The CLR API is clear and far easier to integrate than CoreCLR or Mono.
- **Maintainability** — Clean, understandable code structure, easy to customize and extend.

LeanCLR is especially suitable for:

- Mobile games and apps (iOS/Android)
- H5 and mini-game platforms (WeChat Mini Games, TikTok Mini Games, etc.)
- Game clients requiring hot-update capabilities
- Embedded systems and IoT devices

## Features & Advantages

### Standards Compliance

- **High ECMA-335 compatibility** — Nearly complete implementation, more complete than `IL2CPP + HybridCLR`, slightly less than Mono.
- **Modern C# feature support** — Full support for generics, exception handling, reflection, delegates, LINQ, etc.
- **CoreCLR extensions** (planned) — e.g., static abstract interface methods (.NET 7+)
- **Lean design** — Only removes deprecated features (e.g., `arglist`, `jmp`)

### Ultra Lightweight

- **Tiny binary size** — Single-threaded builds ~**600 KB**; can be reduced to **300 KB** by trimming IR interpreter and non-essential icalls
- **Low memory usage** — Optimized metadata representation, with on-demand method body metadata reclamation
- **Fine-grained alignment** — Separate allocation pools for metadata/managed objects based on actual alignment needs, avoiding waste from uniform 8-byte alignment
- **Compact object header** — Single-threaded object header is just one pointer in size

### Efficient, Modern Execution Model

- **AOT + Interpreter hybrid** — Balances startup speed and runtime efficiency, with excellent cross-platform portability
- **Dual-interpreter architecture** — IL interpreter for cold paths, IR interpreter for hot functions, balancing compile overhead and performance
- **Function-level AOT** — Each managed function can be individually AOT-compiled; only performance-critical functions need AOT, minimizing binary size while maintaining performance
- **Exception fallback** — Exception handling is unified in the interpreter, greatly reducing AOT code size

### Cross-Platform Capability

- **Pure C++ implementation** — C++17 standard, zero platform-specific dependencies
- **No C++ exception dependency** — Can be built and run in environments with exceptions disabled
- **Zero porting cost** — Compiles directly to any C++17 platform (Windows, Linux, macOS, iOS, Android, WebAssembly, etc.)

## Documentation

See the [docs](./docs) directory for detailed documentation:

- [Documentation Overview](./docs/README.md)
- [Build Documentation](./docs/build/README.md)
- [Building the Runtime](./docs/build/build_runtime.md)
- [Embedding LeanCLR](./docs/build/embed_leanclr.md)
- [Test Framework](./src/tests/README.md)

## Integrated Engines & Platforms

LeanCLR is fully cross-platform. For developer convenience, we provide integrations for certain platforms:

| Platform | Status | Notes |
|---|---|---|
| WeChat Mini Games & Apps | Partial | [WeChat SDK](https://github.com/focus-creative-games/leanclr-sdk) available; some APIs wrapped for pure C# development |
| **Unity & Unity WebGL/Mini Game** | In development | ETA: March 2026 |
| **Unreal Engine (all platforms)** | In development | Release date TBD |
| **Godot (all platforms)** | In development | Release date TBD |

## Project Status

### Current Progress

| Module | Status | Notes |
|------|------|------|
| **Metadata Parsing** | ✅ Complete | Full PE/COFF and CLI metadata support |
| **Type System** | ✅ Complete | Classes, interfaces, generics, arrays, value types, etc. |
| **IR Interpreter** | ✅ Complete | Optimized execution for hot functions |
| **Exception Handling** | ✅ Complete | try/catch/finally, nested exceptions, etc. |
| **Reflection** | ✅ Complete | Type, MethodInfo, FieldInfo, etc. |
| **Delegates** | ✅ Complete | Unicast/multicast, generic delegates |
| **Internal Calls** | ✅ Complete | Core version icalls only |
| **P/Invoke** | ✅ Complete | Manual registration supported; automation depends on AOT compiler |
| **Garbage Collection** | 📋 In development | Basic framework ready |
| **AOT Compiler** | 📋 In development | IL → C++ transpilation |
| **Multi-threading** | 📋 Planned | Threads, synchronization primitives, etc. |

### Stability

The current version is **very stable**:

- Fully compatible with Unity 2019.4.x – 6000.3.x LTS IL2CPP BCL; passes all (thousands of) test cases
- 99.95% compatible with Mono 4.8 BCL; only one test case fails

## Editions

LeanCLR provides **Core** and **Standard** editions for different scenarios.

### Core Edition

Status: **Complete**

**Design Goal**: Minimize binary size and memory usage; maximize cross-platform capability

- **Single-threaded execution** — Simplified memory model, no thread synchronization
- **AOT + Interpreter hybrid** — Flexible execution
- **Precise, cooperative GC** — Accurate managed reference tracking, efficient memory reclamation
- **Zero platform dependencies** — No OS/platform-specific functions
- **Standard C++17** — Compiles and runs on any C++17 platform
- **No platform icalls** — e.g., `System.IO.File` must be bridged or implemented in managed code

**Best for**: Mobile apps/games, WebAssembly, embedded/IoT, projects needing maximum cross-platform consistency

### Standard Edition

Status: **Planned**

**Design Goal**: Feature-complete, production-grade runtime

- **Multi-threading support** — Full threading model and synchronization primitives
- **AOT + Interpreter hybrid** — Flexible execution
- **Conservative GC** — Better compatibility for complex apps
- **Full platform icalls** — Implements `System.IO`, `System.Net`, etc.
- **Standard C++17** — Some platform-specific interfaces must be adapted when porting

**Best for**: Desktop apps, large mobile apps/games, projects needing full .NET base library

## Demo

The [leanclr-demo](https://github.com/focus-creative-games/leanclr-demo) repository provides two platform demos for quick evaluation:

| Demo | Description |
|------|------|
| **win64** | Windows x64 demo; run `run.bat` |
| **h5** | WebAssembly browser demo; open `index.html` via HTTP server |

## Contact

- Email: leanclr#code-philosophy.com
- Discord: <https://discord.gg/esAYcM6RDQ>
- QQ Group: 1047250380

### Standard Edition

**Design Goal**: Feature-complete production-grade runtime

- **Multi-Threading Support** — Complete threading model and synchronization primitives
- **AOT + Interpreter Hybrid Execution** — Flexible execution strategy
- **Conservative GC** — Better compatibility, suitable for complex application scenarios
- **Complete Platform icalls** — Implements `System.IO`, `System.Net`, and other platform-specific functionality
- **Standard C++17 Implementation** — Requires adaptation of a few platform-specific interfaces when porting to new platforms

**Best Use Cases**: Desktop applications, mobile games, projects requiring full .NET base library functionality

## Project Status

### Current Progress

| Module | Status | Description |
|--------|--------|-------------|
| **Metadata Parsing** | ✅ Complete | Full support for PE/COFF format and CLI metadata tables |
| **Type System** | ✅ Complete | Classes, interfaces, generics, arrays, value types, etc. |
| **IL Interpreter** | 🔶 In Development | Covers almost all ECMA-335 IL instructions |
| **IR Interpreter** | ✅ Complete | Optimized execution for hot functions |
| **Exception Handling** | ✅ Complete | try/catch/finally, nested exceptions, etc. |
| **Reflection** | ✅ Complete | Type, MethodInfo, FieldInfo, and other core APIs |
| **Delegates** | ✅ Complete | Unicast/multicast delegates, generic delegates |
| **Internal Calls** | 🔶 In Progress | Core icalls implemented, platform icalls being added |
| **Garbage Collection** | 🔶 In Development | Basic framework ready |
| **AOT Compiler** | 📋 Planned | IL → C++ transpilation |
| **P/Invoke** | � Partial | Manual registration supported, automation depends on AOT compiler |
| **Multi-Threading** | 📋 Planned | Threads, synchronization primitives, etc. |

### ECMA-335 Compatibility

- Completeness exceeds `IL2CPP + HybridCLR` combination
- Slightly lower completeness than Mono (main gap is platform-specific icalls)
- CoreCLR extension features (such as static abstract interface methods) will be implemented in future versions

### Stability

The current version has achieved a **very high** level of stability.

- Fully compatible with Unity 2019.4.x - 6000.3.x LTS IL2CPP's BCL, passing all thousands of test cases.
- 99.95% compatible with Mono 4.8's BCL, with only one test case failing.

### Roadmap

**Near-Term Goals:**

- Complete garbage collector implementation
- Implement AOT compiler (IL → C++)
- Complete P/Invoke automation support (depends on AOT compiler)
- Support CoreCLR extension features
- Provide more complete examples and documentation

**Mid-Term Goals:**

- Add more platform-specific internal calls (such as `System.IO`)
- Multi-threading support

**Long-Term Goals:**

- Continuous performance optimization
- Broader platform support

## Project Structure

For detailed project structure documentation, see [Project Structure](./docs/project_structure.md).

```
leanclr/
├── src/
│   ├── runtime/      # LeanCLR runtime core
│   ├── libraries/    # Base class libraries
│   ├── tools/        # Command-line tools
│   ├── samples/      # Sample projects
│   └── tests/        # Unit tests
├── docs/             # Documentation
└── tools/            # Build utilities
```

## Documentation

Detailed documentation is available in the [docs](./docs) directory:

- [Documentation Overview](./docs/README.md) - Documentation structure and navigation
- [Build Documentation](./docs/build/README.md) - Build-related documentation overview
- [Building the Runtime](./docs/build/build_runtime.md) - How to build the LeanCLR runtime
- [Embedding LeanCLR](./docs/build/embed_leanclr.md) - How to integrate LeanCLR into your project
- [Test Framework](./src/tests/README.md) - Unit test framework and how to write test cases

## Quick Build

### Windows (Visual Studio)

```cmd
cd src/runtime
build.bat Release
```

### WebAssembly

```cmd
# 1. Prepare Emscripten SDK environment
emsdk_env.bat

# 2. Build
cd src/samples/lean-wasm
build-wasm.bat
```

For more details, see [Build Documentation](./docs/build/build_runtime.md).

## Demo

Repository [leanclr-demo](https://github.com/focus-creative-games/leanclr-demo) provides two platform demos for quickly experiencing LeanCLR's capabilities:

| Demo | Description |
|------|-------------|
| **win64** | Windows x64 platform demo, run `run.bat` to execute |
| **h5** | WebAssembly browser demo, access `index.html` through an HTTP server |

## Contact

- Email: leanclr#code-philosophy.com
- Discord: <https://discord.gg/esAYcM6RDQ>
- QQ Group: 1047250380
