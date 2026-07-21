using UnityEngine;

public class NebulaEffect : MonoBehaviour
{
    [Header("Nebula Layers")]
    [SerializeField] private int layerCount = 3;
    [SerializeField] private float layerRadius = 180f;
    [SerializeField] private float layerSize = 80f;

    [Header("Colors")]
    [SerializeField] private Color[] nebulaColors = new Color[]
    {
        new Color(0.2f, 0.0f, 0.4f, 0.08f),
        new Color(0.0f, 0.1f, 0.3f, 0.06f),
        new Color(0.3f, 0.0f, 0.2f, 0.07f)
    };

    [Header("Animation")]
    [SerializeField] private float rotationSpeed = 0.5f;
    [SerializeField] private float pulseSpeed = 0.3f;
    [SerializeField] private float pulseIntensity = 0.15f;

    private GameObject[] nebulaLayers;

    private void Start()
    {
        CreateNebulaLayers();
    }

    private void Update()
    {
        if (nebulaLayers == null)
            return;

        for (int i = 0; i < nebulaLayers.Length; i++)
        {
            if (nebulaLayers[i] == null)
                continue;

            float rotSpeed = rotationSpeed * (i % 2 == 0 ? 1f : -1f);
            nebulaLayers[i].transform.Rotate(Vector3.forward, rotSpeed * Time.deltaTime);

            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed + i * 2.1f) * pulseIntensity;
            nebulaLayers[i].transform.localScale = Vector3.one * layerSize * pulse;
        }
    }

    private void CreateNebulaLayers()
    {
        nebulaLayers = new GameObject[layerCount];

        for (int i = 0; i < layerCount; i++)
        {
            GameObject layer = new GameObject($"NebulaLayer_{i}");
            layer.transform.SetParent(transform);
            layer.transform.localPosition = Random.onUnitSphere * layerRadius * 0.3f;

            MeshFilter mf = layer.AddComponent<MeshFilter>();
            MeshRenderer mr = layer.AddComponent<MeshRenderer>();

            mf.mesh = CreateQuadMesh();

            Color c = nebulaColors[i % nebulaColors.Length];

            Material mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            mat.color = c;
            mat.SetFloat("_Surface", 1);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mat.SetInt("_ZWrite", 0);
            mat.renderQueue = 3100;
            mr.material = mat;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.receiveShadows = false;

            nebulaLayers[i] = layer;
        }
    }

    private Mesh CreateQuadMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "NebulaQuad";

        mesh.vertices = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, 0),
            new Vector3(0.5f, -0.5f, 0),
            new Vector3(0.5f, 0.5f, 0),
            new Vector3(-0.5f, 0.5f, 0)
        };

        mesh.uv = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1)
        };

        mesh.triangles = new int[] { 0, 2, 1, 0, 3, 2 };
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    private void OnDestroy()
    {
        if (nebulaLayers == null)
            return;

        foreach (var layer in nebulaLayers)
        {
            if (layer == null)
                continue;

            if (layer.GetComponent<MeshRenderer>()?.material != null)
                Destroy(layer.GetComponent<MeshRenderer>().material);

            Destroy(layer);
        }
    }
}
