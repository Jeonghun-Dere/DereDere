using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public struct MonsterData {
    public EntityBase monster;
    public Vector2 spawnOffset;
    public int count, MaxCount;

}

public class NodeData : MonoBehaviour
{
    public DungeonDefaultData defaultData;
    public float startDelay;
    public int weight;
    public int layer;
    public Vector2 center;
    public Vector2 size;
    public List<MonsterData> monsters;
    public SpriteRenderer nodeIcon;
    [HideInInspector]
    public NodeData head;
    Text weightTxt;
    public List<NodeData> Child;
    public Material defMat;

    bool activated;

    void Start() {
        nodeIcon.gameObject.SetActive(true);
        DungeonController.Instance.nodes.Add(this);

        var txt = Instantiate(defaultData.weightText);
        txt.transform.SetParent(nodeIcon.transform);
        txt.transform.localPosition = Vector2.zero;

        weightTxt = txt.GetComponentInChildren<Text>();

        defMat = nodeIcon.material;

        foreach (var child in Child) {
            child.head = this;
        }
    }

    public virtual void OnEventStart(){}
    public virtual void OnEventEnd(){}
    public virtual void OnEventUpdate(){}

    void Update() {
        if (Player.Local != null) {
            var min = (Vector2)transform.position + center - size;
            var max = (Vector2)transform.position + center + size;

            if (Player.Local.transform.position.x > min.x && Player.Local.transform.position.y > min.y &&
                Player.Local.transform.position.x < max.x && Player.Local.transform.position.y < max.y) {
                    nodeIcon.sprite = defaultData.ActiveNode;

                    activated = true;
            } else {
                nodeIcon.sprite = defaultData.DeActiveNode;

                activated = false;
            }

            if (activated) {
                if (Child.Count >= 2) {
                    if (Player.Local.weight >= weight) {
                        Child[1].nodeIcon.material = defaultData.flashWhite;
                        Child[0].nodeIcon.material = defMat;
                    } else {
                        Child[1].nodeIcon.material = defMat;
                        Child[0].nodeIcon.material = defaultData.flashWhite;
                    }
                }
            }
        }

        weightTxt.text = weight.ToString();
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + center, size);

        Gizmos.color = Color.yellow;

        #if UNITY_EDITOR
        UnityEditor.Handles.color = Color.white;
        UnityEditor.Handles.Label( (Vector2)transform.position + center, "weight " + weight.ToString());
        #endif

        foreach (var child in Child) {
            Gizmos.DrawLine((Vector2)transform.position + center, (Vector2)child.transform.position + child.center);
        }

        Gizmos.color = Color.green;

        foreach (var mob in monsters) {
            Gizmos.DrawSphere(mob.spawnOffset, 0.2f);
        }
    }
}