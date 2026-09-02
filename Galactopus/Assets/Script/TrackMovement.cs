using UnityEngine;

public class TrackMovement : MonoBehaviour
{
    [SerializeField] private float forwardSpeed = 20f;

    void Update()
    {
        // Avance constante en el eje Z global o local
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
    }
}