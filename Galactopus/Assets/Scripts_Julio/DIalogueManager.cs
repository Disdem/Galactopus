using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class DIalogueManager : MonoBehaviour
{
    public static DIalogueManager Instance;

    [Header("Contenedor principal")]
    [SerializeField] private GameObject panelDialogo;

    [Header("Componentes visuales")]
    [SerializeField] private Image uiImagenNPC;
    [SerializeField] private Image uiFondoImagenNPC;
    [SerializeField] private TextMeshProUGUI uiNombreNPC;
    [SerializeField] private TextMeshProUGUI uiTextoDialogo;
    [SerializeField] private Image uiFondoTexto;

    [Header("UI adicional")]
    [SerializeField] private GameObject panelIconoTutorial;
    [SerializeField] private Image uiIconoTutorial;

    [Header("Efecto Maquina de Escribir")]
    [SerializeField] private float velocidadEscritura = 0.03f; 
    [SerializeField] private AudioSource audioSource;

    [Header("Ajustes")]
    [SerializeField] private float duracionEnPantalla = 4f;

    private Dictionary<NPCData, int> indiceDialogos = new Dictionary<NPCData, int>();
    private Coroutine corrutinaAnimacion;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        panelDialogo.SetActive(false);

        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    public void MostrarSiguienteDialogo (NPCData npc)
    {
        if (npc == null || npc.dialogos.Count == 0) return;

        if (!indiceDialogos.ContainsKey(npc))
        {
            indiceDialogos.Add(npc, 0);
        }

        int indiceActual = indiceDialogos[npc];

        if (indiceActual < npc.dialogos.Count)
        {
            LineaDialogo dialogoActual = npc.dialogos[indiceActual];

            uiImagenNPC.sprite = npc.imageNPC;
            uiFondoImagenNPC.sprite = npc.fondoImagenNPC;
            uiNombreNPC.text = npc.nombreNPC;
            uiTextoDialogo.text = dialogoActual.texto;
            uiFondoTexto.sprite = npc.fondoTexto;

            if (dialogoActual.iconoTutorial != null && panelIconoTutorial != null && uiIconoTutorial != null)
            {
                uiIconoTutorial.sprite = dialogoActual.iconoTutorial;
                panelIconoTutorial.SetActive(true);
            }
            else if (panelIconoTutorial != null)
            {
                panelIconoTutorial.SetActive(false);
            }

            indiceDialogos[npc]++;
            panelDialogo.SetActive(true);

            if (corrutinaAnimacion != null) StopCoroutine(corrutinaAnimacion);
            corrutinaAnimacion = StartCoroutine(EscribirTextoConSonido(dialogoActual, npc));
        }
    }

    private IEnumerator EscribirTextoConSonido(LineaDialogo dialogo, NPCData npc)
    {
        uiTextoDialogo.text = ""; 

        if (audioSource != null && npc.sonidoVoz != null)
        {
            audioSource.pitch = npc.pitchVoz; 
        }

        foreach (char letra in dialogo.texto.ToCharArray())
        {
            uiTextoDialogo.text += letra; 
            if (letra != ' ' && audioSource != null && npc.sonidoVoz != null)
            {
                audioSource.PlayOneShot(npc.sonidoVoz);
            }

            yield return new WaitForSeconds(velocidadEscritura);
        }

        yield return new WaitForSeconds(dialogo.duracion);
        panelDialogo.SetActive(false);
        if (panelIconoTutorial != null) panelIconoTutorial.SetActive(false);
    }
}
