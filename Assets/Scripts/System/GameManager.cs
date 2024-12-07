using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.DEREDERE.System {
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance {get; private set;}
        public DungeonController dungeon;

        void Awake() {
            Instance = this;
        }
        void Start() {
            StartCoroutine(StartGame());
        }

        IEnumerator StartGame() {
            yield return new WaitForSeconds(0.5f);

            dungeon.OnStart();
        }

        void Update() {
            UpdateCamera();
        }

        void UpdateCamera() {
            CamManager.main.transform.position = Vector3.Lerp(CamManager.main.transform.position, new Vector3(Player.Local.transform.position.x, Player.Local.transform.position.y, CamManager.main.transform.position.z), 6 * Time.deltaTime);
        }
    }
}
