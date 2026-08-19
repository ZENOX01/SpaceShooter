using Godot;
using System;

public partial class GameOver : Node2D
{
	public override void _Ready()
	{
		var scoreLabel = GetNode<Label>("ScoreLabel");
		var bestLabel = GetNode<Label>("BestScoreLabel");
		scoreLabel.Text = "Score: " + Score.CurrentScore;
		bestLabel.Text = "Best: " + Score.BestScore;
	}

	public void _on_retry_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/game.tscn");
	}

	public void _on_main_menu_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/main_menu.tscn");
	}
}
