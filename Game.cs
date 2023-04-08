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
	private readonly Random Random = new();

	public bool AcceptUserInput { get; private set; }

	public Game(Pie pie)
	{
		this.Pie = pie;
		this.AcceptUserInput = true;

		this.Pie.SimulationDone += this.AfterSimulation;
	}

	public void AddRandomMove()
	{
		Slice slice = this.Pie.RandomSlice(this.Random);
		this.AddMove(new(new()
		{
			slice,
		}));
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

		this.CurrentMoveSet = this.Moves[this.NextVerifyMoveIdx++].Slices.ToHashSet();
	}

	private void AfterSimulation()
	{
		GD.Print("After simulation");
		this.AcceptUserInput = true;
	}
}
