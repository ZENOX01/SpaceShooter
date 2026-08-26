using Godot;
using System.Threading.Tasks;

public partial class SelfBlast : CharacterBody2D
{
	[Export] public float Speed = 200f;
	[Export] public float HitDistance = 30f;
	[Export] public AnimationPlayer AnimationPlayer;
	[Export] public AudioStreamPlayer2D EXPSound;

	private CharacterBody2D targetPlayer;
	private bool isExploding;

	public override void _PhysicsProcess(double delta)
	{
		if (isExploding)
			return;

		if (targetPlayer == null || !IsInstanceValid(targetPlayer))
		{
			targetPlayer = null;
			return;
		}

		Vector2 toTarget = targetPlayer.GlobalPosition - GlobalPosition;
		if (toTarget.LengthSquared() < HitDistance * HitDistance)
		{
			ExplodeAndGameOver();
			return;
		}

		Vector2 direction = toTarget.Normalized();
		Velocity = direction * Speed;
		Rotation = direction.Angle();
		MoveAndSlide();
	}

	private void _on_detection_body_entered(Node2D body)
	{
		if (body is CharacterBody2D player)
		{
			targetPlayer = player;
		}
	}

	public async void _on_body_entered(CharacterBody2D body)
	{
		if (isExploding)
			return;

		if (body == targetPlayer)
		{
			ExplodeAndGameOver();
			return;
		}
	}

	public async void _on_area_entered(Area2D area)
	{
		if (isExploding)
			return;

		if (area != null)
		{
			area.CallDeferred(Node.MethodName.QueueFree);
		}

		await DestroySelf();
	}

	private async Task DestroySelf()
	{
		if (isExploding)
			return;

		isExploding = true;

		if (EXPSound != null)
			EXPSound.Play();

		var sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
		if (sprite != null)
			sprite.Hide();

		var explosion = GetNodeOrNull<Sprite2D>("Explosion");
		if (explosion != null)
			explosion.Show();

		var collision = GetNodeOrNull<CollisionShape2D>("Radius");
		if (collision != null)
			collision.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);

		if (AnimationPlayer != null)
		{
			AnimationPlayer.Play("Explode");
			await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
		}

		QueueFree();
	}

	private async void ExplodeAndGameOver()
	{
		if (isExploding)
			return;

		isExploding = true;

		if (targetPlayer != null)
		{
			targetPlayer.CallDeferred(Node.MethodName.QueueFree);
		}

		var sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
		if (sprite != null)
			sprite.Hide();

		var explosion = GetNodeOrNull<Sprite2D>("Explosion");
		if (explosion != null)
			explosion.Show();

		if (EXPSound != null)
			EXPSound.Play();

		if (AnimationPlayer != null)
		{
			AnimationPlayer.Play("Explode");
			await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
		}

		QueueFree();
		GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToFile, "res://scenes/game_over.tscn");
	}
}
