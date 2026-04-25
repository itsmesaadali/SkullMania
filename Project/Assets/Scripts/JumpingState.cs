using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class JumpingState : PlayerBaseState
{
    public JumpingState(PlayerStateMachine ctx, PlayerStateFactory factory) 
        : base(ctx, factory) { }
    
    public override void Enter()
    {
        // Apply jump force
        ctx.rb.velocity = new Vector2(ctx.rb.velocity.x, ctx.jumpForce);
        ctx.canDoubleJump = true;
        
        // Update animations
        if (ctx.animator != null)
        {
            ctx.animator.SetTrigger("Jump");
            ctx.animator.SetBool("IsJumping", true);
            ctx.animator.SetBool("IsWalking", false);
            ctx.animator.SetBool("IsFalling", false);
        }
        
        Debug.Log("Jumping!");
    }
    
    public override void Update()
    {
        // Air control
        ctx.rb.velocity = new Vector2(ctx.moveInput * ctx.speed, ctx.rb.velocity.y);
        
        // Check for double jump
        if (Input.GetKeyDown(KeyCode.Space) && ctx.canDoubleJump)
        {
            ctx.ChangeState(factory.DoubleJump());
        }
        // Transition to falling
        else if (ctx.rb.velocity.y < 0)
        {
            ctx.ChangeState(factory.Falling());
        }
    }
    
    public override void Exit()
    {
        if (ctx.animator != null)
            ctx.animator.SetBool("IsJumping", false);
        
        Debug.Log("Exiting Jumping State");
    }
}