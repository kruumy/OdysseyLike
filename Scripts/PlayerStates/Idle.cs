namespace Scripts.PlayerState
{
    public class Idle : State
    {
        protected override void PostUpdate()
        {
            if(Player.CoolDowns.ContainsKey("CoyoteJumpOpening") && Player.IsOnFloor())
            {
                Player.CurrentState = new Jump();
            }
        }
    }
}

