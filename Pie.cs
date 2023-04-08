using System.Collections.Generic;
using System.Linq;
using Godot;

namespace SimonSays;

public partial class Pie : Node2D
{
	[Export] private Color[] Colors { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.RecolorChildren();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private List<Node2D> ListSlices()
	{
		return this.GetChildren()
			.Cast<Node2D>()
			.ToList();
	}

	private void RecolorChildren()
	{
		List<Node2D> slices = this.ListSlices();
		for (int i = 0; i < slices.Count; i++)
		{
			Node2D slice = slices[i];
			slice.Modulate = this.Colors[i % this.Colors.Length];
		}
	}
}
