using Godot;
using System;

public partial class Score : Label
{
	public static int CurrentScore { get; private set; }
	public static int BestScore { get; private set; }
	public int currentScore;

	public override void _Ready()
	{
		ResetRun();
	}

	public override void _Process(double delta)
	{
	}

	public static void ResetRun()
	{
		CurrentScore = 0;
	}

	public void UpdateScore()
	{
		currentScore += 1;
		CurrentScore = currentScore;
		BestScore = Math.Max(BestScore, currentScore);
		Text = "Score: " + currentScore;
	}
}
