using System.Collections;
using UnityEngine;

public abstract class EntityBase : MonoBehaviour {
    protected Rigidbody2D rb;
    protected SpriteRenderer render;
    protected Animator animator;
    protected Material whiteMat, defMat;
    public bool isDeath;

    public int health, maxHealth;
    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        render = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        defMat = render.material;
        whiteMat = Resources.Load<Material>("Material/PaintWhite");

        health = maxHealth;
    }

    public bool Hurt(int damage, EntityBase attacker) {
        bool cancel = false;

        OnHurt(damage, attacker, ref cancel);

        if (cancel || isDeath) {
            return false;
        }

        health -= damage;

        if (health <= 0) {
            OnDeath(attacker);
        } else {
            StartCoroutine(hurtEff());
        }

        return true;
    }

    IEnumerator hurtEff() {
        render.material = whiteMat;

        yield return new WaitForSeconds(0.2f);

        render.material = defMat;
    }
    public virtual void OnHurt(int damage, EntityBase attacker, ref bool cancel) {}
    public abstract void OnDeath(EntityBase attacker);

}