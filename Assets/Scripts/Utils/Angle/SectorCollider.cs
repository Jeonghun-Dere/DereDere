
using System.Collections.Generic;
using UnityEngine;
namespace Entity.ViewAngle
{


public class SectorCollider : MonoBehaviour
{
    [Header("Sector Settings")]
    public float radius = 5f;
    [Range(0, 360)] public float angle = 90f;
    public int segments = 32;

    [Header("Visual Settings")] 
    public Color sectorColor = new Color(1f, 1f, 0f, 0.3f);

    [Header("Layer Settings")] 
    public LayerMask targetLayer;
    public LayerMask obstacleLayer;

    [Header("Debug")] 
    public bool showDebug = true;
    public bool show = false;
    
    
    private EdgeCollider2D edgeCollider;
    private List<Transform> detectedObjects = new List<Transform>();
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh mesh;

    void Start()
    {
        InitializeComponents();
    }

    void InitializeComponents()
    {
        
        edgeCollider = GetComponent<EdgeCollider2D>();
        if (edgeCollider == null)
        {
            edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
        }
        edgeCollider.isTrigger = true;
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            DestroyImmediate(spriteRenderer);
        }
        
        
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();

        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

        Material material = new Material(Shader.Find("Sprites/Default"));
        material.color = sectorColor;
        meshRenderer.material = material;
        meshRenderer.sortingOrder = -1;

        CreateSectorMesh();
        UpdateColliderShape();
    }

    void CreateSectorMesh() {
        
        
        if (mesh != null)
            DestroyImmediate(mesh);
        if (!show) return;
        mesh = new Mesh();
        meshFilter.mesh = mesh;

        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];
        Color[] colors = new Color[segments + 2];

        vertices[0] = Vector3.zero;
        colors[0] = sectorColor;

        float currentAngle = -angle / 2;
        float angleStep = angle / segments;

        for (int i = 0; i <= segments; i++)
        {
            float radian = currentAngle * Mathf.Deg2Rad;
            float x = Mathf.Sin(radian) * radius;
            float y = Mathf.Cos(radian) * radius;

            vertices[i + 1] = new Vector3(x, y, 0);
            colors[i + 1] = sectorColor;

            if (i < segments)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }

            currentAngle += angleStep;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
    }

    void UpdateColliderShape() {
        List<Vector2> points = new List<Vector2>();
        points.Add(Vector2.zero);

        float angleStep = angle / (segments - 1);
        float startAngle = -angle / 2;

        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = startAngle + angleStep * i;
            float radian = currentAngle * Mathf.Deg2Rad;
            points.Add(new Vector2(Mathf.Sin(radian) * radius, Mathf.Cos(radian) * radius));
        }

        points.Add(Vector2.zero);
        edgeCollider.points = points.ToArray();
    }

    void OnValidate()
    {
        if (Application.isPlaying && meshFilter != null)
        {
            SectorUpdate();
        }
    }

    public void SectorUpdate() {
        CreateSectorMesh();
        UpdateColliderShape();
        if (meshRenderer != null && meshRenderer.material != null)
            meshRenderer.material.color = sectorColor;
    }

    void OnDestroy()
    {
        if (mesh != null)
            DestroyImmediate(mesh);
    }

    // 나머지 기존 메서드들은 그대로 유지
    void OnTriggerStay2D(Collider2D other) {
        if (detectedObjects.Contains(other.transform)) return;
        
        if (!IsInLayerMask(other.gameObject.layer, targetLayer)) return;

        if (IsInViewAngle(other.transform))
        {
            if (!IsBlockedByObstacle(other.transform)) {
                detectedObjects.Add(other.transform);
                if (showDebug) Debug.Log($"Object Detected: {other.name}");
            }
        }
    }

    bool IsInViewAngle(Transform target)
    {
        Vector2 directionToTarget = (target.position - transform.position).normalized;
        float angleToTarget = Vector2.Angle(transform.up, directionToTarget);

        if (showDebug)
        {
            Debug.Log($"Angle to {target.name}: {angleToTarget}, Half View Angle: {angle / 2}");
            Debug.DrawRay(transform.position, directionToTarget * radius, Color.yellow, 0.1f);
        }

        return angleToTarget < angle / 2;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (detectedObjects.Contains(other.transform))
        {
            detectedObjects.Remove(other.transform);
            if (showDebug) Debug.Log($"Object Lost: {other.name}");
        }
    }

    bool IsBlockedByObstacle(Transform target)
    {
        Vector2 dirToTarget = (target.position - transform.position).normalized;
        float distToTarget = Vector2.Distance(transform.position, target.position);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToTarget, distToTarget, obstacleLayer);

        if (showDebug && hit.collider != null)
        {
            Debug.Log($"Ray hit obstacle: {hit.collider.name}");
            Debug.DrawLine(transform.position, hit.point, Color.red, 0.1f);
        }

        return hit.collider != null;
    }

    bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return ((1 << layer) & layerMask) != 0;
    }

    public List<Transform> GetDetectedObjects()
    {
        return new List<Transform>(detectedObjects);
    }
}
}