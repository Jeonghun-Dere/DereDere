using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.DEREDERE.System {
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance {get; private set;}

        void Awake() {
            Instance = this;
        }

        void Update() {
            UpdateCamera();
        }

        void UpdateCamera() {
            CamManager.main.transform.position = Vector3.Lerp(CamManager.main.transform.position, new Vector3(Player.Local.transform.position.x, Player.Local.transform.position.y, CamManager.main.transform.position.z), 6 * Time.deltaTime);
        }
    }
}
