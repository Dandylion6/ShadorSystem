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
            float y = Mathf.Cos(t0 * Mathf.PI) * size;
            for (int j = 0; j < size; ++j)
            {
                float t1 = j / (float)size;
                float x = Mathf.Sin(t1 * Mathf.PI * 2.0f) * Mathf.Sin(t0 * Mathf.PI) * size;
                float z = Mathf.Cos(t1 * Mathf.PI * 2.0f) * Mathf.Sin(t0 * Mathf.PI) * size;
                AddVertex(new Vector3(x, y, z));
            }
        }

        // Bottom cap.
        AddVertex(Vector3.down * size);

        // Connect the vertices.

        // Connect the top cap.
        for (int i = 0; i < size; ++i)
        {
            int vertex0 = 0;
            int vertex1 = i + 1;
            int vertex2 = (i + 1) % size + 1;
            AddTriangle(vertex0, vertex1, vertex2);
        }

        for (int i = 1; i < VertexCount - i; ++i)
        {

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
