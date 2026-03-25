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
    [SerializeField] private bool generateOnStart = false;


    private void Start()
    {
        if (generateOnStart) GenerateSphere(size, type);
    }


    public void GenerateSphere(int size, SphereType type = SphereType.CubeSphere, Material material = null)
    {
        this.size = size;
        if (material != null) SetMaterial(material);
        this.type = type;

        switch (type)
        {
            case SphereType.CubeSphere:
                GenerateCubeSphere();
                break;
            case SphereType.UVSphere:
                GenerateUVSphere();
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
        // Information of a cube's faces.
        Vector3[] origins = new Vector3[6]
        {
            new(-1.0f, -1.0f, -1.0f), // Front
            new(1.0f, -1.0f, 1.0f), // Back
            new(-1.0f, -1.0f, 1.0f), // Left
            new(1.0f, -1.0f, -1.0f), // Right
            new(-1.0f, 1.0f, -1.0f), // Top
            new(-1.0f, -1.0f, 1.0f) // Bottom
        };

        // The relative right hand side of a face.
        Vector3[] rights = new Vector3[6]
        {
            Vector3.right,
            Vector3.left,
            Vector3.back,
            Vector3.forward,
            Vector3.right,
            Vector3.right
        };

        // The relative up direction of a face.
        Vector3[] ups = new Vector3[6]
        {
            Vector3.up,
            Vector3.up,
            Vector3.up,
            Vector3.up,
            Vector3.forward,
            Vector3.back
        };

        for (int i = 0; i < 6;  ++i)
        {
            Vector3 origin = origins[i];
            Vector3 right = rights[i];
            Vector3 up = ups[i];
            GenerateSubdivisions(origin, right, up);
        }

        CreateMesh();
    } 


    void GenerateSubdivisions(Vector3 origin, Vector3 right, Vector3 up)
    {
        int subdivisions = size + 1;
        int stride = subdivisions + 1;
        int vertexOffset = VertexCount;

        for (int i = 0; i < stride * stride; ++i)
        {
            float x = 2.0f * (i % stride) / subdivisions;
            float y = 2.0f * (i / stride) / subdivisions;

            Vector3 point = origin + right * x + up * y;
            Vector2 uv = new(x, y);
            AddVertex(0.5f * size * point.normalized, uv);
        }

        for (int i = 0; i < subdivisions * subdivisions; ++i)
        {
            int row = i / subdivisions;
            int column = i % subdivisions;
            
            int vertex0 = vertexOffset + row * stride + column;
            int vertex1 = vertex0 + 1;
            int vertex2 = vertex0 + stride;
            int vertex3 = vertex2 + 1;

            AddQuad(vertex1, vertex0, vertex2, vertex3);
        }
    }
}
