using System;
using System.Collections.Generic;
using Godot;

namespace SimonSays;

public partial class Pie : Node2D
{
	[Export] private Slice Top { get; set; }
	[Export] private Slice Bottom { get; set; }
	[Export] private Slice Left { get; set; }
	[Export] private Slice Right { get; set; }

	private static readonly Dictionary<string, Func<Pie, Slice>> ActionMap = new()
	{
		["simon_up"] = p => p.Top,
		["simon_down"] = p => p.Bottom,
		["simon_left"] = p => p.Left,
		["simon_right"] = p => p.Right,
	};

	public override void _Input(InputEvent inputEvent)
	{
		foreach ((string name, Func<Pie, Slice> sliceGetter) in ActionMap)
		{
			if (inputEvent.IsActionPressed(name))
			{
				Slice slice = sliceGetter(this);
				slice.SetActive(true);
			}
			else if (inputEvent.IsActionReleased(name))
			{
				Slice slice = sliceGetter(this);
				slice.SetActive(false);
			}
		}
	}
}
