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

    // Update is called once per frame
    void Update()
    {
    }
}
