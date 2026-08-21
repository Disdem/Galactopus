using UnityEngine;

/// <summary>
/// Se encarga SOLO de disparar proyectiles con la polaridad actual de la nave.
/// No sabe nada de movimiento ni de rotación.
/// </summary>
[AddComponentMenu("Ship/Weapon Controller")]
public class ShipWeaponController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private MonoBehaviour inputProviderSource;
    [Tooltip("Opcional: si la nave tiene mecánica de polaridad, el proyectil heredará el color actual.")]
    [SerializeField] private ShipPolarityController polarityController;

    [Header("Disparo")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 0.15f;
    public float projectileSpeed = 60f;

    private IShipInputProvider input;
    private float cooldown;

    void Awake()
    {
        input = inputProviderSource as IShipInputProvider ?? GetComponent<IShipInputProvider>();
    }

    void Update()
    {
        if (input == null) return;
        cooldown -= Time.deltaTime;

        if (input.FireHeld && cooldown <= 0f)
        {
            Fire();
            cooldown = fireRate;
        }
    }

    private void Fire()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject obj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile proj = obj.GetComponent<Projectile>();

        if (proj != null)
        {
            // Si no hay polaridad configurada, dispara como "White" por defecto.
            ShipPolarity polarity = polarityController != null ? polarityController.CurrentPolarity : ShipPolarity.White;
            proj.Initialize(polarity, projectileSpeed);
        }
    }
}
