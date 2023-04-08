using System;
using System.Collections.Generic;
using Godot;

namespace SimonSays;

public partial class MainScene : Control
{
	[Export] private Pie Pie { get; set; }

	[Export] private GameLabel GameLabel;

	private Game Game { get; set; }
	private Label RoundLabel;

	private static readonly Dictionary<string, Func<Pie, Slice>> ActionMap = new()
	{
		["simon_up"] = p => p.Top,
		["simon_down"] = p => p.Bottom,
		["simon_left"] = p => p.Left,
		["simon_right"] = p => p.Right,
	};

	public override void _Ready()
	{
		this.RoundLabel = this.GetNode<Label>("RoundContainer/Value");

		this.Game = new(this.Pie);
		this.Game.AddRandomMove();
		this.GameLabel.DoneDisplaying += this.ResetRound;

		this.ResetRound();
	}

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
					this.GameLabel.DisplayThen("Try again!", () =>
					{
						slice.SetActive(false);
						this.ResetRound();
					});
				}
			}
			else if (inputEvent.IsActionReleased(name) && slice.IsActive())
			{
				slice.SetActive(false);
				if (!this.Game.VerifyMove())
				{
					// If released a slice when not all expected slices were pressed...
					GD.Print("Missing slice!");
					this.GameLabel.DisplayThen("Try again!", this.ResetRound);
				}
				else
				{
					// Slice released when all expected slices were pressed
					GD.Print("Correct move!");
					if (this.Game.VerifyRound())
					{
						this.GameLabel.DisplayThen("Next round!", () =>
						{
							this.Game.AddRandomMove();
							this.ResetRound();
						});
					}
					else
					{
						this.Game.AdvanceMoveVerification();
					}
				}
			}
		}
	}

	private void ResetRound()
	{
		this.RoundLabel.Text = this.Game.MoveCount().ToString();
		this.Game.ResetMoveVerification();
		this.Game.PlayMoves();
	}
}
