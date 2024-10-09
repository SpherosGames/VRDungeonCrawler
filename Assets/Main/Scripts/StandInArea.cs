using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class StandInArea : MonoBehaviour
{

    [SerializeField] private UnityEvent OnEnoughTimeIn;
    [SerializeField] private float TimeBeforeUse;
    [SerializeField] private float Cooldown;
    [SerializeField] private Gradient ChangeColorOnTime;
    [SerializeField] private ParticleSystem Particles;
    float CurrentCooldown;
    public float CurrentTime;
    private bool PlayerInArea = false;
    // Start is called before the first frame update
    void Start()
    {
        CurrentTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerInArea && CurrentCooldown <= 0)
        {
            CurrentTime += Time.deltaTime;
            ChangeParticleColor();
            if(CurrentTime > TimeBeforeUse) { EnoughTimeInArea(); }
        }else
        {
            if (CurrentTime > 0 && CurrentCooldown <= 0)
            {
                CurrentTime -= Time.deltaTime;
                ChangeParticleColor();
            }
            CurrentCooldown -= Time.deltaTime;
        }
    }

    private void ChangeParticleColor()
    {
        ParticleSystem.MainModule main = Particles.main;
        main.startColor = ChangeColorOnTime.Evaluate(CurrentTime / 5);
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[Particles.particleCount];
        Particles.GetParticles(particles);

        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].startColor = ChangeColorOnTime.Evaluate(CurrentTime / 5);
        }
        Particles.SetParticles(particles, particles.Length);
    }

    private void EnoughTimeInArea()
    {
        OnEnoughTimeIn.Invoke();
        PlayerInArea = false;
        CurrentCooldown = Cooldown;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 12)
        {
            OnPlayerEnter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 12)
        {
            OnPlayerExit();
        }
    }

    private void OnPlayerEnter()
    {
        PlayerInArea = true;
    }

    private void OnPlayerExit()
    {
        PlayerInArea = false;
    }
}
