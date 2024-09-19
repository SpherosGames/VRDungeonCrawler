using UnityEngine;

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

            //Renderer bookPrefabRenderer = new();

            Renderer bookPrefabRenderer = bookPrefab.GetComponent<Renderer>();

            //bookPrefabRenderer.transform.localScale = localScale;

            //Bounds localBounds = bookShelfCol.bounds;

            //Bounds worldBounds = bookPrefabRenderer.bounds;

            //localBounds.size = localScale;
            //print(bounds.size);

            //bookPrefabRenderer.localBounds = bounds;

            //bookPrefabRenderer.localBounds = localBounds;

            Vector3 worldScale = bookPrefabRenderer.bounds.size;
            print("World scale" + worldScale);

            float gapSize = Random.Range(minGapSizeBetweenBooks, maxGapSizeBetweenBooks);

            Vector3 position = topLeftShelfPoint.position + new Vector3(((bookCount == 0 ? 0 : worldScale.x) + 0.001f + gapSize) * (bookCount + 1), 0, 0);
            position.y = currentY;

            Debug.DrawRay(position, Vector3.up, Color.blue, 30);

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

    Vector3 lastBoxCenter;
    Vector3 lastBoxSize;
    Quaternion lastBoxRot;

    private bool MaySpawn(Vector3 pos, Quaternion rotation, Vector3 scale)
    {
        pos.y -= scale.y / 2;

        bool foundAnotherBook = Physics.CheckBox(pos, scale / 2, Quaternion.identity, bookLayer);

        lastBoxCenter = pos;
        lastBoxSize = scale;
        lastBoxRot = rotation;

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

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(lastBoxCenter, lastBoxSize);
    }
}
