# Starting to Work

## Conventions

The standard vocabulary for calling notes etc. follows the names on [Wikia](http://lowiro.wikia.com/wiki/Arcaea_Wiki).

The tips below are to help you understand the coding conventions in this repo.

For coding style, use:

- `_camelCaseWithAPrefixUnderscore` for private instance members (`private`, `private static`, and `private readonly`);
- `camelCase` for parameters and local variables;
- `PascalCase` for the rest, including `private static readonly`.

Visibility order: `public` -> `protected` -> `private`

## Coordinate Systems

Arcaea's beatmap coordinate system and Arcaea's coordinate system (in code): left hand, Y-up (OpenGL coordinate system).

```plain
     Y
  Z  |
    \--X
```

ArcaeaSim's coordinate system (in code): right hand, Z-up (Direct3D and mathematical coordinate system).

```plain
     Z
  Y  |
    \--X
```
