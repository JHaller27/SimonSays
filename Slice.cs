using Godot;

namespace SimonSays;

public partial class Slice : Node2D
{
	[Export] private Color ActiveColor { get; set; }
	[Export] private Color InactiveColor { get; set; }

	private State CurrentState { get; set; }

	public bool IsActive() => this.CurrentState is ActiveState;

	public override void _Ready()
	{
		base._Ready();
		this.SetState(new InactiveState(this));
	}

	public void SetActive(bool isActive)
	{
		this.SetState(isActive ? new ActiveState(this) : new InactiveState(this));
	}

	private void SetState(State state)
	{
		this.CurrentState = state;
		this.CurrentState.Activate();
	}

	// States

	private abstract class State
	{
		protected Slice Slice { get; }

		public State(Slice slice)
		{
			this.Slice = slice;
		}

		public void Activate()
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
