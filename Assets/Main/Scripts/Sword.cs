using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] private float minForceThreshold = 3f; 
    [SerializeField] private float damageMultiplier = 0.5f;
    [SerializeField] private float maxDamage = 10f; 

    private Rigidbody rb;
    private Vector3 previousPosition;
    private float currentVelocityMagnitude;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        previousPosition = transform.position;
    }

    void Update()
    {
        
        Vector3 currentVelocity = (transform.position - previousPosition) / Time.deltaTime;
        currentVelocityMagnitude = currentVelocity.magnitude;
        previousPosition = transform.position;
    }
    private void OnCollisionEnter(Collision other)
    {

        if (currentVelocityMagnitude >= minForceThreshold)
        {
            if (other.gameObject.GetComponentInParent<Unit>())
            {
                Unit unit = other.gameObject.GetComponentInParent<Unit>();

                float damage = Mathf.Min((currentVelocityMagnitude - minForceThreshold) * damageMultiplier, maxDamage);
                int roundedDamage = Mathf.RoundToInt(damage);

                // Ensure minimum damage of 1 if threshold is met
                roundedDamage = Mathf.Max(1, roundedDamage);

                Debug.Log($"Dealing {roundedDamage} damage to {other.gameObject.name}");
                unit.TakeDamage(roundedDamage);
            }
        }
    }

    public void SetDamageMultiplier(float value)
    {
        damageMultiplier = value;
    }
}
