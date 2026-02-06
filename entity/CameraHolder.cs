using Godot;
using System;

public partial class CameraHolder : Node3D
{
	[Export] private NodePath p_target;
	private Node3D _target;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_target = GetNode<Node3D>(p_target);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalPosition = GlobalPosition.Lerp(_target.GlobalPosition, (float)delta * 16f); 
	}
}
