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
        GenerateUVSphere();
    }


    private void GenerateUVSphere()
    {
        // Top cap.
        AddVertex(Vector3.up * size);

        // Generates the strips of the sphere.
        for (int i = 1; i < size; ++i)
        {
            float t0 = i / (float)size;
            float y = Mathf.Cos(t0 * Mathf.PI);
            for (int j = 0; j < size; ++j)
            {
                float t1 = j / (float)size;
                float x = Mathf.Sin(t1 * Mathf.PI * 2.0f) * Mathf.Sin(t0 * Mathf.PI);
                float z = Mathf.Cos(t1 * Mathf.PI * 2.0f) * Mathf.Sin(t0 * Mathf.PI);
                Vector3 position = new(x, y, z);
                AddVertex(position.normalized * size);
            }
        }

        // Bottom cap.
        AddVertex(Vector3.down * size);

        // Connect the top cap.
        for (int i = 0; i < size; ++i)
        {
            int vertex0 = 0;
            int vertex1 = i + 1;
            int vertex2 = (i + 1) % size + 1;
            AddTriangle(vertex0, vertex1, vertex2);
        }

        // Connect the strips.
        for (int i = 0; i < VertexCount - 2; ++i)
        {
            int vertex0 = i;
            int vertex1 = ((i + 1) % size) + size * (i / size);
            int vertex2 = vertex1 + size;
            int vertex3 = vertex0 + size;
            AddQuad(vertex1 + 1, vertex0 + 1, vertex3 + 1, vertex2 + 1);
        }

        // Same as the top but reversed.
        int lastVertex = VertexCount - 1;
        for (int i = 0; i < size; ++i)
        {
            int vertex0 = lastVertex;
            int vertex1 = lastVertex - (i + 1);
            int vertex2 = lastVertex - ((i + 1) % size + 1);
            AddTriangle(vertex0, vertex1, vertex2);
        }

        CreateMesh();
    }
}
