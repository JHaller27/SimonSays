using System.Collections.Generic;
using System.Linq;
using Godot;

namespace SimonSays;

public class Game
{
	private readonly List<Move> Moves = new();
	private readonly Pie Pie;
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

		HashSet<Slice> expectedSlicesSet = this.Moves[this.VerifyMoveIdx++].Slices;

		return expectedSlicesSet.IsSubsetOf(activeSliceSet) &&
			   activeSliceSet.IsSubsetOf(expectedSlicesSet);
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
