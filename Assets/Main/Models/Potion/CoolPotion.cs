using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolPotion : MonoBehaviour
{
    public ParticleSystem pourEffect;
    public GameObject Cork;
    public Renderer FluidMesh;
    public float healthAmount = 5f; // Amount of health to add when pouring
    public Player player; // Reference to the player

    float activationAngle = 60f; // Angle for starting the pour
    float headPourAngle = 60f; // Angle for adding health (above head)
    float FluidLevel;
    bool isPouring = false;
    float pourSpeed = 0.1f;
    private readonly string fillPropertyName = "_Fill";

    private void Start()
    {
        StopPouring();
        FluidLevel = FluidMesh.material.GetFloat(fillPropertyName);

        // Optionally find player if not set in the inspector
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }
    }

    void Update()
    {
        float angle = Vector3.Angle(Vector3.down, transform.up);
        FluidLevel = FluidMesh.material.GetFloat(fillPropertyName);

        // Stop pouring if no fluid is left
        if (FluidLevel <= 0 && isPouring)
        {
            FluidLevel = 0;
            FluidMesh.material.SetFloat(fillPropertyName, 0);
            StopPouring();
            return;
        }

        // Start pouring based on the angle of the potion
        if (angle < activationAngle && !isPouring && FluidLevel > 0)
        {
            StartPouring();
        }
        else if (angle >= activationAngle && isPouring)
        {
            StopPouring();
        }

        // While pouring, decrease fluid level and apply health if poured above head
        if (isPouring && FluidLevel > 0)
        {
            FluidLevel = Mathf.Max(0, FluidLevel - (pourSpeed * Time.deltaTime));
            FluidMesh.material.SetFloat(fillPropertyName, FluidLevel);

            // If poured above head, add health to the player
           
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

    // Function to add health to the player when pouring above head
    void AddHealth()
    {
        if (player != null && player.Health < player.MaxHealth)
        {
            player.Health = Mathf.Min(player.MaxHealth, player.Health + healthAmount);
        }
    }
}
