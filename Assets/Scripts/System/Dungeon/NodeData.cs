using System;
using System.Collections.Generic;
using UnityEngine;


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
    public List<NodeData> Child;

    void Start() {
        nodeIcon.gameObject.SetActive(true);
        DungeonController.Instance.nodes.Add(this);

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
            } else {
                nodeIcon.sprite = defaultData.DeActiveNode;
            }
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + center, size);

        Gizmos.color = Color.yellow;

        #if UNITY_EDITOR
        UnityEditor.Handles.color = Color.white;
        UnityEditor.Handles.Label( (Vector2)transform.position + center, "weight " + weight.ToString() );
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