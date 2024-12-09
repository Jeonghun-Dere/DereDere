using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public void GoHome() {
        SceneManager.LoadScene("StartScene");
        
    }
}
