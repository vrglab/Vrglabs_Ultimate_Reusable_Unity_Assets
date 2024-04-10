using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerPlayer : Creature
{
    [Header("Collision checks")]
    [SerializeField] private Transform player_bottom_pos_marker;
    [SerializeField] private LayerMask ground_check_area_mask;


    [SerializeField] private CameraShakeData shake_data = new CameraShakeData()
    {
        Magnitued = 0.083f,
        Duration = 0.025f
    };

    private Rigidbody2D rigidbody2D;

    [SerializeField] private MeleeHandler meleeHandler;
    [SerializeField] private ParticleSystem dust_effect;

    private PlatformerPlayerMovement platformerPlayerMovement;

    protected override void Movement()
    {
        platformerPlayerMovement.Tick(transform);
    }

    private void Start()
    {
        meleeHandler = new MeleeHandler(transform, transform, transform, gameObject);
        rigidbody2D = GetComponent<Rigidbody2D>();
        platformerPlayerMovement = new PlatformerPlayerMovement(rigidbody2D, shake_data, dust_effect);
        platformerPlayerMovement.Start();
    } 

    private void Update()
    {
        base.Update();
        meleeHandler.SetMeleeState();
        platformerPlayerMovement.Update();
    }

    private void LateUpdate()
    {
        platformerPlayerMovement.LateUpdate(player_bottom_pos_marker, ground_check_area_mask, this);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(player_bottom_pos_marker.position, 0.2f);
    }
}
