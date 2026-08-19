using Godot;
using System;

public partial class Game : Node2D
{
	public PackedScene BUlletScene = GD.Load<PackedScene>("res://scenes/bullets.tscn");
	public PackedScene AstroidScene = GD.Load<PackedScene>("res://scenes/astroid.tscn");
	[Export] public Node2D Bullets;
	[Export] public Node2D Astroids;
	[Export]public AudioStreamPlayer2D shootSound;
	private const float BaseAsteroidSpeed = 100f;
	private const float MaxSpawnDelay = 1.7f;
	private const float MinSpawnDelay = 0.5f;

	public override void _Ready()
	{
		Score.ResetRun();
		ApplyDifficulty();
	}

	public void _on_player_shoot_bullet(Vector2 pos, Vector2 direction)
	{
		var bullet = BUlletScene.Instantiate() as Bullets;
		Bullets.AddChild(bullet);
		bullet.GlobalPosition = pos;
		bullet.Direction = direction;
		shootSound.Play();
	}
	
	public void _on_astroid_timer_timeout()
	{
		var PosMarker = GetNode("astroidSpwan").GetChildren().PickRandom() as Node2D;
		var astroid = AstroidScene.Instantiate() as Astroid;
		Astroids.AddChild(astroid);
		astroid.GlobalPosition = PosMarker.GlobalPosition;
		astroid.Speed = (int)(BaseAsteroidSpeed + Score.CurrentScore * 8f);

		var rng = new RandomNumberGenerator();
		rng.Randomize();
		float angle = rng.RandfRange(0, Mathf.Tau);
		astroid.Direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
		ApplyDifficulty();
	}

	private void ApplyDifficulty()
	{
		var timer = GetNodeOrNull<Timer>("AstroidTimer");
		if (timer == null)
			return;

		float difficultyPush = MathF.Min(Score.CurrentScore * 0.04f, 1.2f);
		timer.WaitTime = Mathf.Max(MinSpawnDelay, MaxSpawnDelay - difficultyPush);
	}

	public override void _Process(double delta)
	{
		ApplyDifficulty();
	}
}
