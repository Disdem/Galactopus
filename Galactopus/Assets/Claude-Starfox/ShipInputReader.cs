using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Única fuente de verdad para el input del jugador. Soporta teclado
/// y gamepad simultáneamente sin que el resto del código sepa la diferencia.
///
/// Requiere el paquete "Input System" (Window > Package Manager > Input System)
/// y que en Project Settings > Player > Active Input Handling esté en
/// "Input System Package (New)" o "Both".
/// </summary>

public class ShipInputReader : MonoBehaviour, IShipInputProvider
{
    [Header("Barrel Roll (doble tap en teclado)")]
    [Tooltip("Ventana de tiempo en segundos para detectar doble tap en A/D.")]
    [SerializeField] private float doubleTapWindow = 0.3f;

    // Acciones de input, creadas en código para no depender de un asset externo.
    private InputAction moveAction;
    private InputAction keyLeftAction;
    private InputAction keyRightAction;
    private InputAction fireAction;
    private InputAction boostAction;
    private InputAction brakeAction;
    private InputAction gamepadRollLeftAction;
    private InputAction gamepadRollRightAction;
    private InputAction polarityAction;

    private float lastLeftTapTime = -10f;
    private float lastRightTapTime = -10f;

    public Vector2 MoveInput { get; private set; }
    public bool FireHeld { get; private set; }
    public bool FirePressedThisFrame { get; private set; }
    public bool BoostHeld { get; private set; }
    public bool BrakeHeld { get; private set; }
    public bool BarrelRollLeftTriggered { get; private set; }
    public bool BarrelRollRightTriggered { get; private set; }
    public bool PolaritySwitchTriggered { get; private set; }

    void Awake()
    {
        // Movimiento: WASD compuesto + stick izquierdo del gamepad
        moveAction = new InputAction("Move", InputActionType.Value, expectedControlType: "Vector2");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        moveAction.AddBinding("<Gamepad>/leftStick");

        // Se usan aparte para detectar doble tap en teclado sin ensuciar moveAction
        keyLeftAction = new InputAction("KeyLeft", binding: "<Keyboard>/a");
        keyRightAction = new InputAction("KeyRight", binding: "<Keyboard>/d");

        fireAction = new InputAction("Fire", InputActionType.Button);
        fireAction.AddBinding("<Keyboard>/space");
        fireAction.AddBinding("<Mouse>/leftButton");
        fireAction.AddBinding("<Gamepad>/buttonSouth"); // A (Xbox) / X (PlayStation)

        boostAction = new InputAction("Boost", InputActionType.Button);
        boostAction.AddBinding("<Keyboard>/leftShift");
        boostAction.AddBinding("<Gamepad>/rightTrigger");

        brakeAction = new InputAction("Brake", InputActionType.Button);
        brakeAction.AddBinding("<Keyboard>/leftCtrl");
        brakeAction.AddBinding("<Gamepad>/leftTrigger");

        // En gamepad el barrel roll es un botón dedicado (bumpers), no doble tap
        gamepadRollLeftAction = new InputAction("GamepadRollLeft", binding: "<Gamepad>/leftShoulder");
        gamepadRollRightAction = new InputAction("GamepadRollRight", binding: "<Gamepad>/rightShoulder");

        polarityAction = new InputAction("PolaritySwitch", InputActionType.Button);
        polarityAction.AddBinding("<Keyboard>/e");
        polarityAction.AddBinding("<Gamepad>/buttonWest"); // X (Xbox) / Cuadrado (PlayStation)
    }

    void OnEnable()
    {
        moveAction.Enable();
        keyLeftAction.Enable();
        keyRightAction.Enable();
        fireAction.Enable();
        boostAction.Enable();
        brakeAction.Enable();
        gamepadRollLeftAction.Enable();
        gamepadRollRightAction.Enable();
        polarityAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        keyLeftAction.Disable();
        keyRightAction.Disable();
        fireAction.Disable();
        boostAction.Disable();
        brakeAction.Disable();
        gamepadRollLeftAction.Disable();
        gamepadRollRightAction.Disable();
        polarityAction.Disable();
    }

    void OnDestroy()
    {
        moveAction.Dispose();
        keyLeftAction.Dispose();
        keyRightAction.Dispose();
        fireAction.Dispose();
        boostAction.Dispose();
        brakeAction.Dispose();
        gamepadRollLeftAction.Dispose();
        gamepadRollRightAction.Dispose();
        polarityAction.Dispose();
    }

    void Update()
    {
        MoveInput = moveAction.ReadValue<Vector2>();
        FireHeld = fireAction.IsPressed();
        FirePressedThisFrame = fireAction.WasPressedThisFrame();
        BoostHeld = boostAction.IsPressed();
        BrakeHeld = brakeAction.IsPressed();
        PolaritySwitchTriggered = polarityAction.WasPressedThisFrame();

        BarrelRollLeftTriggered = gamepadRollLeftAction.WasPressedThisFrame()
            || CheckDoubleTap(keyLeftAction, ref lastLeftTapTime);

        BarrelRollRightTriggered = gamepadRollRightAction.WasPressedThisFrame()
            || CheckDoubleTap(keyRightAction, ref lastRightTapTime);
    }

    private bool CheckDoubleTap(InputAction action, ref float lastTapTime)
    {
        if (!action.WasPressedThisFrame()) return false;

        bool isDoubleTap = (Time.unscaledTime - lastTapTime) <= doubleTapWindow;
        lastTapTime = Time.unscaledTime;
        return isDoubleTap;
    }
}
