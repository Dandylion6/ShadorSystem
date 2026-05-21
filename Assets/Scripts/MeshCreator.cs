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


    [Header("Mesh Settings")]
    [SerializeField] private Material material = null;
    [SerializeField] private bool gizmoOnSelect = false;


    public MeshFilter Filter => filter;
    public MeshRenderer Renderer => renderer;
    public Mesh Mesh => mesh;
    protected int VertexCount => vertices.Count;

    private MeshFilter filter = null;
    private new MeshRenderer renderer = null;
    private Mesh mesh = null;
    private List<Vector3> vertices = null;
    private List<Vector3> normals = null;
    private List<Vector2> uvs = null;
    private List<Triangle> triangles = null;


    public void SetMaterial(Material material) => this.material = material;


    private void Awake()
    {
        // Makes sure the object is initalized properly.
        vertices = new();
        normals = new();
        uvs = new();
        triangles = new();

        if (!TryGetComponent(out filter))
            filter = gameObject.AddComponent<MeshFilter>();

        mesh = new();
        filter.mesh = mesh;

        if (!TryGetComponent(out MeshRenderer _))
            renderer = gameObject.AddComponent<MeshRenderer>();

        renderer.material = material;
    }


    protected void CreateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = TrianglesToInt();

        mesh.RecalculateBounds();
        if (normals.Count == 0)
        {
            mesh.RecalculateNormals();
        }
        filter.mesh = mesh;
    }


    protected Vector3 GetVertex(int index)
    {
        if (index >= vertices.Count) return Vector3.zero;
        return vertices[index];
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


    protected int AddVertex(Vector3 vertex, Vector2 uv, Vector3 normal)
    {
        vertices.Add(vertex);
        uvs.Add(uv);
        normals.Add(normal);
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


    protected void AddQuad(int vertex0, int vertex1, int vertex2, int vertex3)
    {
        if (vertex0 >= vertices.Count) return;
        if (vertex1 >= vertices.Count) return;
        if (vertex2 >= vertices.Count) return;
        if (vertex3 >= vertices.Count) return;

        Triangle triangle1 = new(vertex0, vertex1, vertex2);
        Triangle triangle2 = new(vertex0, vertex2, vertex3);

        triangles.Add(triangle1);
        triangles.Add(triangle2);
    }


    private void OnDrawGizmosSelected()
    {
        if (!gizmoOnSelect) return;
        if (vertices == null) return;

        Gizmos.color = Color.yellow;
        foreach (Vector3 vertex in vertices)
        {
            Gizmos.DrawSphere(transform.TransformPoint(vertex), 0.1f);
        }
    }
}
