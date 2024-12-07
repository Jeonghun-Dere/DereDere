using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonController : MonoBehaviour
{
    public static DungeonController Instance {get; private set;}
    public List<NodeData> nodes;
    NodeData currentNode;
    void Awake()
    {
        Instance = this;
    }

    public void StartGame() {
        NodeData firstNode = nodes.Find((v)=>v.layer == 0);

        currentNode = firstNode;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
