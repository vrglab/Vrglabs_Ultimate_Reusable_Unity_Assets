using FMODUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementModule : ModuleSystem
{

    public class StaticModifiers : CoreModifier
    {
        public float Speed { get { return (float)this["speed"]; } private set { this["speed"] = value; } }
        public float RunSpeed { get { return (float)this["run_speed"]; } private set { this["run_speed"] = value; } }
        public float JumpSpeed { get { return (float)this["jmp_speed"]; } private set { this["jmp_speed"] = value; } }
        public float DashSpeed { get { return (float)this["dsh_speed"]; } private set { this["dsh_speed"] = value; } }
        public float JumpAcceleration { get { return (float)this["jmp_acceleration"]; } private set { this["jmp_acceleration"] = value; } }
        public float FallAcceleration { get { return (float)this["fall_acceleration"]; } private set { this["fall_acceleration"] = value; } }
        public float MaxFallSpeed { get { return (float)this["max_fall_speed"]; } private set { this["max_fall_speed"] = value; } }
        public float CoyoteTime { get { return (float)this["coyote_time"]; } private set { this["coyote_time"] = value; } }

        public StaticModifiers(float speed, float run_speed, float dash_speed, float jump_speed, float jump_acceleration, float fall_acceleration, float max_fall_speed, float coyoteTime)
        {
            this.Speed = speed;
            this.JumpSpeed = jump_speed;
            this.DashSpeed = dash_speed;
            this.RunSpeed = run_speed;
            this.JumpAcceleration = jump_acceleration;
            this.FallAcceleration = fall_acceleration;
            this.MaxFallSpeed = max_fall_speed;
            CoyoteTime = coyoteTime;

        }
    }

    public class DynamicModifiers : CoreModifier
    {
        public Directions CurrentFacingDir { get { return (Directions)this["facing_dir"]; } set { this["facing_dir"] = value; } }
        public Directions ValidFacingDir { get { return (Directions)this["facing_dir_valid"]; } set { this["facing_dir_valid"] = value; } }
        public Vector2 DashDirection { get { return (Vector2)this["dash_direction"]; } set { this["dash_direction"] = value; } }
        
        public bool IsAlive { get { return (bool)this["is_alive"]; } set { this["is_alive"] = value; } }

        public DynamicModifiers()
        {
            CurrentFacingDir = Directions.Left;
            ValidFacingDir = Directions.Left;
        }

    }

    public class ConditionModifiers : CoreModifier
    {
        // Player State Properties
        public bool IsDashing { get { return (bool)this["is_dashing"]; } set { this["is_dashing"] = value; } }
        public bool IsRunning { get { return (bool)this["is_running"]; } set { this["is_running"] = value; } }
        public bool IsGrounded { get { return (bool)this["is_grounded"]; } set { this["is_grounded"] = value; } }
        public bool IsJumping { get { return (bool)this["is_jumping"]; } set { this["is_jumping"] = value; } }
        public bool IsFalling { get { return !IsJumping && !IsGrounded; }  }


        // Input Request Flags
        public bool DashingRequested { get { return (bool)this["req_dash"]; } set { this["req_dash"] = value; } }
        public bool JumpRequested { get { return (bool)this["req_jump"]; } set { this["req_jump"] = value; } }
        public bool JumpReleased { get { return (bool)this["jump_released"]; } set { this["jump_released"] = value; } }


        // Timers and Buffers
        public float GroundedDelay { get { return (float)this["g_delay"]; } set { this["g_delay"] = value; } }
        public float GroundedTimer { get { return (float)this["g_timer"]; } set { this["g_timer"] = value; } }

        public float JumpBufferTime { get { return (float)this["jmp_buffer_time"]; } set { this["jmp_buffer_time"] = value; } }
        public float JumpBufferTimer { get { return (float)this["jmp_buffer_timer"]; } set { this["jmp_buffer_timer"] = value; } }

        public float DashDuration { get { return (float)this["dash_duration"]; } set { this["dash_duration"] = value; } }
        public float DashCooldown { get { return (float)this["dash_cooldown"]; } set { this["dash_cooldown"] = value; } }
        public float DashTimer { get { return (float)this["dash_timer"]; } set { this["dash_timer"] = value; } }
        public float DashCooldownTimer { get { return (float)this["dash_cooldown_timer"]; } set { this["dash_cooldown_timer"] = value; } }

        public float CoyoteTimeCounter { get { return (float)this["coyote_time_counter"]; } set { this["coyote_time_counter"] = value; } }

        // Jump Tracking
        public int RemainingJumps { get { return (int)this["remaining_air_jumps"]; } set { this["remaining_air_jumps"] = value; } }

        public bool WasGrounded { get { return (bool)this["was_grounded"]; } set { this["was_grounded"] = value; } }

        public ConditionModifiers(float groundedDelay, float jumpBufferTime, float dashDuration, float dashCooldown)
        {
            DashingRequested = false;
            IsDashing = false;

            IsRunning = false;

            IsGrounded = true;
            WasGrounded = true;
            IsJumping = false;


            JumpRequested = false;
            JumpReleased = false;

            GroundedDelay = groundedDelay;
            GroundedTimer = 0f;

            JumpBufferTime = jumpBufferTime;
            JumpBufferTimer = 0f;

            DashDuration = dashDuration;
            DashCooldown = dashCooldown;
            DashTimer = 0f;
            DashCooldownTimer = 0f;

            CoyoteTimeCounter = 0f;

            RemainingJumps = 0;
        }
    }

    private Rigidbody2D rg_body_2D;
    private StaticModifiers static_modifiers;
    private DynamicModifiers dynamic_modifiers;
    private ConditionModifiers condition_modifiers;

    public PlayerMovementModule(StaticModifiers modifiers, ConditionModifiers conditionModifiers)
    {
        this.static_modifiers = modifiers;
        dynamic_modifiers = new DynamicModifiers();
        condition_modifiers = conditionModifiers;
    }




    /* HELPER/VALIDATION FUNCTIONS */

    private bool validateMovement()
    {
        return !(condition_modifiers.IsDashing && condition_modifiers.DashingRequested);
    }

    private bool CheckGround()
    {
        Vector2 checkPosition = (Vector2)rg_body_2D.position + new Vector2(0, -0.5f);
        return Physics2D.OverlapCircle(checkPosition, 0.1f, LayerMask.GetMask("Ground", "B_ground", "G_ground", "R_ground", "Y_ground"));
    }

    private void jumpChecksAndValidations(DataHolder dataHolders)
    {
        if (condition_modifiers.GroundedTimer > 0)
        {
            condition_modifiers.GroundedTimer -= Time.deltaTime;
        }
        else if (CheckGround())
        {
            if (!condition_modifiers.IsGrounded)
            {
                // The player just landed, so check if they should jump due to the buffer
                jump(dataHolders);
            }

            if (!condition_modifiers.WasGrounded)
            {
                condition_modifiers.CoyoteTimeCounter = static_modifiers.CoyoteTime;
                condition_modifiers.RemainingJumps = dataHolders.GetData<int>("MaxJumps");
                condition_modifiers.DashCooldownTimer = 0f;
            }

            condition_modifiers.IsGrounded = true;
            condition_modifiers.IsJumping = false;
            condition_modifiers.WasGrounded = true;
            if(condition_modifiers.JumpBufferTimer > 0)
            {
                jump(dataHolders, true);
            }
        }
        else
        {
            condition_modifiers.IsGrounded = false;
            if (condition_modifiers.WasGrounded)
            {
                condition_modifiers.WasGrounded = false;
            }
        }
    }

    private void dashChecksAndValidations()
    {
        if (condition_modifiers.IsDashing)
        {

            condition_modifiers.DashTimer -= Time.deltaTime;


            if (condition_modifiers.DashTimer <= 0)
            {
                condition_modifiers.IsDashing = false;
                condition_modifiers.DashCooldownTimer = condition_modifiers.DashCooldown;
                rg_body_2D.linearVelocity = Vector2.zero; // Stop dash velocity
            }
        }
        else if (condition_modifiers.DashCooldownTimer > 0 && condition_modifiers.IsGrounded)
        {
            condition_modifiers.DashCooldownTimer -= Time.deltaTime;
        }
    }



    /* IMPLEMENTATION FUNCTIONS */

    private void setFacingDir(DataHolder dataHolders)
    {
        float x_movement = InputManager.Instance.GetValueData<float>("pl_mv_x");
        float y_movement = InputManager.Instance.GetValueData<float>("pl_mv_y");

        if (x_movement > 0f)
        {

            dynamic_modifiers.CurrentFacingDir = Directions.Left;
            dynamic_modifiers.ValidFacingDir = dynamic_modifiers.CurrentFacingDir;
        }

        if (x_movement < 0f)
        {
            dynamic_modifiers.CurrentFacingDir = Directions.Right;
            dynamic_modifiers.ValidFacingDir = dynamic_modifiers.CurrentFacingDir;
        }

        if (y_movement > 0f)
        {
            dynamic_modifiers.CurrentFacingDir = Directions.Up;
        }

        if (y_movement < 0f)
        {
            dynamic_modifiers.CurrentFacingDir = Directions.Down;
        }

        if (y_movement == 0f)
        {
            dynamic_modifiers.CurrentFacingDir = dynamic_modifiers.ValidFacingDir;
        }

        dataHolders.SetData("player_facing", dynamic_modifiers.CurrentFacingDir);
    }

    private void updateConditions()
    {
        condition_modifiers.DashingRequested = InputManager.Instance.GetKeyDown("pl_mv_dash");
        condition_modifiers.JumpRequested = InputManager.Instance.GetKeyDown("pl_mv_jump");

        // Start or reset the jump buffer timer when the jump button is pressed
        if (condition_modifiers.JumpRequested)
        {
            condition_modifiers.JumpBufferTimer = condition_modifiers.JumpBufferTime;
        }

        condition_modifiers.JumpReleased = InputManager.Instance.GetKeyUp("pl_mv_jump");
    }

    private void applyGravity()
    {
        if (!condition_modifiers.IsGrounded)
        {
            float fallSpeed = rg_body_2D.linearVelocity.y;

            // If the jump button is released mid-jump and the player is still moving upwards
            if (condition_modifiers.JumpReleased && fallSpeed > 0)
            {
                // Reduce upward velocity to create a jump stop effect
                fallSpeed *= 0.5f; // Adjust this factor as needed for a smoother or sharper stop
            }
            else
            {
                // Apply normal fall acceleration
                fallSpeed -= static_modifiers.FallAcceleration * Time.deltaTime;
                fallSpeed = Mathf.Max(fallSpeed, -static_modifiers.MaxFallSpeed); // Cap fall speed
            }

            rg_body_2D.linearVelocity = new Vector2(rg_body_2D.linearVelocity.x, fallSpeed);
        }
    }


    /* MOVEMENT IMPLEMENTATIONS */

    private void jump(DataHolder dataHolders, bool bypass = false)
    {
        if (((condition_modifiers.IsGrounded || 
            (condition_modifiers.CoyoteTimeCounter > 0f && condition_modifiers.IsFalling) || 
            (condition_modifiers.RemainingJumps > 0 && condition_modifiers.IsJumping)) && condition_modifiers.JumpRequested) 
            || bypass
            )
        {
             rg_body_2D.linearVelocity = new Vector2(rg_body_2D.linearVelocity.x, static_modifiers.JumpSpeed * static_modifiers.JumpAcceleration);

            condition_modifiers.RemainingJumps--;

            condition_modifiers.IsGrounded = false;
            condition_modifiers.IsJumping = true;

            condition_modifiers.JumpBufferTimer = 0;
            condition_modifiers.CoyoteTimeCounter = 0f;
        }
    }

    private void basicMovement()
    {
        float x_movement = InputManager.Instance.GetValueData<float>("pl_mv_x");

        if(validateMovement())
        {
            rg_body_2D.transform.position = new Vector2(
                rg_body_2D.transform.position.x - (condition_modifiers.IsRunning ? static_modifiers.RunSpeed : static_modifiers.Speed) * x_movement * Time.deltaTime,
                rg_body_2D.transform.position.y
            );
        }
    }


    private void dash(DataHolder dataHolders)
    {
        bool enableDash = dataHolders.GetData<bool>("Dash");


        if (enableDash && condition_modifiers.DashingRequested && !condition_modifiers.IsDashing && condition_modifiers.DashCooldownTimer <= 0f)
        {
            condition_modifiers.IsDashing = true;
            condition_modifiers.DashTimer = condition_modifiers.DashDuration;

            float dashSpeed = static_modifiers.DashSpeed;
            Vector2 dashDirection = Vector2.zero;

            switch (dynamic_modifiers.CurrentFacingDir)
            {
                case Directions.Right:
                    dashDirection = Vector2.right;
                    break;
                case Directions.Left:
                    dashDirection = Vector2.left;
                    break;
            }

            dynamic_modifiers.DashDirection = dashDirection;
            rg_body_2D.linearVelocity = dashDirection * dashSpeed;
        }
    }


    /* EXTERNALLY CALLED */

    public override DataHolder UpdateModule(DataHolder dataHolders, Components components, Functions functions)
    {
        updateConditions();

        if (!condition_modifiers.IsGrounded)
        {
            condition_modifiers.CoyoteTimeCounter -= Time.deltaTime;
        }

        if (condition_modifiers.JumpBufferTimer > 0)
        {
            condition_modifiers.JumpBufferTimer -= Time.deltaTime;
        }

        if (condition_modifiers.JumpRequested)
        {
            jump(dataHolders);
            condition_modifiers.GroundedTimer = condition_modifiers.GroundedDelay;
        }

        if (condition_modifiers.DashingRequested)
        {
            dash(dataHolders);
        }

        if(!condition_modifiers.IsDashing)
        {
            applyGravity();

            condition_modifiers.JumpReleased = false;
        }

        basicMovement();

        return dataHolders;
    }

    public override DataHolder UpdatePhysicsModule(DataHolder dataHolders, Components components, Functions functions)
    {
        setFacingDir(dataHolders);

        jumpChecksAndValidations(dataHolders);

        dashChecksAndValidations();

        return dataHolders;
    }

    public override DataHolder InitModule(DataHolder dataHolders, Components components, Functions functions)
    {

        try
        {
            dataHolders.GetData<bool>("Dash");
        }
        catch (Exception e)
        {
            dataHolders.SetData("Dash", false);
        }

        try
        {
            dataHolders.GetData<int>("MaxJumps");
        }
        catch (Exception e)
        {
            dataHolders.SetData("MaxJumps", 1);
        }

        condition_modifiers.RemainingJumps = dataHolders.GetData<int>("MaxJumps");

        rg_body_2D = (Rigidbody2D)components["rigidbody"];

        return dataHolders;
    }
}
