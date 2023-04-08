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

		if (inputEvent.IsActionPressed("ui_accept"))
		{
			this.Game.PlayMoves();
			return;
		}

		// Handle slice selection
		foreach ((string name, Func<Pie, Slice> sliceGetter) in ActionMap)
		{
			Slice slice = sliceGetter(this.Pie);

			if (inputEvent.IsActionPressed(name) && !slice.IsActive())
			{
				slice.SetActive(true);
				if (!this.Game.VerifySlice(slice))
				{
					// If slice is wrong...
					GD.Print("Wrong slice!");
					slice.SetActive(false);
					this.Game.ResetMoveVerification();
					this.Game.PlayMoves();
				}
			}
			else if (inputEvent.IsActionReleased(name) && slice.IsActive())
			{
				slice.SetActive(false);
				if (!this.Game.VerifyMove())
				{
					// If released a slice when not all expected slices were pressed...
					GD.Print("Missing slice!");
					this.Game.ResetMoveVerification();
					this.Game.PlayMoves();
				}
				else
				{
					// Slice released when all expected slices were pressed
					GD.Print("Correct move!");
					if (this.Game.VerifyRound())
					{
						GD.Print("Next round...\n");
						this.Game.ResetMoveVerification();
						this.Game.AddRandomMove();
						this.Game.PlayMoves();
					}
					else
					{
						this.Game.AdvanceMoveVerification();
					}
				}
			}
		}
	}

	public override void _Ready()
	{
		this.Game = new(this.Pie);
		this.Game.AddRandomMove();
	}
}
