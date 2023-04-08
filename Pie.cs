using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace SimonSays;

public partial class Pie : Node2D
{
	[Export] public Slice Top { get; private set; }
	[Export] public Slice Bottom { get; private set; }
	[Export] public Slice Left { get; private set; }
	[Export] public Slice Right { get; private set; }

	public async Task SimulateSlices(IEnumerable<Slice> slices)
	{
		IEnumerable<Slice> sliceList = slices as Slice[] ?? slices.ToArray();

		foreach (Slice slice in sliceList)
		{
			slice.SetActive(true);
		}

		// Wait a second or so...
		// Could use the following commented-out line, but that requires async-propagation ick.
		// Prefer to instead use _Process(double delta) with state-machine-like behavior...
		// which is also ugly, but I prefer complicated architecture over complicated async things.

		// WHEN THIS IS REMOVED, also remove the async-propagation!!!
		// await ToSignal(GetTree().CreateTimer(1), SceneTreeTimer.SignalName.Timeout);

		foreach (Slice slice in sliceList)
		{
			slice.SetActive(false);
		}
	}
}
