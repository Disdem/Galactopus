using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LineaDialogo
{
    [TextArea(3, 5)]
    public string texto;
    public float duracion = 4f;

    [Header("Activar otra UI")]
    public Sprite iconoTutorial;
}



[CreateAssetMenu(fileName = "Nuevo NPC", menuName = "Sistema Dialogo/NPC Data")]
public class NPCData : ScriptableObject
{
    public string nombreNPC;
    public Sprite imageNPC;
    public Sprite fondoImagenNPC;
    public Sprite fondoTexto;

    [Header("Efecto de Voz (Tipo Undertale)")]
    public AudioClip sonidoVoz; 
    [Range(0.5f, 1.5f)] public float pitchVoz = 1f;

    public List<LineaDialogo> dialogos;
}
