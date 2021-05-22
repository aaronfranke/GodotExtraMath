using Godot;
using ExtraMath;

public class Test : Node
{
    public override void _Ready()
    {
        GD.Print(Basis25D.Isometric);
    }
}
