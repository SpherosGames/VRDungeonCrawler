using System.Collections.Generic;
using UnityEngine;

public class RandomBookShelf : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Material[] materials;
    [SerializeField] private Transform topLeftShelfPoint;
    [SerializeField] private GameObject bookPrefab;
    [SerializeField] private MeshRenderer bookPrefabRenderer;
    [SerializeField] private MeshRenderer bookShelfMesh;
    [SerializeField] private Transform bookParent;

    [Header("Settings")]
    [SerializeField] private LayerMask bookLayer;
    [SerializeField] private Quaternion defaultRotation;
    [SerializeField] private int amountOfShelfs;
    [SerializeField] private float distanceBetweenShelfs;
    [SerializeField] private float defaultScale;
    [SerializeField] private float scaleRandomDifference;
    [SerializeField] private float minGapSizeBetweenBooks;
    [SerializeField] private float maxGapSizeBetweenBooks;
    [SerializeField] private int maxTries = 100;

    private int tries;
    private int bookCount;

    private Bounds bookShelfBounds;

    private bool shelfDone;
    private int shelfCount;

    private float currentY;

    private void OnEnable()
    {
        bookShelfBounds = bookShelfMesh.localBounds;

        currentY = topLeftShelfPoint.position.y;

        GenerateBooks();
    }

    private void GenerateBooks()
    {
        while (tries < maxTries)
        {
            tries++;

            //Move on to shelf below current shelf
            if (shelfDone)
            {
                currentY -= distanceBetweenShelfs;
                shelfDone = false;

                if (shelfCount >= amountOfShelfs) return;
            }

            //Generate random scale
            float scale = Random.Range(-scaleRandomDifference, scaleRandomDifference) + defaultScale;

            Vector3 localScale = new(scale, scale, scale);

            bookPrefab.transform.localScale = localScale;

            Vector3 worldScale = bookPrefabRenderer.bounds.size;

            float gapSize = Random.Range(minGapSizeBetweenBooks, maxGapSizeBetweenBooks);

            //Generate the position where the book is supposed to be based on the last position of the book with a random distance (gapSize)
            Vector3 gapPosition = new(((bookCount == 0 ? 0 : worldScale.x) + 0.001f + gapSize) * (bookCount + 1), 0, 0);

            //Adjust for rotation
            Vector3 rotatedGapPosition = topLeftShelfPoint.TransformDirection(gapPosition);

            Vector3 position = topLeftShelfPoint.position + rotatedGapPosition;
            position.y = currentY;
            
            //Check if you can spawn a book here
            if (MaySpawn(position, worldScale))
            {
                //Actually spawn the book
                bookCount++;
                GenerateBook(position, localScale);
            }

            //Timeout: the program to spawn too many times
            if (tries >= maxTries) return;
        }
    }

    private void GenerateBook(Vector3 pos, Vector3 scale)
    {
        //Generate random color/material
        Material material = new(materials[Random.Range(0, materials.Length)]);

        GameObject spawnedBook = Instantiate(bookPrefab, pos, defaultRotation, bookParent);
        spawnedBook.transform.localScale = scale;
        MeshRenderer mesh = spawnedBook.GetComponent<MeshRenderer>();
        mesh.material = material;
    }

    private bool MaySpawn(Vector3 pos, Vector3 scale)
    {
        //Check if current book collides with another book
        bool foundAnotherBook = Physics.CheckBox(pos, scale / 4, defaultRotation, bookLayer);

        if (foundAnotherBook) return false;

        Vector3 localPos = transform.InverseTransformPoint(pos);

        //Check if the right most side of the bounds of the bookshelf has been reached, if so, start spawning on a layer below this one
        //The 0.0005f, is because this check isn't perfect and will sometimes spawn a book outside of the bookshelf anyway
        if (localPos.x >= bookShelfBounds.max.x - 0.0005f)
        {
            shelfDone = true;
            bookCount = 0;
            shelfCount++;
            return false;
        }

        return true;
    }
}
