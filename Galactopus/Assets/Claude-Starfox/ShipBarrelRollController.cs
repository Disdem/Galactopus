using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Maneja únicamente el estado del barrel roll (activo/inactivo, ángulo actual).
/// No mueve ni rota nada por sí mismo: expone su estado (IsRolling, CurrentRollAngle)
/// para que ShipTiltController lo consuma, y dispara eventos para que
/// efectos visuales/sonoros/invulnerabilidad se conecten sin acoplarse al código.
/// </summary>
[AddComponentMenu("Ship/Barrel Roll Controller")]
public class ShipBarrelRollController : MonoBehaviour
{
    [SerializeField] private MonoBehaviour inputProviderSource;

    public float duration = 0.5f;

    [Header("Eventos (para VFX, SFX, invulnerabilidad temporal, etc.)")]
    public UnityEvent onBarrelRollStart;
    public UnityEvent onBarrelRollEnd;

    public bool IsRolling { get; private set; }
    public float CurrentRollAngle { get; private set; }

    private IShipInputProvider input;
    private float timer;
    private float direction;

    void Awake()
    {
        input = inputProviderSource as IShipInputProvider ?? GetComponent<IShipInputProvider>();
    }

    void Update()
    {
        if (input == null) return;

        if (!IsRolling)
        {
            if (input.BarrelRollLeftTriggered) StartRoll(-1f);
            else if (input.BarrelRollRightTriggered) StartRoll(1f);
        }
        else
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / duration);
            CurrentRollAngle = direction * 360f * progress;

            if (progress >= 1f)
            {
                IsRolling = false;
                CurrentRollAngle = 0f;
                onBarrelRollEnd?.Invoke();
            }
        }
    }

    private void StartRoll(float dir)
    {
        IsRolling = true;
        timer = 0f;
        direction = dir;
        onBarrelRollStart?.Invoke();
    }
}
