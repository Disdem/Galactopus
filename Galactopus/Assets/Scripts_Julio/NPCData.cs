using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Nuevo NPC", menuName = "Sistema Dialogo/NPC Data")]
public class NPCData : ScriptableObject
{
    public string nombreNPC;
    public Sprite imageNPC;
    public Sprite fondoImagenNPC;
    public Sprite fondoTexto;

    [TextArea(3, 5)]
    public List<string> dialogos;
}
