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
    }
}
