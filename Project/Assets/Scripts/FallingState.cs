using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingState : PlayerBaseState
{
    public FallingState(PlayerStateMachine ctx, PlayerStateFactory factory) 
        : base(ctx, factory) { }
    
    public override void Enter()
    {
        // Update animations
        if (ctx.animator != null)
        {
            ctx.animator.SetBool("IsFalling", true);
            ctx.animator.SetBool("IsJumping", false);
        }
        
        Debug.Log("Falling...");
    }
    
    public override void Update()
    {
        // Air control while falling
        ctx.rb.velocity = new Vector2(ctx.moveInput * ctx.speed, ctx.rb.velocity.y);
        
        // Check for double jump while falling
        if (Input.GetKeyDown(KeyCode.Space) && ctx.canDoubleJump)
        {
            ctx.ChangeState(factory.DoubleJump());
        }
    }
    
    public override void Exit()
    {
        if (ctx.animator != null)
        {
            ctx.animator.SetBool("IsFalling", false);
            // Play landing animation
            if (ctx.isGrounded)
                ctx.animator.SetTrigger("Land");
        }
        
        Debug.Log("Landed!");
    }
}