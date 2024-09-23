using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public Material VisionConeMaterial;
    public float VisionRange;
    public float VisionAngle;
    public LayerMask VisionObstructingLayer; // Layer with objects that obstruct the enemy view, like walls, for example
    public LayerMask TargetLayer; // Layer for potential targets
    public int VisionConeResolution = 120; // The vision cone will be made up of triangles, the higher this value is the prettier the vision cone will be
    public Mesh VisionConeMesh;
    public MeshFilter MeshFilter_;
    public bool HasTarget;
    public List<GameObject> currentTargets = new List<GameObject>();
    public GameObject CurrentTarget;
    private List<GameObject> enteredTargets = new List<GameObject>(); 
    public List<GameObject> potentialTargets = new List<GameObject>(); 

    // Create all of these variables, most of them are self explanatory, but for the ones that aren't i've added a comment to clue you in on what they do
    // for the ones that you dont understand dont worry, just follow along
    void Start()
    {

        transform.AddComponent<MeshRenderer>().material = VisionConeMaterial;
        MeshFilter_ = transform.AddComponent<MeshFilter>();
        VisionConeMesh = new Mesh();
        VisionAngle = 1.24f;
    }

    void Update()
    {
       
        if (currentTargets.Count > 0)
        {
            HasTarget = true;
        }
        else
        {
            HasTarget = false;
        }
        enteredTargets.Clear(); 
        potentialTargets.Clear(); 

        DrawVisionCone();

        
        foreach (GameObject target in currentTargets)
        {
            if (!IsInView(target.transform.position, target))
            {
           
                currentTargets.Remove(target);
                Debug.Log("Target left: " + target.name);
            }
        }

        foreach (GameObject target in potentialTargets)
        {
            if (!currentTargets.Contains(target) && IsInView(target.transform.position, target))
            {
      
                currentTargets.Add(target);
                enteredTargets.Add(target);
                CurrentTarget = target;
                Debug.Log("Target entered: " + target.name);
            }
        }
    }

    void DrawVisionCone()
    {
        int[] triangles = new int[(VisionConeResolution - 1) * 3];
        Vector3[] Vertices = new Vector3[VisionConeResolution + 1];
        Vertices[0] = Vector3.zero;
        float Currentangle = -VisionAngle / 2;
        float angleIcrement = VisionAngle / (VisionConeResolution - 1);
        float Sine;
        float Cosine;

        for (int i = 0; i < VisionConeResolution; i++)
        {
            Sine = Mathf.Sin(Currentangle);
            Cosine = Mathf.Cos(Currentangle);
            Vector3 RaycastDirection = (transform.forward * Cosine) + (transform.right * Sine);
            Vector3 VertForward = (Vector3.forward * Cosine) + (Vector3.right * Sine);

            if (Physics.Raycast(transform.position, RaycastDirection, out RaycastHit hit, VisionRange, TargetLayer))
            {
     
                potentialTargets.Add(hit.collider.gameObject);
                Vertices[i + 1] = VertForward * hit.distance;
            }
            else
            {
       
                Vertices[i + 1] = VertForward * VisionRange;
            }

            Currentangle += angleIcrement;
        }

        for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
        {
            triangles[i] = 0;
            triangles[i + 1] = j + 1;
            triangles[i + 2] = j + 2;
        }

        VisionConeMesh.Clear();
        VisionConeMesh.vertices = Vertices;
        VisionConeMesh.triangles = triangles;
        MeshFilter_.mesh = VisionConeMesh;
    }

    bool IsInView(Vector3 targetPosition, GameObject target)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, targetPosition - transform.position, out hit, VisionRange, TargetLayer))
        {
            return hit.collider.gameObject == target; 
        }
        return false;
    }
}