using UnityEngine;

public class StarPulse : MonoBehaviour
{
    [Header("Pulse Settings")]
    [SerializeField] private int pulseStarCount = 20;
    [SerializeField] private float spawnRadius = 100f;
    [SerializeField] private float starSize = 0.5f;

    [Header("Animation")]
    [SerializeField] private float minPulseSpeed = 0.5f;
    [SerializeField] private float maxPulseSpeed = 3f;
    [SerializeField] private float minPulseIntensity = 1f;
    [SerializeField] private float maxPulseIntensity = 5f;
    [SerializeField] private float minRange = 3f;
    [SerializeField] private float maxRange = 10f;

    [Header("Colors")]
    [SerializeField] private Color[] starColors = new Color[]
    {
        new Color(1f, 0.9f, 0.7f, 1f),
        new Color(0.7f, 0.85f, 1f, 1f),
        new Color(1f, 0.7f, 0.5f, 1f),
        new Color(0.9f, 0.9f, 1f, 1f),
        new Color(1f, 1f, 0.8f, 1f)
    };

    private Light[] pulseStars;
    private float[] pulseSpeeds;
    private float[] pulsePhases;
    private float[] baseIntensities;

    private void Start()
    {
        CreatePulseStars();
    }

    private void Update()
    {
        if (pulseStars == null)
            return;

        for (int i = 0; i < pulseStars.Length; i++)
        {
            if (pulseStars[i] == null)
                continue;

            float pulse = Mathf.Sin(Time.time * pulseSpeeds[i] + pulsePhases[i]);
            float t = (pulse + 1f) * 0.5f;
            pulseStars[i].intensity = Mathf.Lerp(minPulseIntensity, maxPulseIntensity, t) * baseIntensities[i];
        }
    }

    private void CreatePulseStars()
    {
        pulseStars = new Light[pulseStarCount];
        pulseSpeeds = new float[pulseStarCount];
        pulsePhases = new float[pulseStarCount];
        baseIntensities = new float[pulseStarCount];

        for (int i = 0; i < pulseStarCount; i++)
        {
            Vector3 pos = Random.onUnitSphere * spawnRadius;

            GameObject starObj = new GameObject($"PulseStar_{i}");
            starObj.transform.position = pos;

            Light light = starObj.AddComponent<Light>();
            light.type = LightType.Point;
            light.intensity = minPulseIntensity;
            light.range = Random.Range(minRange, maxRange);

            Color color = starColors[Random.Range(0, starColors.Length)];
            light.color = color;

            GameObject glowObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
            glowObj.name = "Glow";
            glowObj.transform.SetParent(starObj.transform);
            glowObj.transform.localPosition = Vector3.zero;
            glowObj.transform.localScale = Vector3.one * starSize;

            Object.Destroy(glowObj.GetComponent<Collider>());

            MeshRenderer mr = glowObj.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                mr.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
                mr.material.color = color;
                mr.material.SetFloat("_Surface", 1);
                mr.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mr.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mr.material.SetInt("_ZWrite", 0);
                mr.material.renderQueue = 3100;
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            pulseStars[i] = light;
            pulseSpeeds[i] = Random.Range(minPulseSpeed, maxPulseSpeed);
            pulsePhases[i] = Random.Range(0f, 6.28f);
            baseIntensities[i] = Random.Range(0.7f, 1.3f);
        }
    }

    private void OnDestroy()
    {
        if (pulseStars == null)
            return;

        foreach (var star in pulseStars)
        {
            if (star != null)
                Destroy(star.gameObject);
        }
    }
}
