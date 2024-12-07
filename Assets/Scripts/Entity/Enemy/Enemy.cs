using System;
using System.Collections.Generic;
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
    [SerializeField]
    int attackDamage;
    [SerializeField]
    float moveSpeed, followMin, followMax, attackDelay;
    float startFollowing, atkDelay;
    public Action<Enemy> onDeath;

    void Update() {
        updateTime += Time.deltaTime;

        if (updateTime > 2) {
            updateTime = 0;
            weight = UnityEngine.Random.Range(weightRangeMin, weihtRangeMax);
        }
    }
    private void FixedUpdate() {
        float dist = Vector2.Distance(transform.position, Player.Local.transform.position);

        if (dist < followMax) {
            if (dist < followMin) {
                if (atkDelay > attackDelay) {
                    startFollowing = 0;
                    atkDelay = 0;

                    Attack(Player.Local);
                }

                atkDelay += Time.deltaTime;
            } else {
                if (startFollowing < 1) {
                    startFollowing += Time.deltaTime;
                }
                transform.position = Vector2.MoveTowards(transform.position, Player.Local.transform.position, Time.deltaTime * moveSpeed * startFollowing);
            }
        }
    }

    void LateUpdate() {
        if (weight > 0) {
            weightText.text = "+" + weight.ToString();
        } else if (weight < 0) {
            weightText.text = "<color=\"red\">" + weight.ToString() + "</color>";
        } else {
            weightText.text = "<color=\"white\">" + weight.ToString() + "</color>";
        }

        HP.value = (float)health / maxHealth;
    }

    void Attack(EntityBase target) {
        target.Hurt(attackDamage, this);
    }
    public override void OnDeath(EntityBase attacker)
    {
        Player player = attacker.GetComponent<Player>();
        if (player != null) {
            player.weight += weight;
            player.kill++;
        }
        isDeath = true;

        if (onDeath != null) {
            onDeath(this);
        }

        Destroy(gameObject, 0.5f);
    }
}