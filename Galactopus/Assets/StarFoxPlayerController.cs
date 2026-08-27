using UnityEngine;

public class StarFoxPlayerController : MonoBehaviour
{
    [Header("Movement Limits")]
    [SerializeField] private float xLimit = 8f;
    [SerializeField] private float yLimit = 4.5f;

    [Header("Speed & Sensitivity")]
    [SerializeField] private float movementSpeed = 15f;

    [Header("Rotation Settings")]
    [SerializeField] private Transform playerModel;
    [SerializeField] private float pitchFactor = -2f;
    [SerializeField] private float yawFactor = 2f;
    [SerializeField] private float rollFactor = -3f;
    [SerializeField] private float rotationSmoothTime = 10f;

    private float xOffset;
    private float yOffset;

    void Update()
    {
        ProcessTranslation();
        ProcessRotation();
    }

    private void ProcessTranslation()
    { 
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");


        xOffset = xInput * movementSpeed * Time.deltaTime;
        yOffset = yInput * movementSpeed * Time.deltaTime;

        float rawXPos = transform.localPosition.x + xOffset;
        float clampedXPos = Mathf.Clamp(rawXPos, -xLimit, xLimit);

        float rawYPos = transform.localPosition.y + yOffset;
        float clampedYPos = Mathf.Clamp(rawYPos, -yLimit, yLimit);

        transform.localPosition = new Vector3(clampedXPos, clampedYPos, transform.localPosition.z);
    }

    private void ProcessRotation()
    {

        float pitchDueToPosition = transform.localPosition.y * pitchFactor;
        float pitchDueToControl = Input.GetAxis("Vertical") * pitchFactor;
        float pitch = pitchDueToPosition + pitchDueToControl;

        float yaw = transform.localPosition.x * yawFactor;

        float roll = Input.GetAxis("Horizontal") * rollFactor;

        Quaternion targetRotation = Quaternion.Euler(pitch, yaw, roll);
        playerModel.localRotation = Quaternion.Slerp(
            playerModel.localRotation,
            targetRotation,
            Time.deltaTime * rotationSmoothTime
        );
    }
}