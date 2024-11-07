using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Potion : MonoBehaviour
{
    [SerializeField] private ParticleSystem pourEffect;
    [SerializeField] private GameObject Cork;
    [SerializeField] private Renderer FluidMesh;
    [SerializeField] private float healthAmount = 5f; // Amount of health to add when pouring
    [SerializeField] private Player player; // Reference to the player
    [SerializeField] private float effectCoolDownTimer;
    [SerializeField] private bool isSingleUse;

    public UnityEvent effectEvent;
    
    private float activationAngle = 60f; // Angle for starting the pour
    private float headPourAngle = 60f; // Angle for adding health
    private float FluidLevel;
    private float pourSpeed = 0.01f;
    private readonly string fillPropertyName = "_Fill";

    private float effectTimer;
    private PotionDrinkCollider potionDrinkCollider;

    private void Awake()
    {
        potionDrinkCollider = FindObjectOfType<PotionDrinkCollider>();
    }

    private void Start()
    {
        StopPouring();
        FluidLevel = FluidMesh.material.GetFloat(fillPropertyName);

        player = FindObjectOfType<Player>();
    }

    void Update()
    {
        float angle = Vector3.Angle(Vector3.down, transform.up);
        FluidLevel = FluidMesh.material.GetFloat(fillPropertyName);

        // Stop pouring if no fluid is left
        if (FluidLevel <= 0)
        {
            FluidLevel = 0;
            FluidMesh.material.SetFloat(fillPropertyName, 0);
            StopPouring();
            return;
        }

        // Start pouring based on the angle of the potion
        if (angle < activationAngle && FluidLevel > 0)
        {
            StartPouring();
        }
        else if (angle >= activationAngle)
        {
            StopPouring();
        }
    }

    void StartPouring()
    {
        pourEffect.Play();

        // While pouring, decrease fluid level
        if (FluidLevel > 0)
        {
            FluidLevel = Mathf.Max(0, FluidLevel - (pourSpeed * Time.deltaTime));
            FluidMesh.material.SetFloat(fillPropertyName, FluidLevel);
        }

        effectTimer -= Time.deltaTime;

        if (effectTimer <= 0)
        {
            effectEvent.Invoke();
            effectTimer = effectCoolDownTimer;
        }
    }

    void StopPouring()
    {
        pourEffect.Stop();
    }
}
