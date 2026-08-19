using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public Vector2 Direction;
	public int Speed = 300;
	public float Friction = 5f;
	public float rotationSpeed = 5;
	[Signal] public delegate void ShootBulletEventHandler(Vector2 pos, Vector2 direction);
	public bool canShoot = true;

	[Export]public AudioStreamPlayer2D engineSound;
	
	// Called when the 	node enters the scene tree for the first time.
	public override void _Ready()
	{
		var shootTimer = GetNode<Timer>("ShootTimer");
		shootTimer.Timeout += _on_shoot_timer_timeout;

		if (engineSound != null)
		{
			engineSound.VolumeDb = -15f;
			engineSound.PitchScale = 0.8f;
			engineSound.Play();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		
		
		Vector2 velocity = Velocity;
		Direction = Input.GetVector("left", "right","up","down");
		velocity = velocity.MoveToward(Direction * Speed, Friction);
		if (Direction != Vector2.Zero)
		{
			float targatedRotation = Direction.Angle();
			Rotation = Mathf.LerpAngle(Rotation, targatedRotation, (float)delta * rotationSpeed);
			engineSound.VolumeDb = Mathf.Lerp(engineSound.VolumeDb, 0.0f, (float)delta * 5f);
		}else
		{
			engineSound.VolumeDb = Mathf.Lerp(engineSound.VolumeDb, -15.0f, (float)delta * 5f);
			engineSound.PitchScale = Mathf.Lerp(engineSound.PitchScale, 0.8f,(float)delta * 5f);
		}
		shoot();
		Velocity = velocity;
		MoveAndSlide();
	}
	public void shoot()
	{
		var shootTimer = GetNode<Timer>("ShootTimer");
		Vector2 shootDirection = Vector2.Right.Rotated(Rotation).Normalized();
		if (Input.IsActionJustPressed("shoot") && canShoot)
		{
			canShoot = false;
			shootTimer.Start();
			EmitSignal(SignalName.ShootBullet, GlobalPosition, shootDirection);
		}
	}
	
	public void _on_shoot_timer_timeout()
	{
		canShoot = true;
	}
}
