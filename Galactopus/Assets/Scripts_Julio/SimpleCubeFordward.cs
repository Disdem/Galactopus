using UnityEngine;

public class SimpleCubeFordward : MonoBehaviour
{
    [Header("Control de velocidad")]
    [Tooltip("Se ajusta la velocidad del cubo")]
    public float Velocidad = 5f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Velocidad * Time.deltaTime);
    }
}
