using UnityEngine;

public class DynamicSpaceBackground : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    [Header("Sprite Layers")]
    [SerializeField] private Sprite starsSmall;
    [SerializeField] private Sprite starsBig;
    [SerializeField] private Sprite nebulaRed;
    [SerializeField] private Sprite nebulaBlue;
    [SerializeField] private Sprite nebulaAquaPink;

    [Header("Layer Depth")]
    [SerializeField] private float starsSmallDepth = -80f;
    [SerializeField] private float starsBigDepth = -60f;
    [SerializeField] private float nebulaRedDepth = -120f;
    [SerializeField] private float nebulaBlueDepth = -140f;
    [SerializeField] private float nebulaAquaPinkDepth = -100f;

    [Header("Layer Scale")]
    [SerializeField] private float starsSmallScale = 200f;
    [SerializeField] private float starsBigScale = 180f;
    [SerializeField] private float nebulaScale = 250f;

    [Header("Parallax")]
    [SerializeField] private float starsSmallParallax = 0.02f;
    [SerializeField] private float starsBigParallax = 0.05f;
    [SerializeField] private float nebulaParallax = 0.01f;

    [Header("Animation")]
    [SerializeField] private float nebulaRotationSpeed = 0.3f;
    [SerializeField] private float nebulaPulseSpeed = 0.2f;
    [SerializeField] private float nebulaPulseIntensity = 0.15f;
    [SerializeField] private float nebulaDriftSpeed = 0.1f;

    private GameObject[] layerObjects;
    private Vector3[] originalPositions;
    private float[] parallaxFactors;
    private MeshRenderer[] layerRenderers;

    private void Start()
    {
        if (cameraTransform == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
                cameraTransform = mainCam.transform;
        }

        CreateLayers();
    }

    private void Update()
    {
        if (cameraTransform == null || layerObjects == null)
            return;

        for (int i = 0; i < layerObjects.Length; i++)
        {
            if (layerObjects[i] == null)
                continue;

            Vector3 cameraOffset = cameraTransform.position * parallaxFactors[i];
            layerObjects[i].transform.position = originalPositions[i] + cameraOffset;

            if (i >= 2)
            {
                float time = Time.time;
                float rotAngle = nebulaRotationSpeed * Time.deltaTime * (i % 2 == 0 ? 1f : -1f);
                layerObjects[i].transform.Rotate(Vector3.forward, rotAngle);

                float drift = Mathf.Sin(time * nebulaDriftSpeed + i * 1.5f) * 2f;
                Vector3 driftOffset = new Vector3(drift, Mathf.Sin(time * nebulaDriftSpeed * 0.7f + i) * 1.5f, 0f);
                layerObjects[i].transform.position += driftOffset;

                if (layerRenderers[i] != null && layerRenderers[i].material != null)
                {
                    float pulse = 1f + Mathf.Sin(time * nebulaPulseSpeed + i * 2.3f) * nebulaPulseIntensity;
                    layerRenderers[i].material.color = new Color(1f, 1f, 1f, 0.4f * pulse);
                }
            }
        }
    }

    private void CreateLayers()
    {
        layerObjects = new GameObject[5];
        originalPositions = new Vector3[5];
        parallaxFactors = new float[5];
        layerRenderers = new MeshRenderer[5];

        CreateLayer("StarsSmall", starsSmall, starsSmallDepth, starsSmallScale,
            starsSmallParallax, Color.white, 0.6f, 0);
        CreateLayer("StarsBig", starsBig, starsBigDepth, starsBigScale,
            starsBigParallax, Color.white, 0.8f, 1);
        CreateLayer("NebulaRed", nebulaRed, nebulaRedDepth, nebulaScale,
            nebulaParallax, Color.white, 0.4f, 2);
        CreateLayer("NebulaBlue", nebulaBlue, nebulaBlueDepth, nebulaScale,
            nebulaParallax * 0.8f, Color.white, 0.35f, 3);
        CreateLayer("NebulaAquaPink", nebulaAquaPink, nebulaAquaPinkDepth, nebulaScale,
            nebulaParallax * 0.6f, Color.white, 0.38f, 4);
    }

    private void CreateLayer(string layerName, Sprite sprite, float depth, float scale,
        float parallax, Color tint, float alpha, int index)
    {
        GameObject layerObj = new GameObject(layerName);
        layerObj.transform.SetParent(transform);
        layerObj.transform.localPosition = new Vector3(0f, 0f, depth);
        layerObj.transform.localRotation = Quaternion.identity;
        layerObj.transform.localScale = Vector3.one * scale;

        MeshFilter mf = layerObj.AddComponent<MeshFilter>();
        MeshRenderer mr = layerObj.AddComponent<MeshRenderer>();

        mf.mesh = CreateQuadMesh();

        Material mat;

        if (sprite != null)
        {
            mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            mat.mainTexture = sprite.texture;
        }
        else
        {
            mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            mat.color = new Color(tint.r, tint.g, tint.b, alpha);
        }

        mat.SetFloat("_Surface", 1);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.SetInt("_ZWrite", 0);
        mat.renderQueue = 3000;

        Color matColor = mat.color;
        matColor.a = alpha;
        mat.color = matColor;

        mr.material = mat;
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mr.receiveShadows = false;

        layerObjects[index] = layerObj;
        originalPositions[index] = layerObj.transform.position;
        parallaxFactors[index] = parallax;
        layerRenderers[index] = mr;
    }

    private Mesh CreateQuadMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "BackgroundQuad";

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
        if (layerObjects == null)
            return;

        foreach (var obj in layerObjects)
        {
            if (obj == null)
                continue;

            MeshRenderer mr = obj.GetComponent<MeshRenderer>();
            if (mr != null && mr.material != null)
                Destroy(mr.material);

            Destroy(obj);
        }
    }
}
