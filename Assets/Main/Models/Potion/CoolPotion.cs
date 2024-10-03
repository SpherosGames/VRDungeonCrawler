using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolPotion : MonoBehaviour
{
    public ParticleSystem potionParticles;  // Reference to the Particle System
    

    void Update()
    {
        if (transform.rotation.x > -130f)
        {
            potionParticles.Play();
        }
        else
        {
            potionParticles.Stop();
        }
    }
}
