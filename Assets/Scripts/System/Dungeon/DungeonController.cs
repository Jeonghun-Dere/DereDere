using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonController : MonoBehaviour
{
    public static DungeonController Instance {get; private set;}
    public List<NodeData> nodes;
    NodeData currentNode;
    LinkedList<EntityBase> spawnedMob = new();
    LinkedList<MonsterData> UnspawnedMob = new();
    List<MonsterData> UnSpawnedDelete = new();
    float afterWave;
    public bool inWave, ended;
    void Awake()
    {
        Instance = this;
    }

    public void OnStart() {
        NodeData firstNode = nodes.Find((v)=>v.layer == 0);

        StartWave(firstNode);
    }

    public void NextRound() {
        StartCoroutine(nextRound());
    }

    IEnumerator nextRound() {
        inWave = false;
        NodeData node = currentNode.Next;

        if (node.final) {
            ended = true;
            yield break;
        }

        yield return new WaitForSeconds(1f);

        Player.Local.transform.LeanMove((Vector2)node.transform.position + node.center, 0.6f);

        yield return new WaitForSeconds(0.8f);

        StartWave(node);
    }

    public void StartWave(NodeData node) {
        UnspawnedMob.FromList(node.monsters);

        afterWave = 0;

        currentNode = node;

        inWave = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        afterWave += Time.fixedDeltaTime;

        foreach (MonsterData monster in UnspawnedMob) {
            if (monster.spawnDelay < afterWave) {
                int count = monster.count;

                if (monster.MaxCount != 0) {
                    count = Random.Range(count, monster.MaxCount);
                }
                for (int i = 0; i < count; i++) {
                    var mob = Instantiate(monster.monster, (Vector2)currentNode.transform.position + monster.spawnOffset, Quaternion.identity);

                    mob.transform.SetParent(currentNode.transform);

                    mob.onDeath = OnDeathEnemy;

                    spawnedMob.Add(mob);
                    UnSpawnedDelete.Add(monster);
                }
            }
        }
        foreach (MonsterData monster in UnSpawnedDelete) {
            UnspawnedMob.Remove(monster);
        }

        UnSpawnedDelete.Clear();

        if (inWave) {
            if (spawnedMob.Count() <= 0 && UnspawnedMob.Count() <= 0) {
                NextRound();
            }
        } else {
            Player.Local.falled = false;
        }
    }

    void OnDeathEnemy(Enemy _this) {
        spawnedMob.Remove(_this);
    }
}
