# GodotExtraMath

Library for extra math types in C# for the Godot game engine.

Based on Godot's own C# math types, which can be found here: https://github.com/godotengine/godot/tree/master/modules/mono/glue/Managed/Files

This library adds the following:

-   `Vector4` type for various purposes.

-   Integer vector types `Vector2i`, `Vector3i`, and `Vector4i`.

-   2.5D types `Basis25D` and `Transform25D` for 2.5D games.

-   Double-precision versions of all floating-point types.

-   `Mathd` static class that mirrors `Mathf` but with doubles.

More things may be added in the future.

To use this library, download it and place in the `addons` folder, call the folder `extra-math-cs`. Alternatively, add this repository as a submodule. Then add the following lines to your `csproj` file:

    <Compile Include="addons\extra-math-cs\ExtraMath\Basis25D.cs" />
    <Compile Include="addons\extra-math-cs\ExtraMath\Double\AABBd.cs" />
    <Compile Include="addons\extra-math-cs\ExtraMath\Double\Basis25Dd.cs" />
    <Compile Include="addons\extra-math-cs\ExtraMath\Double\Basisd.cs" />
    <Compile Include="addons\extra-math-cs\ExtraMath\Double\Mathd.cs" />
    <Compile Include="addons\extra-math-cs\ExtraMath\Double\MathdEx.cs" />
    <Compile Include="addons\extra-math-cs\ExtraMath\Double\Planed.cs" />
    <Compile Include="addons\extra-math-cs\ExtraMath\Double\Quatd.cs" />
    <Compile Include="addons\extra-math-cs\ExtraMath\Double\Rect2d.cs" />
    <Compile Include="addons\extra-math-cs\ExtraMath\Double\Transform2Dd.cs" />
    <Compile Include="addons\extra-math-cs\ExtraMath\Double\Transform25Dd.cs" />
    <Compile Include="addons\extra-math-cs\ExtraMath\Double\Transformd.cs" />
    <Compile Include="addons\extra-math-cs\ExtraMath\Double\Vector2d.cs" />
    <Compile Include="addons\extra-math-cs\ExtraMath\Double\Vector3d.cs" />
    <Compile Include="addons\extra-math-cs\ExtraMath\Double\Vector4d.cs" />
    <Compile Include="addons\extra-math-cs\ExtraMath\Integer\AABBi.cs" />
    <Compile Include="addons\extra-math-cs\ExtraMath\Integer\Rect2i.cs" />
    <Compile Include="addons\extra-math-cs\ExtraMath\Integer\Vector2i.cs" />
    <Compile Include="addons\extra-math-cs\ExtraMath\Integer\Vector3i.cs" />
    <Compile Include="addons\extra-math-cs\ExtraMath\Integer\Vector4i.cs" />
    <Compile Include="addons\extra-math-cs\ExtraMath\Transform25D.cs" />
    <Compile Include="addons\extra-math-cs\ExtraMath\Vector4.cs" />
