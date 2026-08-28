using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LineaDialogo
{
    [TextArea(3, 5)]
    public string texto;
    public float duracion = 4f;
}



[CreateAssetMenu(fileName = "Nuevo NPC", menuName = "Sistema Dialogo/NPC Data")]
public class NPCData : ScriptableObject
{
    public string nombreNPC;
    public Sprite imageNPC;
    public Sprite fondoImagenNPC;
    public Sprite fondoTexto;

    public List<LineaDialogo> dialogos;
}
