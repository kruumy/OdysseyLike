using Godot;

namespace Scripts
{
    public partial class PlayerController : CharacterBody3D
    {
        [Export] public Node3D Camera;
        [Export] public Cap Cap;
        public PlayerState.State LastState { get; private set; }
        private PlayerState.State currentState;

        public PlayerState.State CurrentState
        {
            get => currentState;
            set
            {
                value.Player = this;
                LastState = currentState;
                currentState = value;
            }
        }
        public override void _Ready()
        {
            CurrentState = new PlayerState.Idle();
        }
        public override void _Process(double delta)
        {
            CurrentState?.Update((float)delta);
        }
        public override void _Input(InputEvent @event)
        {
            CurrentState?.Input(@event);
        }
    }
}

