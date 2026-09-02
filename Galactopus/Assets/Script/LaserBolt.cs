using UnityEngine;

// Alternativa liviana a Rigidbody: mueve el proyectil por transform,
// ideal para rail shooters donde no necesitas fisica real.
public class LaserBolt : MonoBehaviour
{
    private Vector3 velocity;

    public void SetVelocity(Vector3 newVelocity)
    {
        velocity = newVelocity;
    }

    void Update()
    {
        transform.position += velocity * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Aca enganchas tu logica de daño/colision (enemigos, obstaculos, etc.)
        Destroy(gameObject);
    }
}
