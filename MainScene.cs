using System;
using System.Collections.Generic;
using Godot;

namespace SimonSays;

public partial class MainScene : Control
{
	[Export] private Pie Pie { get; set; }

	private Game Game { get; set; }
	private Random Random = new();

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

		if (inputEvent.IsActionPressed("ui_accept"))
		{
			Slice slice = this.Pie.RandomSlice(this.Random);
			this.Game.AddMove(new(new()
			{
				slice,
			}));
			return;
		}

		// Handle slice selection
		foreach ((string name, Func<Pie, Slice> sliceGetter) in ActionMap)
		{
			Slice slice = sliceGetter(this.Pie);

			if (inputEvent.IsActionPressed(name) && !slice.IsActive())
			{
				slice.SetActive(true);
			}
			else if (inputEvent.IsActionReleased(name) && slice.IsActive())
			{
				slice.SetActive(false);
			}
		}
	}

	public override void _Ready()
	{
		this.Game = new(this.Pie);
	}
}
