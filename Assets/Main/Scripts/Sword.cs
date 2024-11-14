using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] private float minForceThreshold = 3f; 
    [SerializeField] private float damageMultiplier = 0.5f;
    [SerializeField] private float maxDamage = 10f;
    [SerializeField] private LayerMask enemyLayer;

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
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1, enemyLayer);

        if (colliders.Length > 0)
        {
            DamageUnit(colliders[0].gameObject);
        }

        Vector3 currentVelocity = (transform.position - previousPosition) / Time.deltaTime;
        currentVelocityMagnitude = currentVelocity.magnitude;
        previousPosition = transform.position;
    }

    private void OnCollisionEnter(Collision other)
    {
        //Debug.Log(other.gameObject.name);
        //if (currentVelocityMagnitude >= minForceThreshold)
        //{
        //DamageUnit(other);
        //}
    }

    private void DamageUnit(GameObject hitObject)
    {
        if (hitObject.gameObject.GetComponentInParent<Unit>())
        {
            print("Trying to damage: " + hitObject.name);

            Unit unit = hitObject.gameObject.GetComponentInParent<Unit>();

            float damage = Mathf.Min((currentVelocityMagnitude - minForceThreshold) * damageMultiplier, maxDamage);
            int roundedDamage = Mathf.RoundToInt(damage);

            // Ensure minimum damage of 1 if threshold is met
            roundedDamage = Mathf.Max(1, roundedDamage);

            Debug.Log($"Dealing {roundedDamage} damage to {unit.gameObject.name}");
            unit.TakeDamage(roundedDamage);
        }
    }

    public void SetDamageMultiplier(float value)
    {
        damageMultiplier = value;
    }
}
