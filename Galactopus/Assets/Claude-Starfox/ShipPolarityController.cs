using UnityEngine;
using UnityEngine.Events;

public enum ShipPolarity { White, Black }

/// <summary>
/// UnityEvent tipado y serializable para poder conectarlo desde el Inspector
/// (UnityEvent&lt;T&gt; genérico "a secas" no se ve en el Inspector, por eso
/// se necesita esta subclase concreta).
/// </summary>
[System.Serializable]
public class PolarityChangedEvent : UnityEvent<ShipPolarity> { }

/// <summary>
/// Mecánica de polaridad estilo Ikaruga: la nave alterna entre Blanco/Negro.
/// - Los disparos propios salen con la polaridad actual (ver ShipWeaponController).
/// - Al recibir un disparo de la MISMA polaridad, la nave lo absorbe (ver ShipHealth).
/// - Al recibir un disparo de polaridad OPUESTA, recibe daño extra.
///
/// Este script solo administra el ESTADO y los efectos visuales/sonoros del
/// cambio; la lógica de daño/absorción vive en ShipHealth para no mezclar
/// responsabilidades.
/// </summary>
[AddComponentMenu("Ship/Polarity Controller")]
public class ShipPolarityController : MonoBehaviour
{
    [SerializeField] private MonoBehaviour inputProviderSource;

    [Header("Visuales")]
    [Tooltip("Renderer de la nave al que se le cambiará el color según la polaridad.")]
    public Renderer shipRenderer;
    public Color whiteColor = Color.white;
    public Color blackColor = Color.black;
    [Tooltip("Objetos hijos con partículas/efectos para cada polaridad (opcional).")]
    public GameObject whiteVFX;
    public GameObject blackVFX;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip switchToWhiteClip;
    public AudioClip switchToBlackClip;

    [Header("Cooldown")]
    [Tooltip("Evita que se pueda cambiar de polaridad más rápido de lo permitido (anti-spam).")]
    public float switchCooldown = 0.15f;

    public ShipPolarity CurrentPolarity { get; private set; } = ShipPolarity.White;

    [Header("Eventos")]
    public PolarityChangedEvent onPolarityChanged;

    private IShipInputProvider input;
    private MaterialPropertyBlock propBlock;
    private float cooldownTimer;

    void Awake()
    {
        input = inputProviderSource as IShipInputProvider ?? GetComponent<IShipInputProvider>();
        propBlock = new MaterialPropertyBlock();
        ApplyVisuals();
    }

    void Update()
    {
        if (input == null) return;

        cooldownTimer -= Time.deltaTime;

        if (input.PolaritySwitchTriggered && cooldownTimer <= 0f)
        {
            SwitchPolarity();
            cooldownTimer = switchCooldown;
        }
    }

    /// <summary>Público para poder forzar el cambio desde otros sistemas (power-ups, tutoriales, etc.).</summary>
    public void SwitchPolarity()
    {
        CurrentPolarity = CurrentPolarity == ShipPolarity.White ? ShipPolarity.Black : ShipPolarity.White;
        ApplyVisuals();
        PlaySwitchSound();
        onPolarityChanged?.Invoke(CurrentPolarity);
    }

    private void ApplyVisuals()
    {
        if (shipRenderer != null)
        {
            propBlock.SetColor("_Color", CurrentPolarity == ShipPolarity.White ? whiteColor : blackColor);
            // Si usas URP/HDRP con Lit shader, probablemente necesites "_BaseColor" en vez de "_Color".
            propBlock.SetColor("_BaseColor", CurrentPolarity == ShipPolarity.White ? whiteColor : blackColor);
            shipRenderer.SetPropertyBlock(propBlock);
        }

        if (whiteVFX != null) whiteVFX.SetActive(CurrentPolarity == ShipPolarity.White);
        if (blackVFX != null) blackVFX.SetActive(CurrentPolarity == ShipPolarity.Black);
    }

    private void PlaySwitchSound()
    {
        if (audioSource == null) return;
        AudioClip clip = CurrentPolarity == ShipPolarity.White ? switchToWhiteClip : switchToBlackClip;
        if (clip != null) audioSource.PlayOneShot(clip);
    }
}
