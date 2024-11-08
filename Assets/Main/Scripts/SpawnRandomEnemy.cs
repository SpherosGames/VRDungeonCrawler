using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRandomEnemy : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;

    // Start is called before the first frame update
    void Start()
    {
        int rnd = Random.Range(0, enemies.Length);
        Instantiate(enemies[rnd],transform.position,transform.rotation);
    }
}
