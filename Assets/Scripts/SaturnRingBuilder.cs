using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SaturnRingBuilder : MonoBehaviour
{
    public float innerRadius = 0.13f;
    public float outerRadius = 0.22f;
    public int segments = 64;
    public Material ringMaterial;

    void Start()
    {
        BuildRing();
    }

    void BuildRing()
    {
        GameObject ring = new GameObject("SaturnRing");
        ring.transform.SetParent(transform);
        ring.transform.localPosition = Vector3.zero;
        ring.transform.localRotation = Quaternion.Euler(10f, 0f, 0f);
        ring.transform.localScale = Vector3.one;

        MeshFilter mf = ring.AddComponent<MeshFilter>();
        MeshRenderer mr = ring.AddComponent<MeshRenderer>();

        if (ringMaterial != null)
            mr.material = ringMaterial;

        mf.mesh = CreateRingMesh();
    }

    Mesh CreateRingMesh()
    {
        Mesh mesh = new Mesh();
        int vertCount = (segments + 1) * 2;
        Vector3[] verts = new Vector3[vertCount];
        Vector2[] uvs = new Vector2[vertCount];
        int[] tris = new int[segments * 6];

        for (int i = 0; i <= segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2f;
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            verts[i * 2] = new Vector3(cos * innerRadius, 0f, sin * innerRadius);
            verts[i * 2 + 1] = new Vector3(cos * outerRadius, 0f, sin * outerRadius);

            float u = (float)i / segments;
            uvs[i * 2] = new Vector2(u, 0f);
            uvs[i * 2 + 1] = new Vector2(u, 1f);
        }

        for (int i = 0; i < segments; i++)
        {
            int start = i * 6;
            int v = i * 2;
            tris[start] = v;
            tris[start + 1] = v + 2;
            tris[start + 2] = v + 1;
            tris[start + 3] = v + 1;
            tris[start + 4] = v + 2;
            tris[start + 5] = v + 3;
        }

        mesh.vertices = verts;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        return mesh;
    }
}