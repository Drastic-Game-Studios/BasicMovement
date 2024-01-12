using Godot;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Text.RegularExpressions;

public partial class Player : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;
	public const float MouseSensitivity = 0.006f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	private Node3D Head;
	private Camera3D Camera;
	private bool IsCursorEnabled = true;

	public override void _Ready()
	{
		Head = GetNode<Node3D>("Head");
		Camera = Head.GetNode<Camera3D>("Camera");

		
	}

	public override void _Input(InputEvent @event)
	{
		if (Input.IsActionPressed("change_mouse_mode"))
		{
			if (IsCursorEnabled)
			{
				IsCursorEnabled = false;
				Input.MouseMode = Input.MouseModeEnum.Captured;
			}
			else if (!IsCursorEnabled)
			{
				IsCursorEnabled = true;
				Input.MouseMode = Input.MouseModeEnum.Visible;
			}
			
		}
	}
	

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			Head.RotateY(-eventMouseMotion.Relative.X * MouseSensitivity);
			Camera.RotateX(-eventMouseMotion.Relative.Y * MouseSensitivity);

			Godot.Vector3 CameraRot = Camera.Rotation;
			CameraRot.X = Mathf.Clamp(CameraRot.X, Mathf.DegToRad(-80), Mathf.DegToRad(80));
			Camera.Rotation = CameraRot;
		}
	}
	public override void _PhysicsProcess(double delta)
	{	

		Godot.Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		Godot.Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
		Godot.Vector3 direction = (Head.Transform.Basis * new Godot.Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		if (direction != Godot.Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = 0.0f;
			velocity.Z = 0.0f;
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
