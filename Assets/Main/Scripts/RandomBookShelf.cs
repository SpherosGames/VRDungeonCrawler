using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class RandomBookShelf : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Material[] materials;
    [SerializeField] private Transform topLeftShelfPoint;
    [SerializeField] private GameObject bookPrefab;
    [SerializeField] private BoxCollider bookShelfCol;

    [Header("Settings")]
    [SerializeField] private LayerMask bookLayer;
    [SerializeField] private Quaternion defaultRotation;
    [SerializeField] private Quaternion layingRotation;
    [SerializeField] private Quaternion leftRotation;
    [SerializeField] private Quaternion rightRotation;
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
        bookShelfBounds = bookShelfCol.bounds;

        currentY = topLeftShelfPoint.position.y;

        GenerateBooks();
    }

    private void GenerateBooks()
    {
        while (tries < maxTries)
        {
            tries++;

            if (shelfDone)
            {
                print("Calculate new y pos");
                currentY -= distanceBetweenShelfs;
                shelfDone = false;

                if (shelfCount >= amountOfShelfs) return;
            }

            float scale = Random.Range(-scaleRandomDifference, scaleRandomDifference) + defaultScale;

            Vector3 localScale = new(scale, scale, scale);

            bookPrefab.transform.localScale = localScale;

            //Improve this to make sure that the prefab size isn't changed
            Renderer bookPrefabRenderer = bookPrefab.GetComponent<Renderer>();

            Vector3 worldScale = bookPrefabRenderer.bounds.size;

            float gapSize = Random.Range(minGapSizeBetweenBooks, maxGapSizeBetweenBooks);

            Vector3 position = topLeftShelfPoint.position + new Vector3(((bookCount == 0 ? 0 : worldScale.x) + 0.001f + gapSize) * (bookCount + 1), 0, 0);
            position.y = currentY;

            Debug.DrawRay(position, Vector3.up, Color.blue, 30);

            int randomRot = Random.Range(0, 4);

            Quaternion rotation = randomRot switch
            {
                0 => defaultRotation,
                1 => layingRotation,
                2 => leftRotation,
                3 => rightRotation,
                _ => defaultRotation,
            };

            if (MaySpawn(position, defaultRotation, worldScale))
            {
                print("Spawned book");
                bookCount++;
                GenerateBook(position, defaultRotation, localScale);
            }
            else
            {
                print("Failed to spawn book...");
            }

            if (tries >= maxTries)
            {
                print("Timeout");
                return;
            }
        }
    }

    private void GenerateBook(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        Material material = new(materials[Random.Range(0, materials.Length)]);

        GameObject spawnedBook = Instantiate(bookPrefab, pos, rot);
        spawnedBook.transform.localScale = scale;
        Rigidbody rb = spawnedBook.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        MeshRenderer mesh = spawnedBook.GetComponent<MeshRenderer>();
        mesh.material = material;
    }

    List<Matrix4x4> bookShelfsGoBrr = new();

    private bool MaySpawn(Vector3 pos, Quaternion rotation, Vector3 scale)
    {
        pos.y -= scale.y / 2;

        bool foundAnotherBook = Physics.CheckBox(pos, scale / 2, Quaternion.identity, bookLayer);

        bookShelfsGoBrr.Add(Matrix4x4.TRS(pos, rotation, scale));

        print("Found another book: " + foundAnotherBook);

        if (foundAnotherBook) return false;

        if (pos.x >= bookShelfBounds.max.x)
        {
            shelfDone = true;
            bookCount = 0;
            shelfCount++;
            return false;
        }

        return true;
    }

    //private void OnDrawGizmos()
    //{
    //    for (int i = 0; i < bookShelfsGoBrr.Count; i++)
    //    {
    //        Gizmos.matrix = bookShelfsGoBrr[i];
    //        Gizmos.DrawCube(Vector3.zero, Vector3.one);
    //    }
    //}
}
