using UnityEngine;

public class ShootingStar : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private float spawnRadius = 150f;
    [SerializeField] private float minInterval = 3f;
    [SerializeField] private float maxInterval = 12f;

    [Header("Appearance")]
    [SerializeField] private float trailLength = 15f;
    [SerializeField] private float starSize = 0.15f;
    [SerializeField] private float speed = 80f;
    [SerializeField] private float starLifetime = 2f;

    [Header("Colors")]
    [SerializeField] private Color headColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color tailColor = new Color(0.4f, 0.6f, 1f, 0.3f);

    private float nextSpawnTime;
    private Transform cameraTransform;

    private void Start()
    {
        Camera mainCam = Camera.main;
        if (mainCam != null)
            cameraTransform = mainCam.transform;

        nextSpawnTime = Time.time + Random.Range(minInterval, maxInterval);
    }

    private void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnShootingStar();
            nextSpawnTime = Time.time + Random.Range(minInterval, maxInterval);
        }
    }

    private void SpawnShootingStar()
    {
        Vector3 origin = GetRandomEdgePoint();
        Vector3 direction = (Vector3.zero - origin).normalized + Random.insideUnitSphere * 0.3f;
        direction.Normalize();

        GameObject starObj = new GameObject("ShootingStar");
        starObj.transform.position = origin;
        starObj.transform.rotation = Quaternion.LookRotation(direction);

        Light starLight = starObj.AddComponent<Light>();
        starLight.type = LightType.Point;
        starLight.color = headColor;
        starLight.intensity = 2f;
        starLight.range = 8f;

        ParticleSystem ps = starObj.AddComponent<ParticleSystem>();
        ConfigureShootingStarParticles(ps, direction);

        ParticleSystemRenderer renderer = starObj.GetComponent<ParticleSystemRenderer>();
        if (renderer == null)
            renderer = starObj.AddComponent<ParticleSystemRenderer>();

        renderer.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        renderer.material.color = headColor;
        renderer.material.SetFloat("_Surface", 1);
        renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
        renderer.material.SetInt("_ZWrite", 0);
        renderer.material.renderQueue = 3100;
        renderer.renderMode = ParticleSystemRenderMode.Stretch;
        renderer.velocityScale = 0.05f;
        renderer.lengthScale = 2f;

        ps.Play();

        Object.Destroy(starObj, starLifetime);
    }

    private void ConfigureShootingStarParticles(ParticleSystem ps, Vector3 dir)
    {
        var main = ps.main;
        main.maxParticles = 50;
        main.loop = false;
        main.startLifetime = starLifetime;
        main.startSpeed = speed;
        main.startSize = new ParticleSystem.MinMaxCurve(starSize * 0.5f, starSize);
        main.startColor = new ParticleSystem.MinMaxGradient(headColor, tailColor);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.playOnAwake = true;
        main.startRotation3D = false;

        var emission = ps.emission;
        emission.rateOverTime = 30;

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 2f;
        shape.radius = 0.01f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(headColor, 0f),
                new GradientColorKey(tailColor, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverLifetime.color = grad;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f,
            AnimationCurve.Linear(0f, 1f, 1f, 0.1f));
    }

    private Vector3 GetRandomEdgePoint()
    {
        int side = Random.Range(0, 6);
        float offset = Random.Range(-30f, 30f);

        return side switch
        {
            0 => new Vector3(spawnRadius, offset, offset),
            1 => new Vector3(-spawnRadius, offset, offset),
            2 => new Vector3(offset, spawnRadius, offset),
            3 => new Vector3(offset, -spawnRadius, offset),
            4 => new Vector3(offset, offset, spawnRadius),
            _ => new Vector3(offset, offset, -spawnRadius),
        };
    }
}
