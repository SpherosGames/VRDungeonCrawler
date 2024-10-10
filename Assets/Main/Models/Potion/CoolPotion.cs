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
        // Initialize FluidLevel from the material if needed
        FluidLevel = FluidMesh.material.GetFloat(fillPropertyName);
    }

    void Update()
    {
        // Get the current angle of the object's rotation in world space.
        float angle = Vector3.Angle(Vector3.down, transform.up);

        // If the angle is less than the activationAngle, start pouring
        if (angle < activationAngle && !isPouring)
        {
            StartPouring();
        }
        // If the angle is greater than the activationAngle, stop pouring
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

        // Update the stored FluidLevel from the material
        FluidLevel = FluidMesh.material.GetFloat(fillPropertyName);
    }

    // Function to start the particle effect
    void StartPouring()
    {
        isPouring = true;
        pourEffect.Play(); // Start the particle system
    }

    // Function to stop the particle effect
    void StopPouring()
    {
        isPouring = false;
        pourEffect.Stop(); // Stop the particle system
    }
}