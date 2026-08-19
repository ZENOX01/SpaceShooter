using Godot;
using System;

public partial class PauseMenu : Control
{
	[Export] AnimationPlayer animationPlayer;

	public override void _Ready()
	{
		Visible = false;
	}

	public void Resume()
	{
		GetTree().Paused = false;
		Visible = false;
		if (animationPlayer != null)
			animationPlayer.PlayBackwards("blur");
	}

	public void Paused()
	{
		GetTree().Paused = true;
		Visible = true;
		if (animationPlayer != null)
			animationPlayer.Play("blur");
	}

	public void Escape()
	{
		if (!Input.IsActionJustPressed("pause"))
			return;

		if (GetTree().Paused)
			Resume();
		else
			Paused();
	}

	public void _on_restart_pressed()
	{
		GetTree().ReloadCurrentScene();
	}

	public void _on_resume_pressed()
	{
		Resume();
	}

	public void _on_quit_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/main_menu.tscn");
	}

	public override void _Process(double delta)
	{
		Escape();
	}
}
