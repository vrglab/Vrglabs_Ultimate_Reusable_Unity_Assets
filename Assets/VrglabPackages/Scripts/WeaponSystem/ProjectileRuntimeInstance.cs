using UnityEngine;
using UnityEngine.UIElements;


[RequireComponent (typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class ProjectileRuntimeInstance : MonoBehaviour
{
    private DataHolder data;
    private Vector2 direction;
    private int caster_id;
    private float speed;
    private int damage;
    private float lifespan;
    private float timer;
    private Projectile baseType;

    private Rigidbody2D rb;
    private SpriteRenderer renderer;
    private BoxCollider2D boxCollider;

    public void Initialize(DataHolder dataHolder, Vector2 dir, Projectile baseType, int caster_id)
    {
        this.data = dataHolder;
        this.direction = dir.normalized;
        this.baseType = baseType;
        this.caster_id = caster_id;

        speed = data.GetData<float>("speed");
        damage = data.GetData<int>("damage");
        lifespan = data.GetData<float>("lifespan");

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearVelocity = direction * speed;

        renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = baseType.GetImage();

        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.size = new Vector2(0.5f, 0.5f);
        boxCollider.isTrigger = true;

        timer = lifespan;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        var target = col.GetComponent<IWeaponTarget>();
        if (target != null)
        {
            target.OnHit(damage);
            if (data.GetData<bool>("destroy_on_hit"))
            {
                Destroy(gameObject);
            }
        }

        if (data.GetData<bool>("destroy_on_hit") && col.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
