using System.Collections.Generic;
using System.Linq;
using Godot;

namespace SimonSays;

public class Game
{
	private List<Move> Moves = new();
	private Pie Pie;
	private int VerifyMoveIdx;

	public bool AcceptUserInput { get; private set; }

	public Game(Pie pie)
	{
		this.Pie = pie;
		this.AcceptUserInput = true;

		this.Pie.SimulationDone += this.AfterSimulation;
	}

	public void AddMove(Move move)
	{
		this.VerifyMoveIdx = 0;
		this.AcceptUserInput = false;

		this.Moves.Add(move);
		this.PlayMoves();
	}

	public bool VerifyPlayerMove(IEnumerable<Slice> activeSlices)
	{
		if (!this.AcceptUserInput)
		{
			throw new("Game is still replaying");
		}

		HashSet<Slice> activeSliceSet = activeSlices.ToHashSet();

		List<Slice> expectedSlices = this.Moves[this.VerifyMoveIdx++].Slices;
		HashSet<Slice> expectedSliceSet = expectedSlices.ToHashSet();

		return // Return true if...
			// ...all expected slices are active and...
			expectedSlices.TrueForAll(s => activeSliceSet.Contains(s)) &&
			// ...all active slices are expected
			activeSliceSet.ToList().TrueForAll(s => expectedSliceSet.Contains(s));
	}

	private void PlayMoves()
	{
		this.Pie.SimulateMoves(this.Moves);
	}

	private void AfterSimulation()
	{
		GD.Print("After simulation");
		this.AcceptUserInput = true;
	}
}
