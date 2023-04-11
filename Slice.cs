using Godot;

namespace SimonSays;

[Tool]
public partial class Slice : Control
{
	[Export] private Color ActiveColor { get; set; }
	[Export] private Color InactiveColor { get; set; }
	[Export] private AudioStream AudioStream;

	public bool IsActive() => this.CurrentState is ActiveState;

	private State CurrentState { get; set; }
	private AudioStreamPlayer AudioPlayer() => this.GetNode<AudioStreamPlayer>("AudioPlayer");

	public override void _Ready()
	{
		base._Ready();
		this.CurrentState = new InactiveState(this);

		this.AudioPlayer().Stream = this.AudioStream;
	}

	public override void _Process(double delta)
	{
		this.CurrentState.Process();
	}

	public void SetActive(bool isActive)
	{
		this.CurrentState = isActive ? new ActiveState(this) : new InactiveState(this);
		if (isActive)
		{
			this.AudioPlayer().Play();
		}
	}

	// States

	private abstract class State
	{
		protected Slice Slice { get; }

		protected State(Slice slice)
		{
			this.Slice = slice;
		}

		public void Process()
		{
			this.Slice.Modulate = this.Color;
		}

		protected abstract Color Color { get; }
	}

	private class ActiveState : State
	{
		public ActiveState(Slice slice) : base(slice)
		{
		}

		protected override Color Color => this.Slice.ActiveColor;
	}

	private class InactiveState : State
	{
		public InactiveState(Slice slice) : base(slice)
		{
		}

		protected override Color Color => this.Slice.InactiveColor;
	}
}
