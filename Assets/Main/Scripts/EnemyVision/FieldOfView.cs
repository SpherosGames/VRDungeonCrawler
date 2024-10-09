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
    public int RadialResolution = 20;  // Number of segments around the cone
    public Mesh VisionConeMesh;
    public MeshFilter MeshFilter_;
    public bool HasTarget;
    public List<GameObject> currentTargets = new List<GameObject>();
    public GameObject CurrentTarget;
    private List<GameObject> enteredTargets = new List<GameObject>();
    public List<GameObject> potentialTargets = new List<GameObject>();

    private void Start()
    {
        transform.AddComponent<MeshRenderer>().material = VisionConeMaterial;
        MeshFilter_ = transform.AddComponent<MeshFilter>();
        VisionConeMesh = new Mesh();
    }

    private void Update()
    {
        HasTarget = currentTargets.Count > 0;
        enteredTargets.Clear();
        potentialTargets.Clear();

        DrawVisionCone();
        CheckTargets();
    }

    private void DrawVisionCone()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // Add center vertex (cone apex)
        vertices.Add(Vector3.zero);

        float angleStep = 2f * Mathf.PI / RadialResolution;
        float coneRadius = Mathf.Tan(VisionAngle * 0.5f * Mathf.Deg2Rad) * VisionRange;

        // Generate vertices for the base of the cone
        for (int i = 0; i < RadialResolution; i++)
        {
            float angle = i * angleStep;

            // Calculate the point on the circle at the base of the cone
            float x = Mathf.Cos(angle) * coneRadius;
            float y = Mathf.Sin(angle) * coneRadius;

            Vector3 basePoint = transform.forward * VisionRange + transform.right * x + transform.up * y;
            Vector3 rayDirection = (basePoint - transform.position).normalized;

            // Raycast to check for obstacles
            if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, VisionRange, VisionObstructingLayer))
            {
                vertices.Add(transform.InverseTransformPoint(hit.point));

                // Check if hit object is a potential target
                if (((1 << hit.collider.gameObject.layer) & TargetLayer) != 0)
                {
                    potentialTargets.Add(hit.collider.gameObject);
                }
            }
            else
            {
                // If no hit, use the point on the cone base
                vertices.Add(transform.InverseTransformDirection(basePoint));
            }
        }

        // Generate triangles
        for (int i = 0; i < RadialResolution; i++)
        {
            int nextIndex = (i + 1) % RadialResolution;

            // Triangle from apex to base segment
            triangles.Add(0); // apex
            triangles.Add(i + 1);
            triangles.Add(nextIndex + 1);
        }

        VisionConeMesh.Clear();
        VisionConeMesh.SetVertices(vertices);
        VisionConeMesh.SetTriangles(triangles, 0);
        VisionConeMesh.RecalculateNormals();
        MeshFilter_.mesh = VisionConeMesh;
    }

    private void CheckTargets()
    {
        // Remove targets that are no longer in view
        currentTargets.RemoveAll(target => target == null || !IsInView(target.transform.position));

        // Check for new targets
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
                Debug.Log($"Target entered: {target.name}");
            }
        }
    }

    private bool IsInView(Vector3 targetPosition)
    {
        Vector3 directionToTarget = targetPosition - transform.position;
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

        if (directionToTarget.magnitude <= VisionRange && angleToTarget <= VisionAngle * 0.5f)
        {
            // Check if there are obstacles between
            if (Physics.Raycast(transform.position, directionToTarget, out RaycastHit hit, VisionRange, VisionObstructingLayer | TargetLayer))
            {
                return ((1 << hit.collider.gameObject.layer) & TargetLayer) != 0;
            }
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the cone in the editor
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