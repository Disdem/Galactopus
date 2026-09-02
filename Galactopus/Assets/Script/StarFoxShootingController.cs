using UnityEngine;

public class StarFoxShootingController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Transform[] firePoints;

    [Header("Configuracion de Disparo")]
    [SerializeField] private float laserSpeed = 60f;
    [SerializeField] private float fireRate = 0.25f;
    [SerializeField] private float laserLifetime = 3f;

    [Header("Input")]
    [SerializeField] private string fireButton = "Fire1";

    private float nextFireTime = 0f;
    private int currentFirePointIndex = 0;

    void Update()
    {
        ProcessShooting();
    }

    private void ProcessShooting()
    {
        bool wantsToFire = Input.GetButton(fireButton);

        if (wantsToFire && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Fire()
    {
        if (laserPrefab == null || firePoints == null || firePoints.Length == 0)
            return;

        Transform spawnPoint = firePoints[currentFirePointIndex];

        GameObject laser = Instantiate(laserPrefab, spawnPoint.position, spawnPoint.rotation);

        Rigidbody laserRb = laser.GetComponent<Rigidbody>();
        if (laserRb != null)
        {
            laserRb.linearVelocity = transform.forward * laserSpeed;
        }
        else
        {
            LaserBolt bolt = laser.GetComponent<LaserBolt>();
            if (bolt != null)
            {
                bolt.SetVelocity(transform.forward * laserSpeed);
            }
        }

        Destroy(laser, laserLifetime);

        if (firePoints.Length > 1)
        {
            currentFirePointIndex = (currentFirePointIndex + 1) % firePoints.Length;
        }
    }
}
