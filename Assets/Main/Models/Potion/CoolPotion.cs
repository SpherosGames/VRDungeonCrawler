using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolPotion : MonoBehaviour
{
    public ParticleSystem pourEffect; // Reference to the Particle System for the pouring effect
    private float activationAngle = 60f; // The angle threshold to determine when to start pouring (closer to 0 means more upside down)
    public GameObject Cork;

    private bool isPouring = false;

    void Update()
    {

        // Get the current angle of the object's rotation in world space.
        float angle = Vector3.Angle(Vector3.down, transform.up);

        // If the angle is less than the activationAngle, start the particle effect (meaning it's upside down).
        if (angle < activationAngle && !isPouring && Cork == null)
        {
            StartPouring();
        }
        // If the angle is greater than the activationAngle, stop the particle effect (meaning it's upright).
        else if (angle >= activationAngle && isPouring && Cork != null)
        {
            StopPouring();
        }
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
