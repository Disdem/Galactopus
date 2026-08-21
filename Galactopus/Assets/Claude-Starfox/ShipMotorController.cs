using UnityEngine;

/// <summary>
/// Se encarga ÚNICAMENTE del movimiento físico de la nave: avance
/// automático + desplazamiento lateral/vertical dentro de límites.
/// No sabe nada de rotación visual, disparo, ni polaridad.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[AddComponentMenu("Ship/Motor Controller")]
public class ShipMotorController : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Componente que implementa IShipInputProvider. Si se deja vacío, se busca en este GameObject.")]
    [SerializeField] private MonoBehaviour inputProviderSource;

    [Header("Velocidad de avance")]
    public float forwardSpeed = 20f;
    public float boostSpeed = 35f;
    public float brakeSpeed = 8f;
    public float speedLerpRate = 4f;

    [Header("Movimiento lateral / vertical")]
    public float strafeSpeed = 12f;
    public float verticalSpeed = 12f;
    public Vector2 movementLimits = new Vector2(8f, 5f);

    public enum MovementMode
    {
        WorldForward,   // avanza en Vector3.forward del mundo (riel recto)
        TransformForward, // avanza según transform.forward (modo libre / all-range)
        RailSpline      // avanza sobre un RailPath curvo
    }

    [Header("Modo de movimiento")]
    public MovementMode movementMode = MovementMode.WorldForward;

    [Header("Riel curvo (solo si movementMode = RailSpline)")]
    public RailPath rail;
    public bool loopRail = false;

    /// <summary>Offset actual respecto al centro del riel. Lo usa la cámara para el look-ahead.</summary>
    public Vector2 CurrentOffset { get; private set; }
    public float CurrentForwardSpeed { get; private set; }
    /// <summary>Distancia total recorrida sobre el riel (solo relevante en modo RailSpline).</summary>
    public float DistanceTraveled { get; private set; }
    /// <summary>
    /// Orientación "base" que debe tener la nave según el modo de movimiento actual
    /// (en RailSpline, alineada a la curva). ShipTiltController combina esto con la
    /// inclinación visual (pitch/yaw/roll) para no perder la orientación del riel.
    /// </summary>
    public Quaternion BaseRotation { get; private set; } = Quaternion.identity;

    private Rigidbody rb;
    private IShipInputProvider input;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        input = inputProviderSource as IShipInputProvider ?? GetComponent<IShipInputProvider>();
        CurrentForwardSpeed = forwardSpeed;
    }

    void FixedUpdate()
    {
        if (input == null) return;

        UpdateForwardSpeed();

        if (movementMode == MovementMode.RailSpline && rail != null)
            UpdateRailMovement(input.MoveInput);
        else
            UpdateLateralMovement(input.MoveInput);
    }

    private void UpdateForwardSpeed()
    {
        float targetSpeed = forwardSpeed;
        if (input.BoostHeld) targetSpeed = boostSpeed;
        else if (input.BrakeHeld) targetSpeed = brakeSpeed;

        CurrentForwardSpeed = Mathf.Lerp(CurrentForwardSpeed, targetSpeed, Time.fixedDeltaTime * speedLerpRate);

        // En modo RailSpline, el avance lo maneja UpdateRailMovement (necesita la
        // distancia acumulada), así que aquí solo actualizamos la velocidad para
        // los otros dos modos.
        if (movementMode == MovementMode.RailSpline) return;

        Vector3 forwardDir = movementMode == MovementMode.TransformForward ? transform.forward : Vector3.forward;
        rb.MovePosition(rb.position + forwardDir * CurrentForwardSpeed * Time.fixedDeltaTime);
    }

    private void UpdateLateralMovement(Vector2 moveInput)
    {
        Vector2 newOffset = CurrentOffset + moveInput * new Vector2(strafeSpeed, verticalSpeed) * Time.fixedDeltaTime;
        newOffset.x = Mathf.Clamp(newOffset.x, -movementLimits.x, movementLimits.x);
        newOffset.y = Mathf.Clamp(newOffset.y, -movementLimits.y, movementLimits.y);

        Vector2 delta = newOffset - CurrentOffset;
        CurrentOffset = newOffset;

        Vector3 right = movementMode == MovementMode.TransformForward ? transform.right : Vector3.right;
        Vector3 up = movementMode == MovementMode.TransformForward ? transform.up : Vector3.up;

        rb.MovePosition(rb.position + right * delta.x + up * delta.y);
    }

    private void UpdateRailMovement(Vector2 moveInput)
    {
        // Avance: acumulamos distancia sobre la curva en vez de mover en línea recta.
        DistanceTraveled += CurrentForwardSpeed * Time.fixedDeltaTime;
        RailPath.RailSample sample = rail.EvaluateAtDistance(DistanceTraveled, loopRail);

        // Lateral: igual que antes, pero proyectado sobre right/up DEL RIEL (no del mundo).
        Vector2 newOffset = CurrentOffset + moveInput * new Vector2(strafeSpeed, verticalSpeed) * Time.fixedDeltaTime;
        newOffset.x = Mathf.Clamp(newOffset.x, -movementLimits.x, movementLimits.x);
        newOffset.y = Mathf.Clamp(newOffset.y, -movementLimits.y, movementLimits.y);
        CurrentOffset = newOffset;

        Vector3 targetPosition = sample.position + sample.right * CurrentOffset.x + sample.up * CurrentOffset.y;
        rb.MovePosition(targetPosition);

        // Guardamos la orientación base (alineada a la curva) para que
        // ShipTiltController le sume la inclinación visual encima.
        BaseRotation = Quaternion.LookRotation(sample.forward, sample.up);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(movementLimits.x * 2, movementLimits.y * 2, 0.1f));
    }
}
