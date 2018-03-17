# Building ArcaeaSim

**Requirements:**

- OS: Windows
- Compiler and Toolchain:
  - Visual Studio 2017 Community
  - .NET Framework 4.5 Toolchain
- [NuGet CLI](https://www.nuget.org/downloads) (≥ 4.3.0)

> **Remember** to [update your NuGet version](https://docs.microsoft.com/en-us/nuget/guides/install-nuget) before building.
> Otherwise you are very likely to see errors like "Too many projects specified".

**Step 1**: Clone this repo:

```bash
git clone https://github.com/hozuki/ArcaeaSim.git
cd ArcaeaSim
git submodule update --init --recursive
```

**Step 2**: Install [MonoGame](http://www.monogame.net/downloads/) (version 3.6 or later).

**Step 3**: Build the solution in Visual Studio.

If you encounter errors like "assembly not found", try to run `nuget restore SOLUTION_NAME.sln` in every solution directory.
The automatic NuGet restore in Visual Studio does not work.

**Step 4**: A little cleaning. In the directory containing built binaries, replace the `libSkiaSharp.dll` with the DLL with the same name
under `x86` directory.

The default `libSkiaSharp.dll` in the binaries directory is a 64-bit DLL, but MonoGame WindowsDX build only produces a 32-bit executable by default,
so the executable cannot load the default DLL.
