using Godot;
using System.Collections.Generic;

namespace Scripts
{
    public partial class PlayerController : CharacterBody3D
    {
        [Export] public Node3D Camera;
        [Export] public Cap Cap;
        public PlayerState.State LastState { get; private set; }
        private PlayerState.State currentState;
        public Dictionary<string,float> CoolDowns = new();
        public HashSet<string> CoolDownsFinishedThisFrame = new();
        public PlayerState.State CurrentState
        {
            get => currentState;
            set
            {
                value.Player = this;
                LastState = currentState;
                currentState = value;
                currentState.Init();
            }
        }
        public override void _Ready()
        {
            CurrentState = new PlayerState.Idle();
        }
        public override void _Process(double delta)
        {
            CoolDownsFinishedThisFrame.Clear();
            foreach (var key in CoolDowns.Keys)
            {
                CoolDowns[key] -= (float)delta;
                if (CoolDowns[key] <= 0)
                {
                    CoolDowns.Remove(key);
                    CoolDownsFinishedThisFrame.Add(key);
                }
            }
            CurrentState?.Update((float)delta);
        }
        public override void _Input(InputEvent @event)
        {
            CurrentState?.Input(@event);
        }
    }
}

