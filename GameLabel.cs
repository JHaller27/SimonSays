using System;
using Godot;

namespace SimonSays;

public partial class GameLabel : Label
{
	[Signal]
	public delegate void DoneDisplayingEventHandler();

	[Export] private double DefaultDuration;

	private BetterTimer BetterTimer;
	private string Message;
	private Action Callback;

	public override void _Ready()
	{
		this.BetterTimer = new(this.GetNode<Timer>("Timer"));
	}

	public void DisplayThen(string message, double duration, Action callback)
	{
		this.Message = message;
		this.Callback = callback;

		this.BetterTimer
			.Clear()
			.Run(() =>
			{
				this.Text = this.Message;
				this.Visible = true;
			})
			.RunAfter(duration, () =>
			{
				this.Visible = false;
				this.Callback();
			})
			.Start();
	}

	public void DisplayThen(string message, Action callback) => this.DisplayThen(message, this.DefaultDuration, callback);
}
