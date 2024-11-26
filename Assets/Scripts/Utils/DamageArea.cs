
using System.Collections.Generic;
using Entity;
using Entity.ViewAngle;
using Unity.VisualScripting;
using UnityEngine;

public enum DamageAreaShape {
    Square,
    FanShaped,
    Circle
}
public class DamageArea : MonoBehaviour {
    #region Public Variables
    public float width, length;
    public bool isShowing;
    public List<EntityBase> casted = new();
    public Vector3 offset;
    public Vector3 center;
    public Transform parent;
    public DamageAreaShape shape;
    #endregion

    #region Private Variables
    SpriteRenderer render;
    MeshRenderer renderMesh;
    BoxCollider2D col;
    Vector2 mouse;
    float angle;
    public Transform rot;
    SectorCollider sector;
    #endregion

    #region Static Methods
    public static DamageArea Init(Transform parent, DamageAreaShape shape, float width, float length) {
        GameObject obj = new GameObject("DamageArea");
        GameObject rot = new GameObject("rot");

        rot.transform.SetParent(parent);
        rot.transform.localPosition = Vector3.zero;

        obj.transform.SetParent(rot.transform);
        obj.transform.localPosition = Vector3.zero;

        DamageArea da = obj.AddComponent<DamageArea>();
        da.shape = shape;

        da.width = width;
        da.length = length;

        da.parent = parent;

        da.rot = rot.transform;

        if (shape == DamageAreaShape.Square) {
            var render = obj.AddComponent<SpriteRenderer>();
            render.sprite = Resources.Load<Sprite>("dummy/white_box");

            var col = obj.AddComponent<BoxCollider2D>();
            col.isTrigger = true;   
            col.size = Vector2.one;

            render.sortingLayerName = "damageArea";
        } else if (shape == DamageAreaShape.FanShaped) {
            var render = obj.AddComponent<MeshRenderer>();
            render.sortingLayerName = "damageArea";

            SectorCollider sector = obj.AddComponent<SectorCollider>();
            sector.targetLayer = LayerMask.NameToLayer("monster");
            sector.obstacleLayer = LayerMask.NameToLayer("wall");
            sector.radius = length;
            sector.angle = width * 180f / (Mathf.PI * length);

            sector.show = false;
            sector.sectorColor = Color.white;
        }

        return da;
    }
    #endregion
    
    void Start() {
        render = GetComponent<SpriteRenderer>();
        renderMesh = GetComponent<MeshRenderer>();
        col = GetComponent<BoxCollider2D>();
        sector = GetComponent<SectorCollider>();
        transform.localScale = new Vector3(width, length);
        transform.localPosition = new Vector3(0, length / 2) + offset;
    }

    void Update() {
        rot.transform.localPosition = center;

        if (shape == DamageAreaShape.Square) {
            Color @color = render.color;
            if (isShowing) {
                transform.localScale = new Vector3(width, length);
                transform.localPosition = new Vector3(0, length / 2) + offset;

                @color.a = 0.3f;
            } else {
                @color.a = 0f;
            }

            render.color = @color;
        } else if (shape == DamageAreaShape.FanShaped) {
            Color @color = sector.sectorColor;
            if (isShowing) {
                sector.show = true;

                @color.a = 0.3f;
            } else {
                sector.show = false;

                @color.a = 0f;
            }

            sector.sectorColor = @color;

            sector.SectorUpdate();
        }

        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var centerPos = parent.position + center;
        angle = Mathf.Atan2(mouse.y - centerPos.y, mouse.x - centerPos.x) * Mathf.Rad2Deg;
        rot.rotation = Quaternion.AngleAxis(angle-90, Vector3.forward);
    }

    void OnTriggerEnter2D(Collider2D col) {
        var dam = col.GetComponent<EntityBase>();
        if (dam != null) {
            casted.Add(dam);
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        var dam = col.GetComponent<EntityBase>();
        if (dam != null) {
            casted.Remove(dam);
        }
    }

    public void Show() {
        isShowing = true;
    }
    public void BecomeRed(bool val) {
        if (shape == DamageAreaShape.Square) {
            Color @color = render.color;

            if (val) {
                Color col = Color.red;
                col.a = @color.a;

                render.color = col;
            } else {
                Color col = Color.white;
                col.a = @color.a;

                render.color = col;
            }
        } else if (shape == DamageAreaShape.FanShaped) {
            Color @color = sector.sectorColor;

            if (val) {
                Color col = Color.red;
                col.a = @color.a;

                sector.sectorColor = col;
            } else {
                Color col = Color.white;
                col.a = @color.a;

                sector.sectorColor = col;
            }
        }
    }

    public void Hide() {
        isShowing = false;
    }
}