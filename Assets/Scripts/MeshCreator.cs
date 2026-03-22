using System.Collections.Generic;
using UnityEngine;

public class MeshCreator : MonoBehaviour
{
    public struct Triangle
    {
        public int vertex0, vertex1, vertex2;

        public Triangle(int vertex0, int vertex1, int vertex2)
        {
            this.vertex0 = vertex0;
            this.vertex1 = vertex1;
            this.vertex2 = vertex2;
        }
    }

    protected MeshFilter Filter => filter;

    private MeshFilter filter = null;
    private Mesh mesh = null;
    private List<Vector3> vertices = null;
    private List<Vector2> uvs = null;
    private List<Triangle> triangles = null;


    private void Awake()
    {
        // Makes sure the object is initalized properly.
        vertices = new List<Vector3>();
        uvs = new List<Vector2>();
        triangles = new List<Triangle>();

        if (!TryGetComponent(out filter))
            filter = gameObject.AddComponent<MeshFilter>();

        mesh = new();
        filter.mesh = mesh;

        if (!TryGetComponent(out MeshRenderer _))
            gameObject.AddComponent<MeshRenderer>();
    }


    private void Start()
    {
        int v0 = AddVertex(new Vector3(0, 0, 0), new Vector2(0, 0));
        int v1 = AddVertex(new Vector3(0, 1, 1), new Vector2(0, 1));
        int v2 = AddVertex(new Vector3(1, 0, 0), new Vector2(1, 0));
        int v3 = AddVertex(new Vector3(1, 1, 1), new Vector2(1, 1));

        // Two triangles that are connected and are a quad (are smooth).
        AddTriangle(v0, v1, v2);
        AddTriangle(v1, v3, v2);

        // A triangle that is separate which makes it sharp.
        AddTriangle(new Vector3(1, 1, 1), new Vector3(2, 1, 1), new Vector3(1, 0, 0));

        CreateMesh();
    }


    protected void CreateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = TrianglesToInt();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        filter.mesh = mesh;
    }


    private int[] TrianglesToInt()
    {
        int[] intTriangles = new int[triangles.Count * 3];
        int index = 0;
        foreach (Triangle triangle in triangles)
        {
            intTriangles[index++] = triangle.vertex0;
            intTriangles[index++] = triangle.vertex1;
            intTriangles[index++] = triangle.vertex2;
        }
        return intTriangles;
    }


    protected int AddVertex(Vector3 vertex)
    {
        vertices.Add(vertex);
        uvs.Add(Vector2.zero);
        return vertices.Count - 1;
    }


    protected int AddVertex(Vector3 vertex, Vector2 uv)
    {
        vertices.Add(vertex);
        uvs.Add(uv);
        return vertices.Count - 1;
    }


    protected void AddTriangle(int vertex0, int vertex1, int vertex2)
    {
        if (vertex0 >= vertices.Count) return;
        if (vertex1 >= vertices.Count) return;
        if (vertex2 >= vertices.Count) return;

        Triangle triangle = new(vertex0, vertex1, vertex2);
        triangles.Add(triangle);
    }


    protected void AddTriangle(Vector3 point0, Vector3 point1, Vector3 point2)
    {
        Triangle triangle = new()
        {
            vertex0 = AddVertex(point0),
            vertex1 = AddVertex(point1),
            vertex2 = AddVertex(point2)
        };
        triangles.Add(triangle);
    }
}
