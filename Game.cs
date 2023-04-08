using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace SimonSays;

public class Game
{
	private readonly List<Move> Moves = new();
	private readonly Pie Pie;
	private int NextVerifyMoveIdx;
	private HashSet<Slice> CurrentMoveSet;

	public bool AcceptUserInput { get; private set; }

	public Game(Pie pie) : this(pie, new Random().Next())
	{
	}

	public Game(Pie pie, int seed)
	{
		this.Pie = pie;
		this.AcceptUserInput = true;

		this.Pie.SimulationDone += this.AfterSimulation;
		GD.Print(seed);
		Globals.Random = new(seed);
	}

	public int MoveCount() => this.Moves.Count;

	public void AddRandomMove()
	{
		HashSet<Slice> slices = this.Pie.RandomSlices().ToHashSet();

		this.AddMove(new(slices));
		this.ResetMoveVerification();
	}

	public bool VerifySlice(Slice activeSlice)
	{
		if (!this.AcceptUserInput)
		{
			throw new("Game is still replaying");
		}

		if (!this.CurrentMoveSet.Contains(activeSlice))
		{
			return false;
		}

		this.CurrentMoveSet.Remove(activeSlice);
		return true;
	}

	public bool VerifyMove()
	{
		return this.CurrentMoveSet.Count == 0;
	}

	public bool VerifyRound()
	{
		return this.NextVerifyMoveIdx >= this.Moves.Count;
	}

	public void PlayMoves()
	{
		this.AcceptUserInput = false;
		this.Pie.SimulateMoves(this.Moves);
	}

	public void ResetMoveVerification()
	{
		this.NextVerifyMoveIdx = 0;
		this.SetupMoveVerification();
	}

	public void AdvanceMoveVerification()
	{
		this.SetupMoveVerification();
	}

	private void AddMove(Move move)
	{
		this.Moves.Add(move);
	}

	private void SetupMoveVerification()
	{
		if (this.NextVerifyMoveIdx >= this.Moves.Count) return;

		this.Pie.DeactivateAllSlices();
		this.CurrentMoveSet = this.Moves[this.NextVerifyMoveIdx++].Slices.ToHashSet();
	}

	private void AfterSimulation()
	{
		GD.Print("After simulation");
		this.AcceptUserInput = true;
	}
}
