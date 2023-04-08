using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace SimonSays;

public class BetterTimer
{
	private readonly Timer BaseTimer;
	private readonly List<DelayedAction> DelayedActions = new();
	private int ActionIndex;

	public BetterTimer(Timer timer)
	{
		this.BaseTimer = timer;
		this.BaseTimer.Timeout += this.RunNext;
	}

	public BetterTimer Clear()
	{
		this.DelayedActions.Clear();
		return this;
	}

	public BetterTimer RunAfter(double delay, Action callback)
	{
		this.DelayedActions.Add(new(delay, callback));
		return this;
	}

	public BetterTimer Run(Action callback)
	{
		this.DelayedActions.Add(new(null, callback));
		return this;
	}

	public void Start()
	{
		this.ActionIndex = 0;

		double? delay = this.DelayedActions[this.ActionIndex].Delay;
		if (delay is null)
		{
			this.RunNext();
			return;
		}

		this.BaseTimer.WaitTime = delay.Value;
		this.BaseTimer.Start();
	}

	private void RunNext()
	{
		while (true)
		{
			this.DelayedActions[this.ActionIndex++].Callback();

			if (this.ActionIndex >= this.DelayedActions.Count)
			{
				this.BaseTimer.Stop();
				return;
			}

			double? delay = this.DelayedActions[this.ActionIndex].Delay;
			if (delay is null)
			{
				continue;
			}

			this.BaseTimer.WaitTime = delay.Value;
			this.BaseTimer.Start();
			break;
		}
	}

	private record DelayedAction(double? Delay, Action Callback);
}
