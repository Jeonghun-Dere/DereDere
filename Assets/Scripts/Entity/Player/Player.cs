using System.Collections;
using System.Collections.Generic;
using Com.DEREDERE.System;
using UnityEngine;

public enum PlayerWeapon {
    ShotGun,
    Rifle
}
public class Player : EntityBase
{
    public static Player Local {get; private set;}
    #region Item
    [SerializeField]
    GameObject item;
    SpriteRenderer itemRender;
    #endregion
    #region public values
    public float moveSpeed, stopMove, atkCool;
    public bool grounded, isMoving;
    public bool falled;
    public GameObject target;
    public int weight, kill;
    public PlayerWeapon weapon;
    #endregion

    #region Private values
    Vector2 moveDelta, moveDeltaBefore;
    float moveTime;
    Vector2 itemOffset, targetPos;
    float angle;
    DamageArea atkArea;
    [SerializeField]
    GameObject muzzle;
    #endregion
    Cooldown dashCool = new(0.4f);

    [SerializeField]
    Sprite shotgun, rifle;
    [SerializeField]
    Projectile bullet;
    int atkDamage = 5;

    void Start()
    {
        itemOffset = item.transform.localPosition;
        itemRender = item.GetComponent<SpriteRenderer>();

        atkArea = DamageArea.Init(transform, DamageAreaShape.FanShaped, 1.5f, 1.5f);
        atkArea.center = new Vector3(0, -1);
        atkArea.offset = new Vector3(0, -1);

        Local = this;
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (stopMove > 0) {
            moveDelta = Vector2.zero;
            stopMove -= Time.deltaTime;
        } else {
            moveDelta = Vector2.up * v + Vector2.right * h;
        }

        if (atkCool > 0) {
            atkCool -= Time.deltaTime;

            atkArea.BecomeRed(true);
        } else {
            atkArea.BecomeRed(false);
        }

        isMoving = moveDelta.x + moveDelta.y != 0;

        if (isMoving) {
            moveDeltaBefore = moveDelta;
            moveTime += Time.deltaTime;
        } else {
            moveTime = 0;
        }

        item.transform.localPosition = Vector2.Lerp(item.transform.localPosition, itemOffset + new Vector2(0, Mathf.Sin(moveTime * 15)*0.05f), 12 * Time.deltaTime);

        transform.Translate(moveDelta * moveSpeed * Time.deltaTime);

        animator.SetBool("isMoving", isMoving);

        if (isMoving) {
            transform.localScale = new Vector3((h < 0) ? -1 : 1, 1, 1);
        }

        if (falled) {
            rb.gravityScale = 2;
            render.sortingLayerName = "playerFall";

        } else {
            rb.gravityScale = 0;
            render.sortingLayerName = "player";
        }


        if (target == null) {
            targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        } else {
            targetPos = target.transform.position;
        }

        targetPos.x *= -1;

   	    angle = Mathf.Atan2(targetPos.y - transform.position.y, targetPos.x - transform.position.x) * Mathf.Rad2Deg;
        item.transform.rotation = Quaternion.AngleAxis((transform.localScale.z < 0) ? -angle : angle, Vector3.forward);


        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            Dash();
        }

        if (Input.GetMouseButtonDown(0)) {
            atkArea.Show();
        }

        if (Input.GetMouseButtonDown(1)) {
            ChangeWeapon();
        }

        if (Input.GetMouseButtonUp(0)) {
            atkArea.Hide();
            if (weapon == PlayerWeapon.ShotGun) {
                Attack();
            }
        }

        if (Input.GetMouseButton(0)) {
            if (weapon == PlayerWeapon.Rifle) {
                Attack();
            }
        }
    }

    void ShowMuzzle() {
        Vector3 scale = new Vector3(1f, 1.5f, transform.localScale.z);
        muzzle.SetActive(true);

        muzzle.transform.SetParent(atkArea.rot.transform);

        muzzle.transform.localPosition = new Vector3(0, 0.8f);
        muzzle.transform.localRotation = Quaternion.Euler(0, 0, 90);

        muzzle.transform.localScale = scale * 0.8f;

        LeanTween.scale(muzzle, scale * 1.25f, 0.1f).setEaseInCubic();
    }

    public void Attack(){
        StartCoroutine(atkEffect());
    }

    public void ChangeWeapon() {
        if (weapon == PlayerWeapon.ShotGun) {
            weapon = PlayerWeapon.Rifle;

            itemRender.sprite = rifle;
        } else {
            weapon = PlayerWeapon.ShotGun;

            itemRender.sprite = shotgun;
        }
    }

    IEnumerator atkEffect() {
        if (atkCool > 0) {
            yield break;
        }

        LeanTween.moveLocal(item, itemOffset + new Vector2(0.25f * transform.localScale.x, 0.05f), 0.25f).setEasePunch();
        LeanTween.scale(item, Vector2.one * 0.44f, 0.1f).setEaseInCubic();

        CamManager.main.Shake(6);

        ShowMuzzle();

        yield return new WaitForSeconds(0.1f);

        if (weapon == PlayerWeapon.ShotGun) {
            AtkShotGun();
        } else {
            AtkRifle();
        }

        // LeanTween.moveLocal(item, itemOffset, 0.1f);
        LeanTween.scale(item, Vector2.one * 0.4f, 0.1f);

        atkCool = 0.3f;

        yield return new WaitForSeconds(0.1f);

        muzzle.SetActive(false);
    }

    void AtkRifle() {
        var pj = Instantiate(bullet, muzzle.transform.position, muzzle.transform.rotation);

        pj.LifeTime = 3;

        // pj.transform.rotation = leftArea.rot.rotation;
        
        Vector3 dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        

        pj.rb.AddForce(dir * 8, ForceMode2D.Impulse);
    }

    void AtkShotGun() {
        CamManager.main.Shake(0.8f, 0.3f);

        stopMove = 0.3f;

        foreach (EntityBase entity in atkArea.casted) {
            float dist = 4 - Vector2.Distance(entity.transform.position, transform.position);

            if (dist < 1) dist = 1;

            entity.Hurt((int)(atkDamage * dist), this);
        }
    }

    void Dash() {
        StartCoroutine(dash());
    }
    IEnumerator dash() {
        if (dashCool.IsIn()) {
            yield break;
        }

        dashCool.Start();

        item.SetActive(false);
        rb.linearVelocity = moveDeltaBefore * 6;
        animator.SetTrigger("Dash");

        yield return new WaitForSeconds(0.3f);

        rb.linearVelocity = Vector2.zero;
        item.SetActive(true);
    }
    void OnTriggerExit2D(Collider2D col) {
        if (col.tag == "ground") {
            falled = true;
        }
    }

    public override void OnDeath(EntityBase attacker)
    {
    }
}
