using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace SimonSays;

public partial class Pie : Control
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

	public IEnumerable<Slice> RandomSlices()
	{
		List<Slice> allSlices = this.AllSlices().ToList();
		int k = Globals.WeightedRandom(new[] {70, 15, 10, 5}) + 1;

		for (int i = 0; i < k; i++)
		{
			int idx = Globals.Random.Next(allSlices.Count);
			Slice s = allSlices[idx];
			allSlices.RemoveAt(idx);

			yield return s;
		}
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

	public void DeactivateAllSlices()
	{
		DeactivateSlices(this.AllSlices());
	}

	private static void ActivateSlices(IEnumerable<Slice> slices)
	{
		List<Slice> sliceList = slices.ToList();
		GD.Print($"Activating {sliceList.Count} slices");
		foreach (Slice slice in sliceList)
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

	private Slice[] AllSlices()
	{
		return new[]
		{
			this.Top,
			this.Bottom,
			this.Left,
			this.Right,
		};
	}
}
