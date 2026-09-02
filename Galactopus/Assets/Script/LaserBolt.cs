using UnityEngine;

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
        Destroy(gameObject);
    }
}
