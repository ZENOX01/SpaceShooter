using Godot;
using System;
using System.Threading.Tasks;

public partial class Astroid : Area2D
{
	public int Speed = 100;
	public Vector2 Direction;
	[Export]public AnimationPlayer AnimationPlayer;
	[Export]public AudioStreamPlayer2D EXPSound;
	
	
	private Texture2D[] colors = new Texture2D[]
	{
		GD.Load<Texture2D>("res://SpaceShip Game Assets/Metior.png"),
		GD.Load<Texture2D>("res://SpaceShip Game Assets/Metior2.png")
	};

	public async void _on_area_entered(Area2D area)
	{
		Direction = Vector2.Zero;
		area.CallDeferred(Node.MethodName.QueueFree);
		EXPSound.Play();
		await Explosion();
		GetTree().CallGroup("Score","UpdateScore");
	}
	
	public async void _on_body_entered(CharacterBody2D body)
	{
		body.CallDeferred(Node.MethodName.QueueFree);
		await Explosion();
		GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToFile, "res://scenes/game_over.tscn");
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var asteroidSprite = GetNode<Sprite2D>("Sprite2D");
		var RandomScale = GD.RandRange(1f,2f);
		Scale = new Vector2((float)RandomScale, (float)RandomScale);
		asteroidSprite.Texture = colors[new System.Random().Next(colors.Length)];
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position += Direction * Speed * (float)delta;
		Rotate(1 * (float)delta);

		var screenRect = GetViewportRect();
		if (!screenRect.Grow(120).HasPoint(GlobalPosition))
		{
			QueueFree();
		}
	}
	
	public async Task Explosion()
	{
		var asteroidSprite = GetNode<Sprite2D>("Sprite2D");
		var explosion = GetNode<Sprite2D>("Explosion");
		var collision = GetNode<CollisionShape2D>("CollisionShape2D");

		asteroidSprite.Hide();
		collision.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		explosion.Show();
		AnimationPlayer.Play("Explode");
		await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);

		QueueFree();
	}
}
