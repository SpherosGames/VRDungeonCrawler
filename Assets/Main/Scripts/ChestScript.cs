using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : MonoBehaviour
{
    [SerializeField] private ItemScriptableobject[] PossibleItems;
    [SerializeField] private Transform SpawnSpot;

    [SerializeField] private int MaxPossibleItems;
    [SerializeField] private int MinPossibleItems;
    [SerializeField] private float TimeBetweenItems = 0.5f;
    [SerializeField] private int ItemLaunchUpAmount = 500;
    [SerializeField] private int ItemSpread = 100;

    private bool hasBeenOpened = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(OpenChest());
        }
    }

    public IEnumerator OpenChest()
    {
        if (!hasBeenOpened)
        {
            // Amount of items, choose what rarity, spawn items
            int TotalItems = Random.Range(MinPossibleItems, MaxPossibleItems + 1);

            int TotValue = 0;

            for (int i = 0; i < PossibleItems.Length; i++)
            {
                TotValue += PossibleItems[i].weight;
            }

            for (int i = 0; i < TotalItems; i++)
            {
                SpawnItem(TotValue);
                yield return new WaitForSeconds(TimeBetweenItems);
            }
            hasBeenOpened = true;
        }
    }
    public void SpawnItem(int TotalValue)
    {
        int Value = Random.Range(1, TotalValue);

        int CurrentValue = 0;
        for (int j = 0; j < PossibleItems.Length; j++)
        {
            CurrentValue += PossibleItems[j].weight;
            if (CurrentValue > Value)
            {
                GameObject item = Instantiate(PossibleItems[j].prefab, SpawnSpot.position, gameObject.transform.rotation);
                if (item.GetComponent<Rigidbody>() == null)
                {
                    item.AddComponent(typeof(Rigidbody));
                }
                Rigidbody RigidB = item.GetComponent<Rigidbody>();

                int x = Random.Range(-ItemSpread, ItemSpread);
                int y = Random.Range(-ItemSpread, ItemSpread);

                RigidB.AddForce(new Vector3(x, ItemLaunchUpAmount, y));
                j = 100;
            }
        }
    }
}
