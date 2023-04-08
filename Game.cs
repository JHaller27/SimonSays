using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimonSays;

public class Game
{
	private List<Move> Moves = new();
	private Pie Pie;
	private int MoveIdx;

	public bool AcceptUserInput { get; private set; }

	public Game(Pie pie)
	{
		this.Pie = pie;
		this.AcceptUserInput = true;
	}

	public async Task AddMove(Move move)
	{
		this.MoveIdx = 0;
		this.AcceptUserInput = false;

		await this.Replay();
		this.Moves.Add(move);
		await this.Pie.SimulateSlices(move.Slices);

		this.AcceptUserInput = true;
	}

	public bool VerifyPlayerMove(IEnumerable<Slice> activeSlices)
	{
		if (!this.AcceptUserInput)
		{
			throw new("Game is still replaying");
		}

		HashSet<Slice> activeSliceSet = activeSlices.ToHashSet();

		List<Slice> expectedSlices = this.Moves[this.MoveIdx++].Slices;
		HashSet<Slice> expectedSliceSet = expectedSlices.ToHashSet();

		return // Return true if...
			// ...all expected slices are active and...
			expectedSlices.TrueForAll(s => activeSliceSet.Contains(s)) &&
			// ...all active slices are expected
			activeSliceSet.ToList().TrueForAll(s => expectedSliceSet.Contains(s));
	}

	private async Task Replay()
	{
		foreach (Move move in this.Moves)
		{
			await this.Pie.SimulateSlices(move.Slices);
		}
	}

	public record Move(List<Slice> Slices);
}
