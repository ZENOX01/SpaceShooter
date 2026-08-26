using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public Vector2 Direction;
	public int Speed = 300;
	public float Friction = 5f;
	public float rotationSpeed = 5;
	
	public PackedScene BUlletScene = GD.Load<PackedScene>("res://scenes/bullets.tscn");
	[Signal] public delegate void ShootBulletEventHandler(Vector2 pos, Vector2 direction);
	public bool canShoot = true;

	public bool Has360Shot = false;
	public bool CanUseAbility = false;
	private float abilityTimer = 0f;

	[Signal] public delegate void PlayerLocEventHandler(Vector2 pos, Vector2 Direction);

	[Export] public AudioStreamPlayer2D engineSound;

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

	public void Unlock360Shot()
	{
		Has360Shot = true;
		CanUseAbility = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{

		EmitSignal(SignalName.PlayerLoc, Position, Direction);
		Vector2 velocity = Velocity;
		Direction = Input.GetVector("left", "right", "up", "down");
		velocity = velocity.MoveToward(Direction * Speed, Friction);
		if (Direction != Vector2.Zero)
		{
			float targatedRotation = Direction.Angle();
			Rotation = Mathf.LerpAngle(Rotation, targatedRotation, (float)delta * rotationSpeed);
			engineSound.VolumeDb = Mathf.Lerp(engineSound.VolumeDb, 0.0f, (float)delta * 5f);
		}
		else
		{
			engineSound.VolumeDb = Mathf.Lerp(engineSound.VolumeDb, -15.0f, (float)delta * 5f);
			engineSound.PitchScale = Mathf.Lerp(engineSound.PitchScale, 0.8f, (float)delta * 5f);
		}
		shoot();
		Velocity = velocity;
		if (Input.IsActionJustPressed("ability") && Has360Shot && CanUseAbility)
		{
			Fire360Shot();
			Has360Shot = false;
			CanUseAbility = false;
		}

		if (!CanUseAbility)
		{
			abilityTimer -= (float)delta;
			if (abilityTimer <= 0f)
				CanUseAbility = true;
		}
		MoveAndSlide();
	}

	private void Fire360Shot()
{
    int bulletCount = 12;

    for (int i = 0; i < bulletCount; i++)
    {
        float angle = (Mathf.Tau / bulletCount) * i;
        Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        var bullet = BUlletScene.Instantiate() as Bullets;

        GetTree().CurrentScene.AddChild(bullet);
        bullet.GlobalPosition = GlobalPosition;
        bullet.Direction = dir;
        bullet.Rotation = angle;
    }
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
