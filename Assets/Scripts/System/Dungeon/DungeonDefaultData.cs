using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "Dungeon", menuName = "Scriptable Object/DungeonDefaultData", order = int.MaxValue)]
public class DungeonDefaultData : ScriptableObject
{
    public Sprite ActiveNode, DeActiveNode;
    public GameObject weightText;
    public Material flashWhite;
}
