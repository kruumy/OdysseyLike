namespace Scripts.PlayerState
{
    public class Idle : State
    {
        protected override void PostUpdate()
        {
            if(CoolDowns.ContainsKey("CoyoteJumpOpening") && Player.IsOnFloor())
            {
                Player.CurrentState = new Jump();
            }
        }
    }
}

