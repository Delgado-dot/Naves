using UnityEngine;

public class SpaceDust : MonoBehaviour
{
    [Header("Particle System")]
    [SerializeField] private int maxParticles = 500;
    [SerializeField] private float spawnRadius = 50f;
    [SerializeField] private float particleSize = 0.08f;

    [Header("Movement")]
    [SerializeField] private float driftSpeed = 0.5f;

    [Header("Appearance")]
    [SerializeField] private Color dustColor = new Color(0.6f, 0.65f, 0.8f, 0.4f);

    private ParticleSystem ps;
    private ParticleSystemRenderer psRenderer;
    private Transform cameraTransform;

    private void Start()
    {
        if (cameraTransform == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
                cameraTransform = mainCam.transform;
        }

        SetupParticleSystem();
        EmitParticles();
    }

    private void Update()
    {
        if (ps == null)
            return;

        transform.position = cameraTransform != null ? cameraTransform.position : Vector3.zero;
    }

    private void SetupParticleSystem()
    {
        ps = GetComponent<ParticleSystem>();
        if (ps == null)
            ps = gameObject.AddComponent<ParticleSystem>();

        psRenderer = GetComponent<ParticleSystemRenderer>();
        if (psRenderer == null)
            psRenderer = gameObject.AddComponent<ParticleSystemRenderer>();

        var main = ps.main;
        main.maxParticles = maxParticles;
        main.loop = false;
        main.startLifetime = Mathf.Infinity;
        main.startSpeed = 0;
        main.startSize = particleSize;
        main.startColor = dustColor;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.playOnAwake = false;
        main.startRotation3D = false;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[]
        {
            new ParticleSystem.Burst(0f, (short)maxParticles)
        });

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = spawnRadius;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(Color.white, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(0.5f, 0.1f),
                new GradientAlphaKey(0.5f, 0.9f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverLifetime.color = gradient;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f,
            AnimationCurve.Linear(0f, 0.3f, 1f, 1f));

        psRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        psRenderer.material.color = dustColor;
        psRenderer.material.SetFloat("_Surface", 1);
        psRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        psRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
        psRenderer.material.SetInt("_ZWrite", 0);
        psRenderer.material.renderQueue = 3100;
        psRenderer.renderMode = ParticleSystemRenderMode.Billboard;
        psRenderer.maxParticleSize = particleSize;
    }

    private void EmitParticles()
    {
        ParticleSystem.MainModule main = ps.main;
        main.maxParticles = maxParticles;

        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();

        for (int i = 0; i < maxParticles; i++)
        {
            Vector3 pos = Random.insideUnitSphere * spawnRadius;
            emitParams.position = pos;
            emitParams.startLifetime = Mathf.Infinity;
            emitParams.startSize = Random.Range(particleSize * 0.5f, particleSize * 1.5f);
            emitParams.startColor = dustColor;
            ps.Emit(emitParams, 1);
        }
    }

    private void OnDestroy()
    {
        if (psRenderer != null && psRenderer.material != null)
            Destroy(psRenderer.material);
    }
}
