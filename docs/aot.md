# LeanCLR AOT Workflow for .NET DLLs

This document explains how to use LeanCLR's AOT pipeline to translate managed DLL assemblies into C++ source files, integrate those files into your native build, and register generated AOT modules during runtime startup.

## 1) LeanAOT Tool

### What LeanAOT Does

`LeanAOT` is a command-line tool that performs Ahead-of-Time (AOT) conversion from managed assemblies to C++ code.

- Input: one or more managed assemblies (for example `mscorlib.dll`, app DLLs)
- Output: generated C++ source files for method bodies, invokers, and module registration
- Typical use case: build native executable/runtime with pre-generated managed code metadata and stubs

Generated files usually include patterns such as:

- `*.method_body_partN.cpp`
- `*.module_registration.cpp`
- `*.module_registration.h`
- `modules_registration.cpp`

### How to Build LeanAOT

From repository root:

```bat
src\leanaot\publish.bat
```

This publishes a Release build of `LeanAOT` to:

`src/tools/leanaot`

### Command-Line Parameters

`LeanAOT` currently requires all of the following options:

- `-d <path>`
	- DLL search path
	- can be specified multiple times
- `-a <assembly>` or `--assembly <assembly>`
	- assembly name to AOT (without `.dll` extension)
	- can be specified multiple times
- `-o <dir>`
	- output directory for generated C++ code

Example:

```bat
LeanAOT ^
	-d libraries\dotnetframework4.x ^
	-d leanaot\Test\bin\Debug ^
	-a mscorlib ^
	-a Test ^
	-o samples\simple-aot\cpp
```

---

## 2) How Generated C++ Code Participates in Build

After running `LeanAOT`, add the generated C++ files to your CMake target.

Recommended pattern (same idea as `samples/simple-aot`):

```cmake
file(GLOB CPP_SOURCES "cpp/*.cpp")
add_executable(simple-aot main.cpp ${CPP_SOURCES})
target_link_libraries(simple-aot PRIVATE leanclr)
```

Notes:

- Keep generated files in a dedicated folder (for example `cpp/`)
- Re-run `LeanAOT` when managed DLLs change
- Ensure `modules_registration.cpp` is included in the same target (it defines global module data used by runtime)

---

## 3) Runtime Initialization with AOT Modules

Before runtime startup, register generated AOT modules by wiring the exported global data into `vm::Settings`.

### Step 1: Declare Generated Modules Data

```cpp
extern leanclr::metadata::RtAotModulesData g_aot_modules_data;
```

### Step 2: Set AOT Modules Before Runtime Initialization

```cpp
leanclr::vm::Settings::set_aot_modules_data(&g_aot_modules_data);
```

This call must happen before code paths that initialize or execute managed runtime logic.

Minimal startup sequence:

```cpp
vm::Settings::set_file_loader(assembly_file_loader);
vm::Settings::set_aot_modules_data(&g_aot_modules_data);
// initialize runtime / execute managed entry
```

---

## Reference Example in Repository

For a complete working flow, see:

- `src/samples/simple-aot/CMakeLists.txt` (adds generated `cpp/*.cpp` into target)
- `src/samples/simple-aot/main.cpp` (declares and registers `g_aot_modules_data`)

