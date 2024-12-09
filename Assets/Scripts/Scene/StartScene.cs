using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour
{
    [SerializeField]
    GameObject girl;
    Vector2 girlPos;
    float time;

    void Start() {
        girlPos = girl.transform.localPosition;
    }
    public void GameStart() {
        SceneManager.LoadScene("SampleScene");
    }

    public void Exit() {
        Application.Quit();
    }

    void Update() {
        time += Time.deltaTime;

        girl.transform.localPosition = girlPos + new Vector2(0, Mathf.Sin(time * 0.6f) * 40);
    }
}
