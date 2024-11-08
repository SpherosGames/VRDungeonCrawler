using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public Material VisionConeMaterial;
    public float VisionRange = 10f;
    public float VisionAngle = 90f;
    public LayerMask VisionObstructingLayer;
    public LayerMask TargetLayer;
    public int RadialResolution = 20;
    public Mesh VisionConeMesh;
    public MeshFilter MeshFilter_;
    public bool HasTarget;
    public List<GameObject> currentTargets = new List<GameObject>();
    public GameObject CurrentTarget;
    private List<GameObject> enteredTargets = new List<GameObject>();
    public List<GameObject> potentialTargets = new List<GameObject>();
    public bool showVisionCone;

    private float timer;

    [Header("Visual Settings")]
    public Color MainColor = new Color(1f, 0.5f, 0f, 0.5f); // Orange with transparency
    public Color PulseColor = new Color(1f, 0f, 0f, 0.5f);  // Red with transparency
    public float PulseSpeed = 2f;
    public float EdgeGlowIntensity = 2f;

    private Material instancedMaterial;
    private float pulseTime;
    private void Start()
    {
        // Create an instanced material so we can modify it without affecting other objects
        instancedMaterial = new Material(VisionConeMaterial);
        instancedMaterial.shader = Shader.Find("Universal Render Pipeline/Unlit");
        // Set up the material properties
        instancedMaterial.SetColor("_BaseColor", MainColor);
        instancedMaterial.SetColor("_PulseColor", PulseColor);
        instancedMaterial.SetFloat("_PulseSpeed", PulseSpeed);
        instancedMaterial.SetFloat("_EdgeGlow", EdgeGlowIntensity);
        // Enable transparency
        instancedMaterial.SetFloat("_Surface", 1); // 0 = Opaque, 1 = Transparent
        instancedMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        instancedMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        instancedMaterial.SetInt("_ZWrite", 0);
        instancedMaterial.EnableKeyword("_ALPHABLEND_ON");
        // Set material to be double-sided
        instancedMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);

        transform.AddComponent<MeshRenderer>().material = instancedMaterial;
        MeshFilter_ = transform.AddComponent<MeshFilter>();
        VisionConeMesh = new Mesh();
    }
    private void Update()
    {
        HasTarget = currentTargets.Count > 0;
        enteredTargets.Clear();
        potentialTargets.Clear();
        if (showVisionCone) DrawVisionCone();

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            CheckTargets();
            timer = 1;
        }

        // Update pulse effect
        pulseTime += Time.deltaTime;
        if (showVisionCone) instancedMaterial.SetFloat("_PulseTime", pulseTime);

        // Intensify glow when target is detected
        float targetIntensity = HasTarget ? 2f : 1f;
        if (showVisionCone) instancedMaterial.SetFloat("_EdgeGlow", EdgeGlowIntensity * targetIntensity);
    }
    private void DrawVisionCone()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        vertices.Add(Vector3.zero); // Apex

        float angleStep = 2f * Mathf.PI / RadialResolution;
        float coneRadius = Mathf.Tan(VisionAngle * 0.5f * Mathf.Deg2Rad) * VisionRange;

        // Generate vertices for the base of the cone
        for (int i = 0; i < RadialResolution; i++)
        {
            float angle = i * angleStep;
            float x = Mathf.Cos(angle) * coneRadius;
            float y = Mathf.Sin(angle) * coneRadius;

            Vector3 basePoint = transform.forward * VisionRange + transform.right * x + transform.up * y;
            Vector3 rayDirection = (basePoint - transform.position).normalized;

            if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, VisionRange, VisionObstructingLayer))
            {
                vertices.Add(transform.InverseTransformPoint(hit.point));
                if (((1 << hit.collider.gameObject.layer) & TargetLayer) != 0)
                {
                    potentialTargets.Add(hit.collider.gameObject);
                }
            }
            else
            {
                vertices.Add(transform.InverseTransformDirection(basePoint));
            }
        }
        // Generate triangles (front-facing)
        for (int i = 0; i < RadialResolution; i++)
        {
            int nextIndex = (i + 1) % RadialResolution;

            // Front face
            triangles.Add(0);
            triangles.Add(i + 1);
            triangles.Add(nextIndex + 1);

            // Back face
            triangles.Add(0);
            triangles.Add(nextIndex + 1);
            triangles.Add(i + 1);
        }
        VisionConeMesh.Clear();
        VisionConeMesh.SetVertices(vertices);
        VisionConeMesh.SetTriangles(triangles, 0);
        VisionConeMesh.RecalculateNormals();
        MeshFilter_.mesh = VisionConeMesh;
    }

    private void CheckTargets()
    {
        currentTargets.RemoveAll(target => target == null || !IsInView(target.transform.position));

        Collider[] potentialColliders = Physics.OverlapSphere(transform.position, VisionRange, TargetLayer);
        foreach (Collider collider in potentialColliders)
        {
            GameObject target = collider.gameObject;
            if (!currentTargets.Contains(target) && IsInView(target.transform.position))
            {
                currentTargets.Add(target);
                enteredTargets.Add(target);
                CurrentTarget = target;
                GetComponentInParent<EnemyAttack>()?.GoToTarget(target.transform);
               // Debug.Log($"Target : {target.name}");
            }
        }
    }
    private bool IsInView(Vector3 targetPosition)
    {
        Vector3 directionToTarget = targetPosition - transform.position;
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

        if (directionToTarget.magnitude <= VisionRange && angleToTarget <= VisionAngle * 0.5f)
        {
            if (Physics.Raycast(transform.position, directionToTarget, out RaycastHit hit, VisionRange, VisionObstructingLayer | TargetLayer))
            {
                return ((1 << hit.collider.gameObject.layer) & TargetLayer) != 0;
            }
        }
        return false;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 forward = transform.forward * VisionRange;
        float radius = Mathf.Tan(VisionAngle * 0.5f * Mathf.Deg2Rad) * VisionRange;
        Vector3 right = transform.right * radius;
        Vector3 up = transform.up * radius;

        for (int i = 0; i < 360; i += 30)
        {
            float angle = i * Mathf.Deg2Rad;
            Vector3 point = forward + right * Mathf.Cos(angle) + up * Mathf.Sin(angle);
            Gizmos.DrawLine(transform.position, transform.position + point);
        }
    }
}