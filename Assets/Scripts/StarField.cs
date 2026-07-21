using UnityEngine;

public class StarField : MonoBehaviour
{
    [Header("Star Field")]
    [SerializeField] private int starCount = 2000;
    [SerializeField] private float fieldRadius = 200f;
    [SerializeField] private float minStarSize = 0.05f;
    [SerializeField] private float maxStarSize = 0.3f;

    [Header("Parallax")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float parallaxFactor = 0.1f;

    [Header("Appearance")]
    [SerializeField] private Color starColor = new Color(0.8f, 0.85f, 1f, 1f);
    [SerializeField] private float twinkleSpeed = 2f;
    [SerializeField] private float twinkleIntensity = 0.3f;

    private Vector3[] starPositions;
    private Vector3[] originalPositions;
    private Material starMaterial;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        if (cameraTransform == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
                cameraTransform = mainCam.transform;
        }

        GenerateStarField();
    }

    private void Update()
    {
        if (cameraTransform == null || starPositions == null)
            return;

        ApplyParallax();
        ApplyTwinkle();
    }

    private void GenerateStarField()
    {
        meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();

        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

        starMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        starMaterial.color = starColor;
        starMaterial.enableInstancing = true;
        meshRenderer.material = starMaterial;

        starPositions = new Vector3[starCount];
        originalPositions = new Vector3[starCount];

        Mesh mesh = new Mesh();
        mesh.name = "StarField";

        int vertexCount = starCount * 4;
        int indexCount = starCount * 6;

        Vector3[] vertices = new Vector3[vertexCount];
        Vector2[] uvs = new Vector2[vertexCount];
        int[] indices = new int[indexCount];
        Color[] colors = new Color[vertexCount];

        for (int i = 0; i < starCount; i++)
        {
            Vector3 pos = Random.onUnitSphere * fieldRadius;
            starPositions[i] = pos;
            originalPositions[i] = pos;

            float size = Random.Range(minStarSize, maxStarSize);
            Vector3 up = Vector3.up * size;
            Vector3 right = Vector3.right * size;

            int vi = i * 4;
            vertices[vi] = pos - right - up;
            vertices[vi + 1] = pos + right - up;
            vertices[vi + 2] = pos + right + up;
            vertices[vi + 3] = pos - right + up;

            uvs[vi] = new Vector2(0, 0);
            uvs[vi + 1] = new Vector2(1, 0);
            uvs[vi + 2] = new Vector2(1, 1);
            uvs[vi + 3] = new Vector2(0, 1);

            float brightness = Random.Range(0.5f, 1f);
            Color c = starColor * brightness;
            c.a = 1f;
            colors[vi] = c;
            colors[vi + 1] = c;
            colors[vi + 2] = c;
            colors[vi + 3] = c;

            int ii = i * 6;
            indices[ii] = vi;
            indices[ii + 1] = vi + 2;
            indices[ii + 2] = vi + 1;
            indices[ii + 3] = vi;
            indices[ii + 4] = vi + 3;
            indices[ii + 5] = vi + 2;
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.colors = colors;
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
    }

    private void ApplyParallax()
    {
        Vector3 cameraOffset = cameraTransform.position * parallaxFactor;
        transform.position = cameraOffset;
    }

    private void ApplyTwinkle()
    {
        if (meshFilter == null || meshFilter.mesh == null)
            return;

        Mesh mesh = meshFilter.mesh;
        Color[] colors = mesh.colors;

        if (colors == null || colors.Length == 0)
            return;

        float time = Time.time * twinkleSpeed;

        for (int i = 0; i < starCount; i++)
        {
            float offset = (float)i / starCount * 6.28f;
            float brightness = 0.7f + Mathf.Sin(time + offset) * twinkleIntensity * 0.5f;

            int vi = i * 4;
            if (vi + 3 >= colors.Length)
                break;

            Color c = starColor * brightness;
            c.a = 1f;
            colors[vi] = c;
            colors[vi + 1] = c;
            colors[vi + 2] = c;
            colors[vi + 3] = c;
        }

        mesh.colors = colors;
    }

    private void OnDestroy()
    {
        if (starMaterial != null)
            Destroy(starMaterial);
    }
}
