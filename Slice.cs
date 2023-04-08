using Godot;


namespace SimonSays;

public partial class Slice : Node2D
{
	[Export] private Color ActiveColor { get; set; }
	[Export] private Color InactiveColor { get; set; }

	public override void _Ready()
	{
		base._Ready();
		this.Modulate = this.InactiveColor;
	}

	public void SetActive(bool isActive)
	{
		this.Modulate = isActive ? ActiveColor : InactiveColor;
	}
}
