# GodotExtraMath

Check out this addon on the Godot Asset Library: https://godotengine.org/asset-library/asset/408

Library for extra math types in C# for the Godot game engine.

Based on Godot's own C# math types, which can be found here: https://github.com/godotengine/godot/commits/master/modules/mono/glue/GodotSharp/GodotSharp/Core

This library adds the following:

* `Vector4` type for various purposes.

* Integer vector types `Vector2i`, `Vector3i`, `Vector4i`, `Rect2i`, and `AABBi`.

* 2.5D types `Basis25D` and `Transform25D` for 2.5D games.

* Double-precision versions of all floating-point types.

* `Mathd` static class that mirrors `Mathf` but with doubles.

More things may be added in the future.

To use this addon, copy the addon into a 3.3+ Godot project with C#, and enable the plugin. Then in your C# scripts, you can add `using ExtraMath;` to use these structures (or just write `ExtraMath.Vector4` etc).
