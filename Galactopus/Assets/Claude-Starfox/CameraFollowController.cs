using UnityEngine;

/// <summary>
/// Cámara de persecución con suavizado (SmoothDamp) y "look-ahead":
/// se desplaza un poco en la dirección en la que la nave se mueve lateralmente,
/// dando esa sensación de deriva característica de Star Fox. Totalmente
/// independiente de la lógica de la nave: solo lee propiedades públicas.
/// </summary>
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Ship/Camera Follow Controller")]
public class CameraFollowController : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform target;
    [Tooltip("Opcional: si se asigna, la cámara hace look-ahead según el offset lateral de la nave.")]
    public ShipMotorController targetMotor;

    [Header("Offset base (espacio local del objetivo)")]
    public Vector3 offset = new Vector3(0f, 3f, -8f);

    [Header("Suavizado")]
    public float positionSmoothTime = 0.2f;
    public float rotationLerpSpeed = 5f;

    [Header("Look-ahead")]
    public float lookAheadFactor = 0.5f;

    [Header("Efecto de FOV al hacer boost")]
    public float baseFOV = 60f;
    public float boostFOV = 70f;
    public float fovLerpSpeed = 4f;

    private Vector3 velocity;
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + target.TransformDirection(offset);

        if (targetMotor != null)
        {
            Vector2 lookAheadOffset = targetMotor.CurrentOffset * lookAheadFactor;
            desiredPosition += target.right * lookAheadOffset.x + target.up * lookAheadOffset.y;
        }

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, positionSmoothTime);

        Vector3 lookTarget = target.position + target.forward * 10f;
        Quaternion desiredRotation = Quaternion.LookRotation(lookTarget - transform.position, target.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationLerpSpeed);

        if (targetMotor != null)
        {
            bool isBoosting = targetMotor.CurrentForwardSpeed > targetMotor.forwardSpeed + 0.1f;
            float targetFOV = isBoosting ? boostFOV : baseFOV;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * fovLerpSpeed);
        }
    }
}
