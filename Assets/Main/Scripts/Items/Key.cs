using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Item
{

    [SerializeField] private Transform KeyPlus;
    [SerializeField] private float SpinSpeedMultiplier;
    private float[] RandoSpin = new float[3];
    [SerializeField] private Rigidbody RigidB;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < RandoSpin.Length; i++)
        {
            if (RandoSpin[i]  < 5 && RandoSpin[i] > 0)
            {
                RandoSpin[i] += 10;
            }
            else if (RandoSpin[i] < 0 && RandoSpin[i] > -5)
            {
                RandoSpin[i] += -10;
            }
            else
            {
                RandoSpin[i] += Random.Range(-10, 11);
            }
            if (RandoSpin[i] > 100)
            {
                RandoSpin[i] = 100;
            }else if (RandoSpin[i] < -100)
            {
                RandoSpin[i] = -100;
            }
        }

        KeyPlus.Rotate((SpinSpeedMultiplier * RandoSpin[0] * (RigidB.velocity.magnitude + 1)) * Time.deltaTime, (SpinSpeedMultiplier * RandoSpin[1] * (RigidB.velocity.magnitude + 1)) * Time.deltaTime, (SpinSpeedMultiplier * RandoSpin[2] * (RigidB.velocity.magnitude + 1)) * Time.deltaTime);
    }
}
