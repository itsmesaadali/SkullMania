using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState
{
    protected PlayerStateMachine ctx;
    protected PlayerStateFactory factory;
    
    public PlayerBaseState(PlayerStateMachine ctx, PlayerStateFactory factory)
    {
        this.ctx = ctx;
        this.factory = factory;
    }
    
    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}