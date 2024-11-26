using TMPro;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    TMP_Text txt;
    float time;
    public static void Show(Vector2 pos, float damage, Color col, bool crit = false) {
        DamageIndicator obj = Resources.Load<DamageIndicator>("Prefabs/UI/DamageInfo");

        DamageIndicator instantiated = Instantiate(obj, pos, Quaternion.identity);

        instantiated.txt.text = ((int)damage).ToString();
        instantiated.txt.color = col;
        
        if (crit) {
            instantiated.txt.fontStyle = FontStyles.Italic;
            instantiated.txt.text += "!!";
        }

    }
    void Awake() {
        txt = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        transform.position = transform.position + new Vector3(0, 0.3f * Time.deltaTime);

        if (time > 1) {
            Destroy(gameObject);
        }
    }
}
