using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingState : PlayerBaseState
{
    public WalkingState(PlayerStateMachine ctx, PlayerStateFactory factory) 
        : base(ctx, factory) { }
    
    public override void Enter()
    {
        // Update animations
        if (ctx.animator != null)
        {
            ctx.animator.SetBool("IsWalking", true);
            ctx.animator.SetBool("IsJumping", false);
            ctx.animator.SetBool("IsFalling", false);
        }
        
        Debug.Log("Entering Walking State");
    }
    
    public override void Update()
    {
        // Move the player
        ctx.rb.velocity = new Vector2(ctx.moveInput * ctx.speed, ctx.rb.velocity.y);
        
        // Flip sprite based on direction
        if (ctx.moveInput > 0)
            ctx.transform.localScale = new Vector3(1, 1, 1);
        else if (ctx.moveInput < 0)
            ctx.transform.localScale = new Vector3(-1, 1, 1);
        
        // Set animation speed
        if (ctx.animator != null)
            ctx.animator.SetFloat("Speed", Mathf.Abs(ctx.moveInput));
        
        // Transition to idle
        if (Mathf.Abs(ctx.moveInput) == 0)
        {
            ctx.ChangeState(factory.Idle());
        }
        // Transition to jumping
        else if (Input.GetKeyDown(KeyCode.Space) && ctx.isGrounded)
        {
            ctx.ChangeState(factory.Jumping());
        }
    }
    
    public override void Exit()
    {
        if (ctx.animator != null)
            ctx.animator.SetBool("IsWalking", false);
        
        Debug.Log("Exiting Walking State");
    }
}