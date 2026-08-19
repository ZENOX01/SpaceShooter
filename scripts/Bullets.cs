using Godot;
using System;

public partial class Bullets : Area2D
{
	public int Speed = 500;
	public Vector2 Direction;

	

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		Position += Direction * Speed * (float)delta;
		

		var viewRect = GetViewportRect();
		if (!viewRect.Grow(80).HasPoint(GlobalPosition))
		{
			QueueFree();
		}
	}
}
