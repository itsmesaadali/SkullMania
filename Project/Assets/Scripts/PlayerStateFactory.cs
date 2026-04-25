using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateFactory
{
    private PlayerStateMachine ctx;
    
    private IdleState idle;
    private WalkingState walking;
    private JumpingState jumping;
    private FallingState falling;
    private DoubleJumpState doubleJump;
    
    public PlayerStateFactory(PlayerStateMachine ctx)
    {
        this.ctx = ctx;
    }
    
    public PlayerBaseState Idle()
    {
        if (idle == null)
            idle = new IdleState(ctx, this);
        return idle;
    }
    
    public PlayerBaseState Walking()
    {
        if (walking == null)
            walking = new WalkingState(ctx, this);
        return walking;
    }
    
    public PlayerBaseState Jumping()
    {
        if (jumping == null)
            jumping = new JumpingState(ctx, this);
        return jumping;
    }
    
    public PlayerBaseState Falling()
    {
        if (falling == null)
            falling = new FallingState(ctx, this);
        return falling;
    }
    
    public PlayerBaseState DoubleJump()
    {
        if (doubleJump == null)
            doubleJump = new DoubleJumpState(ctx, this);
        return doubleJump;
    }
}
