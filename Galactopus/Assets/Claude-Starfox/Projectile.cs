using UnityEngine;

/// <summary>
/// Proyectil genérico con polaridad. Sirve tanto para disparos del jugador
/// como de enemigos: la lógica de absorción/daño vive en quien lo recibe (ShipHealth).
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[AddComponentMenu("Ship/Projectile")]
public class Projectile : MonoBehaviour
{
    public ShipPolarity Polarity { get; private set; }
    public float damage = 10f;
    public float lifeTime = 5f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(ShipPolarity polarity, float speed)
    {
        Polarity = polarity;
        rb.linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        ShipHealth health = other.GetComponent<ShipHealth>();
        if (health != null)
        {
            health.ReceiveHit(Polarity, damage);
            Destroy(gameObject);
        }
    }
}
