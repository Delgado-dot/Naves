using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[InitializeOnLoad]
public static class SpaceEnvironmentSetup
{
    private const string EnvironmentParent = "SpaceEnvironment";

    static SpaceEnvironmentSetup()
    {
        EditorApplication.delayCall += OnDomainLoad;
    }

    private static void OnDomainLoad()
    {
        if (GameObject.Find(EnvironmentParent) != null)
            return;

        CreateEnvironment();
    }

    [MenuItem("Space/Setup Space Environment")]
    public static void CreateEnvironment()
    {
        if (GameObject.Find(EnvironmentParent) != null)
        {
            Object.DestroyImmediate(GameObject.Find(EnvironmentParent));
        }

        GameObject envParent = new GameObject(EnvironmentParent);

        CreateDynamicBackground(envParent.transform);
        CreateSpaceDust(envParent.transform);
        CreateAsteroidBelt(envParent.transform);
        CreateShootingStars(envParent.transform);
        CreatePulseStars(envParent.transform);
        SetupSpaceLighting();
        SetupSpaceBackground();

        Debug.Log("Space environment created successfully!");
    }

    [MenuItem("Space/Remove Space Environment")]
    public static void RemoveEnvironment()
    {
        GameObject env = GameObject.Find(EnvironmentParent);
        if (env != null)
        {
            Object.DestroyImmediate(env);
            Debug.Log("Space environment removed.");
        }
        else
        {
            Debug.Log("No space environment found.");
        }
    }

    private static void CreateStarField(Transform parent)
    {
        GameObject starFieldObj = new GameObject("StarField");
        starFieldObj.transform.SetParent(parent);
        starFieldObj.transform.localPosition = Vector3.zero;

        StarField starField = starFieldObj.AddComponent<StarField>();

        SerializedObject so = new SerializedObject(starField);
        SetSerializedFloat(so, "starCount", 3000);
        SetSerializedFloat(so, "fieldRadius", 250f);
        SetSerializedFloat(so, "parallaxFactor", 0.1f);
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void CreateNebula(Transform parent)
    {
        GameObject nebulaObj = new GameObject("NebulaEffect");
        nebulaObj.transform.SetParent(parent);
        nebulaObj.transform.localPosition = Vector3.zero;

        NebulaEffect nebula = nebulaObj.AddComponent<NebulaEffect>();

        SerializedObject so = new SerializedObject(nebula);
        SetSerializedFloat(so, "layerCount", 4);
        SetSerializedFloat(so, "layerRadius", 200f);
        SetSerializedFloat(so, "layerSize", 100f);
        SetSerializedFloat(so, "rotationSpeed", 0.3f);
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void CreateSpaceDust(Transform parent)
    {
        GameObject dustObj = new GameObject("SpaceDust");
        dustObj.transform.SetParent(parent);
        dustObj.transform.localPosition = Vector3.zero;

        SpaceDust dust = dustObj.AddComponent<SpaceDust>();

        SerializedObject so = new SerializedObject(dust);
        SetSerializedFloat(so, "maxParticles", 800);
        SetSerializedFloat(so, "spawnRadius", 60f);
        SetSerializedFloat(so, "particleSize", 0.06f);
        SetSerializedFloat(so, "driftSpeed", 0.3f);
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void CreateAsteroidBelt(Transform parent)
    {
        GameObject beltObj = new GameObject("AsteroidBelt");
        beltObj.transform.SetParent(parent);
        beltObj.transform.localPosition = Vector3.zero;

        AsteroidBelt belt = beltObj.AddComponent<AsteroidBelt>();

        SerializedObject so = new SerializedObject(belt);
        SetSerializedFloat(so, "maxAsteroids", 40);
        SetSerializedFloat(so, "spawnDistance", 100f);
        SetSerializedFloat(so, "despawnDistance", 12f);
        SetSerializedFloat(so, "minSize", 0.5f);
        SetSerializedFloat(so, "maxSize", 5f);
        SetSerializedFloat(so, "approachSpeed", 1.5f);
        SetSerializedFloat(so, "approachSpeedVariation", 1f);
        SetSerializedFloat(so, "wobbleStrength", 0.3f);
        SetSerializedFloat(so, "wobbleSpeed", 0.5f);
        SetSerializedFloat(so, "rotationSpeed", 8f);
        SetSerializedFloat(so, "spawnInterval", 0.8f);
        SetSerializedFloat(so, "spawnBatch", 2);
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void CreateShootingStars(Transform parent)
    {
        GameObject shootingObj = new GameObject("ShootingStars");
        shootingObj.transform.SetParent(parent);
        shootingObj.transform.localPosition = Vector3.zero;

        ShootingStar ss = shootingObj.AddComponent<ShootingStar>();

        SerializedObject so = new SerializedObject(ss);
        SetSerializedFloat(so, "spawnRadius", 150f);
        SetSerializedFloat(so, "minInterval", 4f);
        SetSerializedFloat(so, "maxInterval", 15f);
        SetSerializedFloat(so, "trailLength", 15f);
        SetSerializedFloat(so, "speed", 80f);
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void CreatePulseStars(Transform parent)
    {
        GameObject pulseObj = new GameObject("PulseStars");
        pulseObj.transform.SetParent(parent);
        pulseObj.transform.localPosition = Vector3.zero;

        StarPulse sp = pulseObj.AddComponent<StarPulse>();

        SerializedObject so = new SerializedObject(sp);
        SetSerializedFloat(so, "pulseStarCount", 25);
        SetSerializedFloat(so, "spawnRadius", 120f);
        SetSerializedFloat(so, "starSize", 0.5f);
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void CreateDynamicBackground(Transform parent)
    {
        GameObject bgObj = new GameObject("DynamicSpaceBackground");
        bgObj.transform.SetParent(parent);
        bgObj.transform.localPosition = Vector3.zero;

        DynamicSpaceBackground bg = bgObj.AddComponent<DynamicSpaceBackground>();

        Sprite starsSmall = AssetDatabase.LoadAssetAtPath<Sprite>(
            "Assets/DinV/Dynamic Space Background/Sprites/Stars Small_1.png");
        Sprite starsBig = AssetDatabase.LoadAssetAtPath<Sprite>(
            "Assets/DinV/Dynamic Space Background/Sprites/Stars-Big_1.png");
        Sprite nebulaRed = AssetDatabase.LoadAssetAtPath<Sprite>(
            "Assets/DinV/Dynamic Space Background/Sprites/Nebula Red.png");
        Sprite nebulaBlue = AssetDatabase.LoadAssetAtPath<Sprite>(
            "Assets/DinV/Dynamic Space Background/Sprites/Nebula Blue.png");
        Sprite nebulaAquaPink = AssetDatabase.LoadAssetAtPath<Sprite>(
            "Assets/DinV/Dynamic Space Background/Sprites/Nebula Aqua-Pink.png");

        SerializedObject so = new SerializedObject(bg);
        so.FindProperty("starsSmall").objectReferenceValue = starsSmall;
        so.FindProperty("starsBig").objectReferenceValue = starsBig;
        so.FindProperty("nebulaRed").objectReferenceValue = nebulaRed;
        so.FindProperty("nebulaBlue").objectReferenceValue = nebulaBlue;
        so.FindProperty("nebulaAquaPink").objectReferenceValue = nebulaAquaPink;
        so.ApplyModifiedPropertiesWithoutUndo();

        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            SerializedObject camSo = new SerializedObject(bg);
            SerializedProperty camProp = camSo.FindProperty("cameraTransform");
            if (camProp != null)
            {
                camProp.objectReferenceValue = mainCam.transform;
                camSo.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }

    private static void SetupSpaceLighting()
    {
        Light[] lights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
        foreach (Light light in lights)
        {
            if (light.type == LightType.Directional)
            {
                SerializedObject so = new SerializedObject(light);
                SerializedProperty colorProp = so.FindProperty("m_Color");
                if (colorProp != null)
                {
                    colorProp.colorValue = new Color(0.85f, 0.9f, 1f, 1f);
                }

                SerializedProperty intensityProp = so.FindProperty("m_Intensity");
                if (intensityProp != null)
                {
                    intensityProp.floatValue = 1.2f;
                }

                SerializedProperty tempProp = so.FindProperty("m_ColorTemperature");
                if (tempProp != null)
                {
                    tempProp.floatValue = 7500f;
                }

                SerializedProperty useTempProp = so.FindProperty("m_UseColorTemperature");
                if (useTempProp != null)
                {
                    useTempProp.boolValue = true;
                }

                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }

    private static void SetupSpaceBackground()
    {
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.05f, 0.05f, 0.1f, 1f);
        RenderSettings.ambientIntensity = 0.5f;

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogColor = new Color(0.02f, 0.02f, 0.05f, 1f);
        RenderSettings.fogDensity = 0.003f;

        RenderSettings.reflectionIntensity = 0.3f;

        Camera[] cameras = Object.FindObjectsByType<Camera>(FindObjectsSortMode.None);
        foreach (Camera cam in cameras)
        {
            if (cam.CompareTag("MainCamera"))
            {
                
            }
        }
    }

    private static void SetSerializedFloat(SerializedObject so, string name, float value)
    {
        SerializedProperty prop = so.FindProperty(name);
        if (prop != null)
            prop.floatValue = value;
    }
}
