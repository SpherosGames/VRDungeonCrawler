using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolPotion : MonoBehaviour
{
    public ParticleSystem pourEffect; 
    float activationAngle = 60f; 
    public GameObject Cork;
    float FluidLevel;
    public Renderer FluidMesh;
    bool isPouring = false;
    float pourSpeed = 0.1f;
    private readonly string fillPropertyName = "_Fill";
    public float healthAmount = 5f;

    private void Start()
    {
        StopPouring();
        FluidLevel = FluidMesh.material.GetFloat(fillPropertyName);
    }
    void Update()
    {
        float angle = Vector3.Angle(Vector3.down, transform.up);
        FluidLevel = FluidMesh.material.GetFloat(fillPropertyName);

        if (FluidLevel <= 0 && isPouring)
        {
            FluidLevel = 0;
            FluidMesh.material.SetFloat(fillPropertyName, 0);
            StopPouring();
            return; 
        }
        if (angle < activationAngle && !isPouring && FluidLevel > 0)
        {
            StartPouring();
        }
        else if (angle >= activationAngle && isPouring)
        {
            StopPouring();
        }
        if (isPouring && FluidLevel > 0)
        {
            FluidLevel = Mathf.Max(0, FluidLevel - (pourSpeed * Time.deltaTime));
            FluidMesh.material.SetFloat(fillPropertyName, FluidLevel);
        }
    }
    void StartPouring()
    {
        isPouring = true;
        pourEffect.Play();
    }
    void StopPouring()
    {
        isPouring = false;
        pourEffect.Stop();
    }
}