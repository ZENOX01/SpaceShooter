using Godot;
using System;

public partial class Game : Node2D
{
	public PackedScene BUlletScene = GD.Load<PackedScene>("res://scenes/bullets.tscn");
	public PackedScene AstroidScene = GD.Load<PackedScene>("res://scenes/astroid.tscn");

	public PackedScene PickupScene = GD.Load<PackedScene>("res://scenes/ability360.tscn");
	public PackedScene SelfBlastScene = GD.Load<PackedScene>("res://scenes/self_blast.tscn");
	[Export] public Node2D Bullets;
	[Export] public Node2D Astroids;
	[Export] public AudioStreamPlayer2D shootSound;
	private const float BaseAsteroidSpeed = 100f;
	private const float BaseSelfBlastSpeed = 160f;
	private const float MaxSpawnDelay = 1.7f;
	private const float MinSpawnDelay = 0.5f;
	private const float MaxSelfBlastDelay = 5.5f;
	private const float MinSelfBlastDelay = 2.2f;

	public override void _Ready()
	{
		Score.ResetRun();
		ApplyDifficulty();
		ApplyEnemyDifficulty();
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
		SpawnAstroid();
		ApplyDifficulty();
	}

	public void _on_self_blast_timer_timeout()
	{
		if (Score.CurrentScore <= 10)
			return;

		SpawnSelfBlast();
		ApplyEnemyDifficulty();
	}

	public void _on_pickup_timer_timeout()
	{
		var pickup = PickupScene.Instantiate<Area2D>();
		AddChild(pickup);

		var viewport = GetViewportRect();
		pickup.GlobalPosition = new Vector2(
			(float)GD.RandRange(50, viewport.Size.X - 50),
			(float)GD.RandRange(50, viewport.Size.Y - 50));
	}

	private void SpawnAstroid()
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
	}

	private void SpawnSelfBlast()
	{
		var player = GetNodeOrNull<Player>("Player");
		if (player == null)
			return;

		var selfBlast = SelfBlastScene.Instantiate() as SelfBlast;
		if (selfBlast == null)
			return;

		var holder = GetNodeOrNull<Node2D>("SelfBlasts");
		if (holder == null)
			holder = Astroids;

		holder.AddChild(selfBlast);

		var viewport = GetViewportRect();
		var rng = new RandomNumberGenerator();
		rng.Randomize();

		Vector2 spawnPosition;
		for (int i = 0; i < 30; i++)
		{
			spawnPosition = new Vector2(
				rng.RandfRange(0, viewport.Size.X),
				rng.RandfRange(0, viewport.Size.Y));

			if ((spawnPosition - player.GlobalPosition).Length() > 350f)
			{
				selfBlast.GlobalPosition = spawnPosition;
				selfBlast.Speed = BaseSelfBlastSpeed + Score.CurrentScore * 4f;
				return;
			}
		}

		selfBlast.GlobalPosition = new Vector2(
			rng.RandfRange(0, viewport.Size.X),
			rng.RandfRange(0, viewport.Size.Y));
		selfBlast.Speed = BaseSelfBlastSpeed + Score.CurrentScore * 4f;
	}

	private void ApplyDifficulty()
	{
		var timer = GetNodeOrNull<Timer>("AstroidTimer");
		if (timer == null)
			return;

		float difficultyPush = MathF.Min(Score.CurrentScore * 0.04f, 1.2f);
		timer.WaitTime = Mathf.Max(MinSpawnDelay, MaxSpawnDelay - difficultyPush);
	}

	private void ApplyEnemyDifficulty()
	{
		var timer = GetNodeOrNull<Timer>("SelfBlastTimer");
		if (timer == null)
			return;

		if (Score.CurrentScore < 10)
		{
			if (!timer.IsStopped())
				timer.Stop();
			return;
		}

		float difficultyPush = MathF.Min((Score.CurrentScore - 10) * 0.08f, 2.0f);
		timer.WaitTime = Mathf.Max(MinSelfBlastDelay, MaxSelfBlastDelay - difficultyPush);
		if (timer.IsStopped())
			timer.Start();
	}

	public override void _Process(double delta)
	{
		ApplyDifficulty();
		ApplyEnemyDifficulty();
	}
}
