using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : EntityBase
{
    public static Player Local {get; private set;}
    #region Item
    [SerializeField]
    GameObject item;
    #endregion
    #region public values
    public float moveSpeed;
    public bool grounded, isMoving;
    public bool falled;
    public GameObject target;
    #endregion

    #region Private values
    private Rigidbody2D rb;
    private SpriteRenderer render;
    private Animator animator;
    Vector2 moveDelta, moveDeltaBefore;
    float moveTime;
    Vector2 itemOffset, targetPos;
    float angle;
    DamageArea atkArea;
    #endregion

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        render = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        itemOffset = item.transform.localPosition;

        atkArea = DamageArea.Init(transform, DamageAreaShape.FanShaped, 1.5f, 1.5f);
        atkArea.center = new Vector3(0, -1);
        atkArea.offset = new Vector3(0, -1);

        Local = this;
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        moveDelta = Vector2.up * v + Vector2.right * h;
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
        item.transform.rotation = Quaternion.AngleAxis(render.flipX ? -angle : angle, Vector3.forward);


        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            Dash();
        }

        if (Input.GetMouseButtonDown(0)) {
            atkArea.Show();
        }

        if (Input.GetMouseButtonUp(0)) {
            Attack();
        }
    }

    public void Attack(){
        atkArea.Hide();
        StartCoroutine(atkEffect());
    }

    IEnumerator atkEffect() {
        LeanTween.moveLocal(item, itemOffset + new Vector2(0.25f * transform.localScale.x, 0.05f), 0.25f).setEasePunch();
        LeanTween.scale(item, Vector2.one * 0.44f, 0.1f).setEaseInCubic();

        yield return new WaitForSeconds(0.1f);

        // LeanTween.moveLocal(item, itemOffset, 0.1f);
        LeanTween.scale(item, Vector2.one * 0.4f, 0.1f);
    }

    void Dash() {
        StartCoroutine(dash());
    }
    IEnumerator dash() {
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
}
