using Godot;
using System;

public partial class CameraHolder : Node3D
{
	[Export] private NodePath p_positionTarget;
	private Node3D _positionTarget;
	[Export] private NodePath p_rotationTarget;
	private Node3D _rotationTarget;

	[Export] private float _movementFactor = 32f;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_positionTarget = GetNode<Node3D>(p_positionTarget);
		_rotationTarget = GetNode<Node3D>(p_rotationTarget);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalPosition = GlobalPosition.Lerp(_positionTarget.GlobalPosition, (float)delta * _movementFactor);
		GlobalRotation = _rotationTarget.GlobalRotation;
	}
}
