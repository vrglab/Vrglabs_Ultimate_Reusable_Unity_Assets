using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class PlatformerPlayerMovement
{
    private int max_valid_jumps, max_valid_dash_amnt = 1, currently_usable_jump_amnt, currently_usable_dash_amnt;
    private bool is_on_the_ground, facing_left = false;
    private float current_dash_time, current_dash_cooldown, current_jump_buffer_time, speed = 7, jump_speed = 9.8f, fall_speed = 0.1f,
    dash_speed = 14, max_dash_time = 0.2f, max_dash_cooldown_time = 0.1f, max_jump_buffer_time = 0.07f; 
    

    private Rigidbody2D rigidbody2D;
    private CameraShakeData shake_data;
    private ParticleSystem dust_effect;

    public PlatformerPlayerMovement(Rigidbody2D rigidbody2D, CameraShakeData cameraShakeData, ParticleSystem dust_effet)
    {
        this.rigidbody2D = rigidbody2D;
        this.shake_data = cameraShakeData;
        this.dust_effect = dust_effet;
    }

    public void Tick(Transform transform)
    {

        this.jump_speed = jump_speed;
        this.max_valid_jumps = max_valid_jumps;
        #region Inputs
        bool jump_start_requested = InputManager.Instance.GetKeyDown("pl_mv_y");
        bool jump_end_requested = InputManager.Instance.GetKeyUp("pl_mv_y");
        bool dash_requested = InputManager.Instance.GetKeyDown("pl_mv_dash");

        float x_movement = InputManager.Instance.GetValueData<float>("pl_mv_x");

        #endregion

        #region Core Movement
        if (!dash_requested)
        {
            transform.position = new Vector2(
                transform.position.x - speed * x_movement * Time.deltaTime,
                transform.position.y
            );
        }

        if (jump_start_requested && (is_on_the_ground || currently_usable_jump_amnt > 0) && !dash_requested)
        {
            Jump();
            currently_usable_jump_amnt--;
        }
        else if (jump_start_requested && (!is_on_the_ground || currently_usable_jump_amnt < 0) && !dash_requested)
        {
            current_jump_buffer_time = max_jump_buffer_time;
        }

        if (jump_end_requested && rigidbody2D.velocity.y > 0)
        {
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);

        }

        if (rigidbody2D.velocity.y < 0)
        {
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, rigidbody2D.velocity.y - fall_speed);
        }
        #endregion

        #region Dash movement

        if (dash_requested && current_dash_cooldown <= 0 && currently_usable_dash_amnt > 0)
        {
            current_dash_time = max_dash_time;
            currently_usable_dash_amnt--;
            if(is_on_the_ground)
                dust_effect.Play();
            CameraEffects.Instance.Shake(shake_data.Duration, shake_data.Magnitued);
        }

        if (x_movement > 0)
        {
            facing_left = true;
        }

        if (x_movement < 0)
        {
            facing_left = false;
        }

        if (current_dash_time > 0)
        {
            if (!facing_left)
                rigidbody2D.velocity = Vector2.right * dash_speed;
            else
                rigidbody2D.velocity = Vector2.left * dash_speed;
            current_dash_time -= Time.deltaTime;
            current_dash_cooldown = max_dash_cooldown_time;
        }
        else
        {
            rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
        }
        #endregion
    }

    private void Jump()
    {
        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jump_speed);
        dust_effect.Play();
    }

    private IEnumerator ChangeGroundStatus(bool status, float wait_time)
    {
        yield return new WaitForSeconds(wait_time);
        is_on_the_ground = status;
    }

    private IEnumerator ChangeGroundCollider(Collider2D col, float wait_time)
    {
        col.enabled = false;
        yield return new WaitForSeconds(wait_time);
        col.enabled = true;
    }

    public void Start()
    {
        currently_usable_jump_amnt = max_valid_jumps;
        currently_usable_dash_amnt = max_valid_dash_amnt;
    }

    
    public void Update()
    {
        if (is_on_the_ground)
        {
            currently_usable_jump_amnt = max_valid_jumps;
        }

        if (current_dash_cooldown > 0)
        {
            current_dash_cooldown -= Time.deltaTime;
        }

        if (is_on_the_ground && currently_usable_dash_amnt <= 0 && current_dash_cooldown <= 0)
        {
            currently_usable_dash_amnt = max_valid_dash_amnt;
        }

        if (current_jump_buffer_time > 0)
        {
            current_jump_buffer_time -= Time.deltaTime;
        }
    }

    public void LateUpdate(Transform player_bottom_pos_marker, LayerMask ground_check_area_mask, PlatformerPlayer Player ,float ground_check_area_size = 0.2f)
    {
        bool status = Physics2D.OverlapCircle(player_bottom_pos_marker.position, ground_check_area_size, ground_check_area_mask);
        Collider2D collider = Physics2D.OverlapCircle(player_bottom_pos_marker.position, ground_check_area_size, ground_check_area_mask);
        bool drop_requested = InputManager.Instance.GetKeyDown("pl_my_drop_down");

        if(drop_requested && collider != null)
        {
            if (collider.tag.Contains("droppable"))
                Player.StartCoroutine(ChangeGroundCollider(collider, 1f));
        } else
        {
            if (!status && is_on_the_ground)
            {
                Player.StartCoroutine(ChangeGroundStatus(status, 0.1f));

            }
            else if (status && !is_on_the_ground)
            {
                Player.StartCoroutine(ChangeGroundStatus(status, 0));

                if (current_jump_buffer_time > 0)
                {
                    Jump();
                    current_jump_buffer_time = 0;
                }

                if (rigidbody2D.velocity.y < 0)
                {
                    dust_effect.Play();
                }

                if (rigidbody2D.velocity.y <= -0.8f)
                {
                    CameraEffects.Instance.Shake(shake_data.Duration, shake_data.Magnitued);
                }
            }
        }
    }
}
