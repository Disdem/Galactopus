using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Administra vida y energía de la nave. Contiene la lógica central de Ikaruga:
/// disparo de la MISMA polaridad -> se absorbe y carga energía.
/// disparo de polaridad OPUESTA -> daño (multiplicado).
/// </summary>
[AddComponentMenu("Ship/Health")]
public class ShipHealth : MonoBehaviour
{
    [SerializeField] private ShipPolarityController polarityController;

    [Header("Vida")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Mecánica Ikaruga")]
    [Tooltip("Multiplicador de daño al recibir un disparo de polaridad opuesta. Súbelo mucho (ej. 999) si quieres que sea 'un golpe = muerte' como en el juego original.")]
    public float oppositeDamageMultiplier = 2f;
    public float maxEnergy = 100f;
    public float energyPerAbsorb = 10f;
    public float currentEnergy;

    [Header("Eventos (para UI, VFX, cámara shake, etc.)")]
    public UnityEvent onDamaged;
    public UnityEvent onAbsorbed;
    public UnityEvent onDeath;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    /// <summary>Llamado por Projectile cuando impacta esta nave.</summary>
    public void ReceiveHit(ShipPolarity projectilePolarity, float baseDamage)
    {
        bool sameColor = polarityController != null && projectilePolarity == polarityController.CurrentPolarity;

        if (sameColor)
            Absorb(baseDamage);
        else
            TakeDamage(baseDamage * oppositeDamageMultiplier);
    }

    private void Absorb(float amount)
    {
        currentEnergy = Mathf.Min(maxEnergy, currentEnergy + energyPerAbsorb);
        onAbsorbed?.Invoke();
    }

    private void TakeDamage(float amount)
    {
        currentHealth -= amount;
        onDamaged?.Invoke();

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            onDeath?.Invoke();
        }
    }
}
