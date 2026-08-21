using UnityEngine;

/// <summary>
/// Controlador de nave estilo Star Fox.
/// La nave avanza automáticamente (modo "on-rails") y el jugador
/// controla el desplazamiento lateral/vertical dentro de límites,
/// con inclinación visual (roll/pitch/yaw), barrel roll y boost/freno.
///
/// Requiere un Rigidbody en el mismo GameObject (puede ser Kinematic o no,
/// ver comentarios abajo).
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class StarFoxShipController : MonoBehaviour
{
    [Header("Movimiento hacia adelante")]
    [Tooltip("Velocidad base de avance (modo on-rails).")]
    public float forwardSpeed = 20f;
    [Tooltip("Velocidad al hacer boost.")]
    public float boostSpeed = 35f;
    [Tooltip("Velocidad al frenar.")]
    public float brakeSpeed = 8f;
    [Tooltip("Qué tan rápido se acelera/desacelera entre velocidades.")]
    public float speedLerpRate = 4f;

    [Header("Movimiento lateral / vertical")]
    [Tooltip("Velocidad de desplazamiento lateral (eje X local).")]
    public float strafeSpeed = 12f;
    [Tooltip("Velocidad de desplazamiento vertical (eje Y local).")]
    public float verticalSpeed = 12f;
    [Tooltip("Límite máximo de desplazamiento respecto al centro del riel.")]
    public Vector2 movementLimits = new Vector2(8f, 5f); // x: horizontal, y: vertical

    [Header("Inclinación visual (roll/pitch/yaw)")]
    public float maxRollAngle = 45f;
    public float maxPitchAngle = 25f;
    public float maxYawAngle = 15f;
    public float rotationLerpSpeed = 6f;

    [Header("Barrel Roll")]
    public float barrelRollDuration = 0.5f;
    public float doubleTapWindow = 0.3f;
    private bool isBarrelRolling = false;
    private float barrelRollTimer = 0f;
    private float barrelRollDirection = 1f;
    private float lastTapTimeLeft = -10f;
    private float lastTapTimeRight = -10f;

    [Header("Disparo")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 0.2f;
    private float fireCooldown = 0f;

    [Header("Modo de movimiento")]
    [Tooltip("Si está activo, la nave se mueve libremente en el eje forward " +
             "(útil para modo 'all-range' tipo Star Fox 64). Si está desactivado, " +
             "solo se mueve en un riel fijo (transform.forward constante).")]
    public bool useTransformForward = false;

    private Rigidbody rb;
    private Vector2 currentOffset; // posición actual respecto al centro del riel
    private float currentForwardSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true; // Movemos manualmente vía MovePosition/MoveRotation
        currentForwardSpeed = forwardSpeed;
    }

    void Update()
    {
        HandleBarrelRollInput();
        HandleShooting();
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal"); // A/D o flechas o joystick
        float v = Input.GetAxis("Vertical");   // W/S (lo usamos para pitch, no para avance)

        HandleForwardSpeed();
        HandleLateralMovement(h, v);
        HandleRotation(h, v);

        if (isBarrelRolling)
            UpdateBarrelRoll();
    }

    void HandleForwardSpeed()
    {
        float targetSpeed = forwardSpeed;

        if (Input.GetButton("Fire3")) // Ej: Shift para boost (configurar en Input Manager)
            targetSpeed = boostSpeed;
        else if (Input.GetButton("Fire2")) // Ej: Ctrl para frenar
            targetSpeed = brakeSpeed;

        currentForwardSpeed = Mathf.Lerp(currentForwardSpeed, targetSpeed, Time.fixedDeltaTime * speedLerpRate);

        Vector3 forwardDir = useTransformForward ? transform.forward : Vector3.forward;
        rb.MovePosition(rb.position + forwardDir * currentForwardSpeed * Time.fixedDeltaTime);
    }

    void HandleLateralMovement(float h, float v)
    {
        // Actualizamos el offset dentro de los límites del "riel"
        currentOffset.x += h * strafeSpeed * Time.fixedDeltaTime;
        currentOffset.y += v * verticalSpeed * Time.fixedDeltaTime;

        currentOffset.x = Mathf.Clamp(currentOffset.x, -movementLimits.x, movementLimits.x);
        currentOffset.y = Mathf.Clamp(currentOffset.y, -movementLimits.y, movementLimits.y);

        // Movemos en los ejes local right/up respecto a la orientación base (sin la inclinación visual)
        Vector3 right = useTransformForward ? transform.right : Vector3.right;
        Vector3 up = useTransformForward ? transform.up : Vector3.up;

        Vector3 desiredLateralVelocity = (right * h * strafeSpeed) + (up * v * verticalSpeed);

        // Si estamos en los límites, evitamos que siga acumulando desplazamiento infinito
        if (Mathf.Abs(currentOffset.x) >= movementLimits.x) desiredLateralVelocity.x = 0;
        if (Mathf.Abs(currentOffset.y) >= movementLimits.y) desiredLateralVelocity.y = 0;

        rb.MovePosition(rb.position + desiredLateralVelocity * Time.fixedDeltaTime);
    }

    void HandleRotation(float h, float v)
    {
        float targetRoll = -h * maxRollAngle;
        float targetPitch = v * maxPitchAngle;
        float targetYaw = h * maxYawAngle;

        if (isBarrelRolling)
        {
            // Durante el barrel roll, el roll lo controla la animación, no el input
            float rollProgress = barrelRollTimer / barrelRollDuration;
            targetRoll = barrelRollDirection * 360f * rollProgress;
        }

        Quaternion targetRotation = Quaternion.Euler(targetPitch, targetYaw, targetRoll);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * rotationLerpSpeed));
    }

    void HandleBarrelRollInput()
    {
        if (isBarrelRolling) return;

        // Doble tap izquierda
        if (Input.GetButtonDown("Horizontal") || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (Time.time - lastTapTimeLeft < doubleTapWindow)
            {
                StartBarrelRoll(-1f);
            }
            lastTapTimeLeft = Time.time;
        }

        // Doble tap derecha
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (Time.time - lastTapTimeRight < doubleTapWindow)
            {
                StartBarrelRoll(1f);
            }
            lastTapTimeRight = Time.time;
        }
    }

    void StartBarrelRoll(float direction)
    {
        isBarrelRolling = true;
        barrelRollTimer = 0f;
        barrelRollDirection = direction;
    }

    void UpdateBarrelRoll()
    {
        barrelRollTimer += Time.fixedDeltaTime;
        if (barrelRollTimer >= barrelRollDuration)
        {
            isBarrelRolling = false;
            barrelRollTimer = 0f;
        }
    }

    void HandleShooting()
    {
        fireCooldown -= Time.deltaTime;

        if (Input.GetButton("Fire1") && fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = fireRate;
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody projRb = proj.GetComponent<Rigidbody>();
        if (projRb != null)
        {
            Vector3 shootDir = useTransformForward ? transform.forward : Vector3.forward;
            projRb.linearVelocity = shootDir * 60f; // ajusta velocidad de disparo
        }
    }

    // Útil para debug: dibuja los límites del riel en el editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = transform.position - transform.forward * 0f; // referencia visual
        Gizmos.DrawWireCube(center, new Vector3(movementLimits.x * 2, movementLimits.y * 2, 0.1f));
    }
}