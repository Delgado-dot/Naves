using UnityEngine;

/// <summary>Projectile that moves forward and detects hits on Enemy-tagged objects.</summary>
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 40f;
    [SerializeField] private float lifetime = 5f;

    private Light projectileLight;
    private ParticleSystem trailPS;

    private void Start()
    {
        Destroy(gameObject, lifetime);
        CreateVisualEffects();
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void CreateVisualEffects()
    {
        projectileLight = gameObject.AddComponent<Light>();
        projectileLight.type = LightType.Point;
        projectileLight.color = new Color(0.3f, 0.7f, 1f, 1f);
        projectileLight.intensity = 2f;
        projectileLight.range = 4f;

        GameObject trailObj = new GameObject("Trail");
        trailObj.transform.SetParent(transform);
        trailObj.transform.localPosition = Vector3.zero;

        trailPS = trailObj.AddComponent<ParticleSystem>();

        var main = trailPS.main;
        main.maxParticles = 30;
        main.loop = true;
        main.startLifetime = 0.4f;
        main.startSpeed = 0;
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.12f);
        main.startColor = new Color(0.3f, 0.7f, 1f, 0.8f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.playOnAwake = true;

        var emission = trailPS.emission;
        emission.rateOverTime = 25;

        var shape = trailPS.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.05f;

        var colorOverLifetime = trailPS.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(0.3f, 0.7f, 1f), 0f),
                new GradientColorKey(new Color(0.1f, 0.3f, 0.8f), 0.5f),
                new GradientColorKey(Color.clear, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0.8f, 0f),
                new GradientAlphaKey(0.3f, 0.5f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverLifetime.color = grad;

        var sizeOverLifetime = trailPS.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f,
            AnimationCurve.Linear(0f, 1f, 1f, 0f));

        var renderer = trailObj.GetComponent<ParticleSystemRenderer>();
        if (renderer == null)
            renderer = trailObj.AddComponent<ParticleSystemRenderer>();

        renderer.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        renderer.material.color = new Color(0.3f, 0.7f, 1f, 0.8f);
        renderer.material.SetFloat("_Surface", 1);
        renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
        renderer.material.SetInt("_ZWrite", 0);
        renderer.material.renderQueue = 3100;
        renderer.renderMode = ParticleSystemRenderMode.Billboard;

        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null)
        {
            mr.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            mr.material.color = new Color(0.4f, 0.8f, 1f, 1f);
            mr.material.SetFloat("_Surface", 1);
            mr.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mr.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mr.material.SetInt("_ZWrite", 0);
            mr.material.renderQueue = 3100;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        SpawnImpactEffect();

        if (!other.CompareTag("Enemy"))
            return;

        HandleEnemyHit(other.gameObject);

        Destroy(gameObject);
    }

    private void SpawnImpactEffect()
    {
        GameObject impact = new GameObject("ProjectileImpact");
        impact.transform.position = transform.position;
        impact.AddComponent<ProjectileImpact>();
    }

    protected virtual void HandleEnemyHit(GameObject enemy)
    {
    }
}