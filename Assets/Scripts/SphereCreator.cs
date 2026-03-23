using UnityEngine;

public class SphereCreator : MeshCreator
{
    public enum SphereType
    {
        CubeSphere,
        UVSphere
    }


    [Header("Generation Settings")]
    [SerializeField] private SphereType type = SphereType.UVSphere;
    [SerializeField] private int size = 6;

    void Start()
    {
        switch (type)
        {
            case SphereType.CubeSphere: GenerateCubeSphere(); 
                break;
            case SphereType.UVSphere: GenerateUVSphere();
                break;
        }
    }


    private void GenerateUVSphere()
    {
        // Top cap.
        AddVertex(Vector3.up * size, Vector2.up);
        int columnSize = size * 2;

        // Generates the strips of the sphere.
        for (int i = 1; i < size; ++i)
        {
            float t0 = i / (float)size;
            float y = Mathf.Cos(t0 * Mathf.PI);
            for (int j = 0; j < columnSize; ++j)
            {
                float t1 = j / (size * 2.0f);
                float x = Mathf.Sin(t1 * Mathf.PI * 2.0f) * Mathf.Sin(t0 * Mathf.PI);
                float z = Mathf.Cos(t1 * Mathf.PI * 2.0f) * Mathf.Sin(t0 * Mathf.PI);
                Vector3 position = new(x, y, z);
                AddVertex(position * size, new Vector2(t1, 1.0f - t0));
            }
        }

        // Bottom cap.
        AddVertex(Vector3.down * size, Vector2.zero);

        // Connect the top cap.
        for (int i = 0; i < columnSize; ++i)
        {
            int vertex0 = 0;
            int vertex1 = i + 1;
            int vertex2 = (i + 1) % columnSize + 1;
            AddTriangle(vertex0, vertex1, vertex2);
        }

        // Connect the strips.
        for (int i = 0; i < VertexCount - (size * 2 + 2); ++i)
        {
            int vertex0 = i + 1;
            int vertex1 = 1 + ((i + 1) % columnSize) + columnSize * (i / columnSize);
            int vertex2 =  vertex1 + columnSize;
            int vertex3 =  vertex0 + columnSize;
            AddQuad(vertex1, vertex0, vertex3, vertex2);
        }

        // Same as the top but reversed.
        int lastVertex = VertexCount - 1;
        for (int i = 0; i < columnSize; ++i)
        {
            int vertex0 = lastVertex;
            int vertex1 = lastVertex - (i + 1);
            int vertex2 = lastVertex - ((i + 1) % columnSize + 1);
            AddTriangle(vertex0, vertex1, vertex2);
        }

        CreateMesh();
    }


    private void GenerateCubeSphere()
    {
    } 
}
