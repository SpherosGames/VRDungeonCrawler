using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBookShelf : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Material[] materials;
    [SerializeField] private Transform leftMostPointOnShelf1;

    [Header("Settings")]
    [SerializeField] private GameObject bookPrefab;

    private void OnEnable()
    {
        GenerateBooks();
    }

    private void GenerateBooks()
    {
        Instantiate(bookPrefab, leftMostPointOnShelf1.position, bookPrefab.transform.rotation, transform);
    }
}
