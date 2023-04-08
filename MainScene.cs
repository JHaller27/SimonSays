using System;
using System.Collections.Generic;
using Godot;

namespace SimonSays;

public partial class MainScene : Control
{
	[Export] private Pie Pie { get; set; }

	private Game Game { get; set; }

	private static readonly Dictionary<string, Func<Pie, Slice>> ActionMap = new()
	{
		["simon_up"] = p => p.Top,
		["simon_down"] = p => p.Bottom,
		["simon_left"] = p => p.Left,
		["simon_right"] = p => p.Right,
	};

	public override void _Input(InputEvent inputEvent)
	{
		if (!this.Game.AcceptUserInput) return;

		// Handle slice selection
		foreach ((string name, Func<Pie, Slice> sliceGetter) in ActionMap)
		{
			if (inputEvent.IsActionPressed(name))
			{
				Slice slice = sliceGetter(this.Pie);
				slice.SetActive(true);
			}
			else if (inputEvent.IsActionReleased(name))
			{
				Slice slice = sliceGetter(this.Pie);
				slice.SetActive(false);
			}
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.Game = new(this.Pie);
		this.Game.AddMove(new(new()
		{
			this.Pie.Top,
		})).GetAwaiter().GetResult();
	}
}
