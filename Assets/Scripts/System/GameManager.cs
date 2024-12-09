using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Com.DEREDERE.System {
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance {get; private set;}
        public DungeonController dungeon;

        bool started;

        void Awake() {
            Instance = this;
        }
        void Start() {
            StartCoroutine(StartGame());
        }

        IEnumerator StartGame() {
            yield return new WaitForSeconds(0.5f);

            dungeon.OnStart();

            started = true;
        }

        void Update() {
            UpdateCamera();

            if (started) {
                if (dungeon.ended) {
                    started = false;

                    SceneManager.LoadScene("GameOver");
                }

                if (Player.Local.fallTime > 2 && dungeon.inWave || Player.Local.health <= 0) {
                    SceneManager.LoadScene("GameOverFail");
                }
            }
        }

        void UpdateCamera() {
            CamManager.main.transform.position = Vector3.Lerp(CamManager.main.transform.position, new Vector3(Player.Local.transform.position.x, Player.Local.transform.position.y, CamManager.main.transform.position.z), 6 * Time.deltaTime);
        }
    }
}
