using System;
using System.Linq;

namespace SimonSays;

public static class Globals
{
	public static Random Random { get; set; } = new();

	public static int WeightedRandom(int[] weights)
	{
		int cumWeight = weights.Sum();
		int r = Random.Next(cumWeight);
		for (int val = 0; val < weights.Length; val++)
		{
			int weight = weights[val];
			if (r < weight)
			{
				return val;
			}

			r -= weight;
		}

		return weights.Length - 1;
	}
}
