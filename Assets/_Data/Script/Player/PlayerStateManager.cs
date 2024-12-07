using UnityEngine;

public enum PlayerState
{
    Idle = 0,
    Run = 1,
    Jump = 2,
    Fall = 3,
    Dash = 4,
    WallSliding = 5,
    WallJumping = 6
}

public class PlayerStateManager : NhoxMonoBehaviour
{
    private PlayerState stateManager;
    public PlayerState StateManager => stateManager;

    public void SetState(PlayerState state)
    {
        stateManager = state;
    }
}
