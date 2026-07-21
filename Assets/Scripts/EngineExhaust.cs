using UnityEngine;

public class EngineExhaust : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform leftExhaust;
    [SerializeField] private Transform rightExhaust;

    [Header("Exhaust Particles")]
    [SerializeField] private Color exhaustCore = new Color(0.3f, 0.6f, 1f, 1f);
    [SerializeField] private Color exhaustEdge = new Color(0.1f, 0.2f, 0.8f, 0.6f);
    [SerializeField] private float exhaustSpeed = 8f;
    [SerializeField] private float exhaustLifetime = 0.6f;
    [SerializeField] private float exhaustSize = 0.3f;

    [Header("Engine Glow")]
    [SerializeField] private Color glowColor = new Color(0.2f, 0.5f, 1f, 1f);
    [SerializeField] private float glowIntensity = 3f;
    [SerializeField] private float pulseSpeed = 4f;

    private ParticleSystem leftPS;
    private ParticleSystem rightPS;
    private Light leftGlow;
    private Light rightGlow;
    private PlayerShipController shipController;

    private void Start()
    {
        shipController = GetComponent<PlayerShipController>();

        if (leftExhaust == null) leftExhaust = transform.Find("FirePointLeft");
        if (rightExhaust == null) rightExhaust = transform.Find("FirePointRight");

        CreateExhaustPoint(leftExhaust, out leftPS, out leftGlow);
        CreateExhaustPoint(rightExhaust, out rightPS, out rightGlow);
    }

    private void Update()
    {
        float intensity = shipController != null ? 1f : 0.5f;
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * 0.15f;

        if (leftGlow != null)
        {
            leftGlow.intensity = glowIntensity * intensity * pulse;
            leftGlow.color = glowColor;
        }

        if (rightGlow != null)
        {
            rightGlow.intensity = glowIntensity * intensity * pulse;
            rightGlow.color = glowColor;
        }
    }

    private void CreateExhaustPoint(Transform point, out ParticleSystem ps, out Light glow)
    {
        ps = null;
        glow = null;

        if (point == null)
            return;

        GameObject exhaustObj = new GameObject("Exhaust");
        exhaustObj.transform.SetParent(point);
        exhaustObj.transform.localPosition = Vector3.back * 0.5f;
        exhaustObj.transform.localRotation = Quaternion.identity;

        ps = exhaustObj.AddComponent<ParticleSystem>();
        glow = exhaustObj.AddComponent<Light>();

        glow.type = LightType.Point;
        glow.color = glowColor;
        glow.intensity = glowIntensity;
        glow.range = 3f;

        var main = ps.main;
        main.maxParticles = 100;
        main.loop = true;
        main.startLifetime = exhaustLifetime;
        main.startSpeed = exhaustSpeed;
        main.startSize = new ParticleSystem.MinMaxCurve(exhaustSize * 0.5f, exhaustSize);
        main.startColor = new ParticleSystem.MinMaxGradient(exhaustCore, exhaustEdge);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.playOnAwake = true;
        main.startRotation3D = false;
        main.gravityModifier = 0f;

        var emission = ps.emission;
        emission.rateOverTime = 40;

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 15f;
        shape.radius = 0.1f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(exhaustCore, 0f),
                new GradientColorKey(exhaustEdge, 0.5f),
                new GradientColorKey(Color.clear, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0.8f, 0.3f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverLifetime.color = grad;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f,
            AnimationCurve.Linear(0f, 1f, 1f, 0.3f));

        var renderer = exhaustObj.GetComponent<ParticleSystemRenderer>();
        if (renderer == null)
            renderer = exhaustObj.AddComponent<ParticleSystemRenderer>();

        renderer.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        renderer.material.color = exhaustCore;
        renderer.material.SetFloat("_Surface", 1);
        renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
        renderer.material.SetInt("_ZWrite", 0);
        renderer.material.renderQueue = 3100;
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
    }
}
