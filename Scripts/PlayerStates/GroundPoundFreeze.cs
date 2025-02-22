using Godot;

namespace Scripts.PlayerState
{
    public class GroundPoundFreeze : GroundPoundBaseState
    {
        public const float GroundPoundFreezeLength = 0.5f;
        protected override void UpdateVelocity(ref Vector3 newVelocity, float delta)
        {
            if (!Player.CoolDowns.ContainsKey("GroundPoundFreeze"))
            {
                Player.CoolDowns["GroundPoundFreeze"] = GroundPoundFreezeLength;
            }
            else
            {
                newVelocity.X = Mathf.MoveToward(newVelocity.X, 0f, GroundPoundFreezeLength * 100 * (float)delta);
                newVelocity.Y = Mathf.MoveToward(newVelocity.Y, 0f, GroundPoundFreezeLength * 100 * (float)delta * 10);
                newVelocity.Z = Mathf.MoveToward(newVelocity.Z, 0f, GroundPoundFreezeLength * 100 * (float)delta);
            }
        }
        protected override void PostUpdate()
        {
            if(Player.CoolDownsFinishedThisFrame.Contains("GroundPoundFreeze"))
            {
                Player.CurrentState = new GroundPound();
            }
        }
    }
}

