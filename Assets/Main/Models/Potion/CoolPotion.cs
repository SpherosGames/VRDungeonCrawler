using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolPotion : MonoBehaviour
{
    public ParticleSystem pourEffect; // Reference to the Particle System for the pouring effect
    private float activationAngle = 60f; // The angle threshold to determine when to start pouring (closer to 0 means more upside down)
    public GameObject Cork;
    public float FluidLevel;
    public Renderer FluidMesh;
    [SerializeField] private bool isPouring = false;
    [SerializeField] private float pourSpeed = 0.1f; // Speed at which the fluid decreases (units per second)
    private readonly string fillPropertyName = "_Fill"; // The name of the shader property for fill level

    private void Start()
    {
        StopPouring();
        // Initialize FluidLevel from the material if needed
        FluidLevel = FluidMesh.material.GetFloat(fillPropertyName);
    }

    void Update()
    {
        // Get the current angle of the object's rotation in world space.
        float angle = Vector3.Angle(Vector3.down, transform.up);

        // Update fluid level before checking conditions
        FluidLevel = FluidMesh.material.GetFloat(fillPropertyName);

        // Check if fluid is empty and stop pouring if it is
        if (FluidLevel <= 0 && isPouring)
        {
            FluidLevel = 0;
            FluidMesh.material.SetFloat(fillPropertyName, 0);
            StopPouring();
            return; // Exit early since we can't pour anymore
        }

        // Handle pouring based on angle
        if (angle < activationAngle && !isPouring && FluidLevel > 0)
        {
            StartPouring();
        }
        else if (angle >= activationAngle && isPouring)
        {
            StopPouring();
        }

        // Decrease fluid level while pouring
        if (isPouring && FluidLevel > 0)
        {
            FluidLevel = Mathf.Max(0, FluidLevel - (pourSpeed * Time.deltaTime));
            FluidMesh.material.SetFloat(fillPropertyName, FluidLevel);
        }
    }

    // Function to start the particle effect
    void StartPouring()
    {
        isPouring = true;
        pourEffect.Play();
    }

    // Function to stop the particle effect
    void StopPouring()
    {
        isPouring = false;
        pourEffect.Stop();
    }
}