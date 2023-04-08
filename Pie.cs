using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace SimonSays;

public partial class Pie : Node2D
{
	[Export] public Slice Top { get; private set; }
	[Export] public Slice Bottom { get; private set; }
	[Export] public Slice Left { get; private set; }
	[Export] public Slice Right { get; private set; }

	[Export] private double ActivateTime { get; set; }
	[Export] private double GapTime { get; set; }

	[Signal]
	public delegate void SimulationDoneEventHandler();

	private BetterTimer BetterTimer;

	public override void _Ready()
	{
		this.BetterTimer = new(this.GetNode<Timer>("Timer"));
	}

	public Slice RandomSlice(Random random)
	{
		return random.Next(4) switch
		{
			0 => this.Top,
			1 => this.Bottom,
			2 => this.Left,
			3 => this.Right,
			_ => throw new("Random value out of range"),
		};
	}

	public void SimulateMoves(IEnumerable<Move> moves)
	{
		List<Move> moveList = moves.ToList();

		this.BetterTimer.Clear()
			.Run(() => ActivateSlices(moveList.First().Slices))
			.RunAfter(this.ActivateTime, () => DeactivateSlices(moveList.First().Slices));

		foreach (Move move in moveList.Skip(1))
		{
			List<Slice> slices = move.Slices.ToList();
			this.BetterTimer
				.RunAfter(this.GapTime, () => ActivateSlices(slices))
				.RunAfter(this.ActivateTime, () => DeactivateSlices(slices));
		}

		this.BetterTimer
			.Run(() => EmitSignal(SignalName.SimulationDone));

		this.BetterTimer.Start();
	}

	private static void ActivateSlices(IEnumerable<Slice> slices)
	{
		GD.Print("Activating slices");
		foreach (Slice slice in slices)
		{
			slice.SetActive(true);
		}
	}

	private static void DeactivateSlices(IEnumerable<Slice> slices)
	{
		GD.Print("Deactivating slices");

		foreach (Slice slice in slices)
		{
			slice.SetActive(false);
		}
	}
}
