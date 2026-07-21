using UnityEngine;

public class ProjectileImpact : MonoBehaviour
{
    [Header("Explosion")]
    [SerializeField] private int explosionParticles = 30;
    [SerializeField] private float explosionSize = 2f;
    [SerializeField] private float explosionLifetime = 0.8f;

    [Header("Colors")]
    [SerializeField] private Color flashColor = new Color(1f, 0.8f, 0.3f, 1f);
    [SerializeField] private Color coreColor = new Color(1f, 0.5f, 0.1f, 1f);
    [SerializeField] private Color edgeColor = new Color(1f, 0.2f, 0.05f, 0.6f);

    [Header("Shockwave")]
    [SerializeField] private bool createShockwave = true;
    [SerializeField] private float shockwaveSize = 4f;
    [SerializeField] private float shockwaveSpeed = 10f;

    private void Awake()
    {
        CreateExplosion(transform.position);
        Destroy(gameObject, 0.1f);
    }

    private void CreateExplosion(Vector3 position)
    {
        GameObject explosionObj = new GameObject("Explosion");
        explosionObj.transform.position = position;

        ParticleSystem ps = explosionObj.AddComponent<ParticleSystem>();
        Light flash = explosionObj.AddComponent<Light>();

        flash.type = LightType.Point;
        flash.color = flashColor;
        flash.intensity = 8f;
        flash.range = explosionSize * 3f;

        var main = ps.main;
        main.maxParticles = explosionParticles;
        main.loop = false;
        main.startLifetime = explosionLifetime;
        main.startSpeed = new ParticleSystem.MinMaxCurve(2f, explosionSize * 2f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.3f);
        main.startColor = new ParticleSystem.MinMaxGradient(coreColor, edgeColor);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.playOnAwake = false;
        main.startRotation3D = true;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[]
        {
            new ParticleSystem.Burst(0f, (short)explosionParticles)
        });

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.2f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(flashColor, 0f),
                new GradientColorKey(coreColor, 0.3f),
                new GradientColorKey(edgeColor, 0.7f),
                new GradientColorKey(Color.clear, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 0.3f),
                new GradientAlphaKey(0.5f, 0.7f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverLifetime.color = grad;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve(
            new Keyframe(0f, 0.3f),
            new Keyframe(0.2f, 1f),
            new Keyframe(1f, 0f)
        );
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        var renderer = explosionObj.GetComponent<ParticleSystemRenderer>();
        if (renderer == null)
            renderer = explosionObj.AddComponent<ParticleSystemRenderer>();

        renderer.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        renderer.material.color = flashColor;
        renderer.material.SetFloat("_Surface", 1);
        renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
        renderer.material.SetInt("_ZWrite", 0);
        renderer.material.renderQueue = 3100;
        renderer.renderMode = ParticleSystemRenderMode.Billboard;

        ps.Play();

        if (createShockwave)
            CreateShockwave(position);

        Destroy(flash.gameObject, explosionLifetime + 0.2f);
        Destroy(explosionObj, explosionLifetime + 0.5f);
    }

    private void CreateShockwave(Vector3 position)
    {
        GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Quad);
        ring.name = "Shockwave";
        ring.transform.position = position;
        ring.transform.localScale = Vector3.zero;

        Object.Destroy(ring.GetComponent<Collider>());

        MeshRenderer mr = ring.GetComponent<MeshRenderer>();
        mr.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        mr.material.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0.6f);
        mr.material.SetFloat("_Surface", 1);
        mr.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mr.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mr.material.SetInt("_ZWrite", 0);
        mr.material.renderQueue = 3100;
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        ShockwaveExpansion expansion = ring.AddComponent<ShockwaveExpansion>();
        expansion.Initialize(shockwaveSize, shockwaveSpeed, flashColor);
    }
}

public class ShockwaveExpansion : MonoBehaviour
{
    private float targetSize;
    private float speed;
    private Color color;
    private float timer;
    private float duration = 0.4f;
    private MeshRenderer mr;

    public void Initialize(float target, float spd, Color col)
    {
        targetSize = target;
        speed = spd;
        color = col;
        mr = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float progress = timer / duration;

        float scale = Mathf.Lerp(0f, targetSize, progress * speed * 0.1f);
        transform.localScale = new Vector3(scale, scale, 0.01f);

        if (mr != null && mr.material != null)
        {
            float alpha = Mathf.Lerp(0.6f, 0f, progress);
            Color c = color;
            c.a = alpha;
            mr.material.color = c;
        }

        if (progress >= 1f)
            Destroy(gameObject);
    }
}
