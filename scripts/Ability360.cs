using Godot;
using System;

public partial class Ability360 : Area2D
{
	[Signal] delegate void PickedUpEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += _on_body_entered;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_body_entered(Node2D body)
	{
		if (body is Player player)
		{
			player.Unlock360Shot();
			EmitSignal(SignalName.PickedUp);
			QueueFree();
		}
	}
}
