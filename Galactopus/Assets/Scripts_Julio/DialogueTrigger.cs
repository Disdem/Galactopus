using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private NPCData personaje;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DIalogueManager.Instance.MostrarSiguienteDialogo(personaje);
            gameObject.SetActive(false);


        }
    }
}
