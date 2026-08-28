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

    [Header("Ajustes")]
    [SerializeField] private float duracionEnPantalla = 4f;

    private Dictionary<NPCData, int> indiceDialogos = new Dictionary<NPCData, int>();
    private Coroutine corrutionaDcultar;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        panelDialogo.SetActive(false);
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

            indiceDialogos[npc]++;
            panelDialogo.SetActive(true);

            if (corrutionaDcultar != null) StopCoroutine(corrutionaDcultar);
            corrutionaDcultar = StartCoroutine(OcultarDialogoTardado(dialogoActual.duracion));
        }
    }

    private IEnumerator OcultarDialogoTardado(float duracion)
    {
        yield return new WaitForSeconds(duracion);
        panelDialogo.SetActive(false);
    }
}
