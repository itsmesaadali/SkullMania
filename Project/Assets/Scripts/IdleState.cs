using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : PlayerBaseState
{
    public IdleState(PlayerStateMachine ctx, PlayerStateFactory factory) 
        : base(ctx, factory) { }
    
    public override void Enter()
    {
        // Stop movement
        ctx.rb.velocity = new Vector2(0, ctx.rb.velocity.y);
        
        // Update animations
        if (ctx.animator != null)
        {
            ctx.animator.SetBool("IsWalking", false);
            ctx.animator.SetBool("IsJumping", false);
            ctx.animator.SetBool("IsFalling", false);
        }
        
        Debug.Log("Entering Idle State");
    }
    
    public override void Update()
    {
        // Transition to walking
        if (Mathf.Abs(ctx.moveInput) > 0)
        {
            ctx.ChangeState(factory.Walking());
        }
        // Transition to jumping
        else if (Input.GetKeyDown(KeyCode.Space) && ctx.isGrounded)
        {
            ctx.ChangeState(factory.Jumping());
        }
    }
    
    public override void Exit()
    {
        Debug.Log("Exiting Idle State");
    }
}