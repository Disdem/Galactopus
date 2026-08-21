using UnityEngine;

/// <summary>
/// Se encarga SOLO de la rotación visual de la nave según el input
/// (efecto Star Fox de inclinarse al moverse). Si hay un barrel roll
/// activo, le cede el control del roll a ese sistema.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[AddComponentMenu("Ship/Tilt Controller")]
public class ShipTiltController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private MonoBehaviour inputProviderSource;
    [Tooltip("Opcional: si la nave tiene barrel roll, arrástralo aquí para que tome prioridad sobre el roll normal.")]
    [SerializeField] private ShipBarrelRollController barrelRoll;
    [Tooltip("Opcional: si se asigna, se usa su BaseRotation (orientación del riel) como base antes de aplicar la inclinación. Si se deja vacío, la base es Quaternion.identity (como antes).")]
    [SerializeField] private ShipMotorController motor;

    [Header("Ángulos máximos")]
    public float maxRollAngle = 45f;
    public float maxPitchAngle = 25f;
    public float maxYawAngle = 15f;
    public float rotationLerpSpeed = 6f;

    private IShipInputProvider input;
    private Rigidbody rb;

    void Awake()
    {
        input = inputProviderSource as IShipInputProvider ?? GetComponent<IShipInputProvider>();
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (input == null) return;

        Vector2 move = input.MoveInput;
        float targetRoll = -move.x * maxRollAngle;
        float targetPitch = move.y * maxPitchAngle;
        float targetYaw = move.x * maxYawAngle;

        if (barrelRoll != null && barrelRoll.IsRolling)
            targetRoll = barrelRoll.CurrentRollAngle;

        Quaternion baseRotation = motor != null ? motor.BaseRotation : Quaternion.identity;
        Quaternion localTilt = Quaternion.Euler(targetPitch, targetYaw, targetRoll);
        Quaternion targetRotation = baseRotation * localTilt;

        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * rotationLerpSpeed));
    }
}
