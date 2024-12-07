using UnityEngine;
using UnityEngine.UI;

public class Enemy : EntityBase
{
    public int weightRangeMin, weihtRangeMax;
    public int weight;
    float updateTime;
    [SerializeField]
    Slider HP;
    [SerializeField]
    Text weightText;

    void Update() {
        updateTime += Time.deltaTime;

        if (updateTime > 2) {
            updateTime = 0;
            weight = Random.Range(weightRangeMin, weihtRangeMax);
        }

        if (weight > 0) {
            weightText.text = "+" + weight.ToString();
        } else if (weight < 0) {
            weightText.text = "<color=\"red\">" + weight.ToString() + "</color>";
        } else {
            weightText.text = "<color=\"white\">" + weight.ToString() + "</color>";
        }

        HP.value = (float)health / maxHealth;
    }
    public override void OnDeath(EntityBase attacker)
    {
        Player player = attacker.GetComponent<Player>();
        if (player != null) {
            player.weight += weight;
        }
        isDeath = true;

        Destroy(gameObject, 0.5f);
    }
}