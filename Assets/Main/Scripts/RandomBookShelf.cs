using UnityEngine;

public class RandomBookShelf : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Material[] Materials;
    [SerializeField] private Transform TopLeftShelfPoint;
    [SerializeField] private GameObject BookPrefab;
    [SerializeField] private MeshRenderer BookPrefabRenderer;
    [SerializeField] private MeshRenderer BookShelfMesh;
    [SerializeField] private Transform BookParent;

    [Header("Settings")]
    [SerializeField] private LayerMask BookLayer;
    [SerializeField] private Quaternion DefaultRotation;
    [SerializeField] private int AmountOfShelfs;
    [SerializeField] private float DistanceBetweenShelfs;
    [SerializeField] private float DefaultScale;
    [SerializeField] private float ScaleRandomDifference;
    [SerializeField] private float MinGapSizeBetweenBooks;
    [SerializeField] private float MaxGapSizeBetweenBooks;
    [SerializeField] private int MaxTries = 100;

    private int Tries;
    private int BookCount;

    private Bounds BookShelfBounds;

    private bool ShelfDone;
    private int ShelfCount;

    private float CurrentY;

    private void OnEnable()
    {
        BookShelfBounds = BookShelfMesh.localBounds;

        CurrentY = TopLeftShelfPoint.position.y;

        GenerateBooks();
    }

    private void GenerateBooks()
    {
        while (Tries < MaxTries)
        {
            Tries++;

            //Move on to shelf below current shelf.
            if (ShelfDone)
            {
                CurrentY -= DistanceBetweenShelfs;
                ShelfDone = false;

                if (ShelfCount >= AmountOfShelfs) return;
            }

            //Generate random scale.
            float Scale = Random.Range(-ScaleRandomDifference, ScaleRandomDifference) + DefaultScale;

            Vector3 LocalScale = new(Scale, Scale, Scale);

            BookPrefab.transform.localScale = LocalScale;

            Vector3 WorldScale = BookPrefabRenderer.bounds.size;

            float GapSize = Random.Range(MinGapSizeBetweenBooks, MaxGapSizeBetweenBooks);

            //Generate the position where the book is supposed to be based on the last position of the book with a random distance (gapSize).
            Vector3 GapPosition = new(((BookCount == 0 ? 0 : WorldScale.x) + 0.001f + GapSize) * (BookCount + 1), 0, 0);

            //Adjust for rotation.
            Vector3 RotatedGapPosition = TopLeftShelfPoint.TransformDirection(GapPosition);

            Vector3 Position = TopLeftShelfPoint.position + RotatedGapPosition;
            Position.y = CurrentY;
            
            //Check if you can spawn a book here.
            if (MaySpawn(Position, WorldScale))
            {
                //Actually spawn the book
                BookCount++;
                GenerateBook(Position, LocalScale);
            }

            //Timeout: the program to spawn too many times.
            if (Tries >= MaxTries) return;
        }
    }

    private void GenerateBook(Vector3 Pos, Vector3 Scale)
    {
        //Generate random color/material.
        Material Material = new(Materials[Random.Range(0, Materials.Length)]);

        GameObject SpawnedBook = Instantiate(BookPrefab, Pos, DefaultRotation, BookParent);
        SpawnedBook.transform.localScale = Scale;
        MeshRenderer Mesh = SpawnedBook.GetComponent<MeshRenderer>();
        Mesh.material = Material;
    }

    private bool MaySpawn(Vector3 Pos, Vector3 Scale)
    {
        //Check if current book collides with another book.
        bool FoundAnotherBook = Physics.CheckBox(Pos, Scale / 4, DefaultRotation, BookLayer);

        if (FoundAnotherBook) return false;

        Vector3 LocalPos = transform.InverseTransformPoint(Pos);

        //Check if the right most side of the bounds of the bookshelf has been reached, if so, start spawning on a layer below this one.
        //The 0.0005f is because this check isn't perfect and will sometimes spawn a book outside of the bookshelf anyway.
        if (LocalPos.x >= BookShelfBounds.max.x - 0.0005f)
        {
            ShelfDone = true;
            BookCount = 0;
            ShelfCount++;
            return false;
        }

        return true;
    }
}
