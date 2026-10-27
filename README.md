# Mimagemagick

A Mono GTK# frontend for ImageMagick.

## What it does

You choose an input file and an output file and set options, such as text with a
font picked from the fonts ImageMagick knows about. The tool builds and runs an
ImageMagick `convert` command and shows its output and errors.

## Tech

- C# with Mono and GTK#
- ImageMagick

## Building

Open the `.sln` file in MonoDevelop, or build it with Mono and run the resulting
`.exe` with `mono`. Requires GTK# 2.

## License

Licensed under the Apache License 2.0. See [LICENSE](LICENSE).
