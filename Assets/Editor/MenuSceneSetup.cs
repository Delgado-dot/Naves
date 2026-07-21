using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class MenuSceneSetup
{
    private const string MenuScenePath = "Assets/Scenes/MenuPrincipal.unity";

    static MenuSceneSetup()
    {
        EditorApplication.delayCall += CheckAndSetup;
    }

    private static void CheckAndSetup()
    {
        if (!EditorSceneManager.GetActiveScene().name.Contains("MenuPrincipal"))
            return;

        if (GameObject.Find("SpaceEnvironment_Menu") != null)
            return;

        SetupMenuScene();
    }

    [MenuItem("Space/Setup Menu Background")]
    public static void SetupMenuScene()
    {
        GameObject envParent = new GameObject("SpaceEnvironment_Menu");

        CreateMenuStarField(envParent.transform);
        CreateMenuNebula(envParent.transform);
        CreateMenuSpaceDust(envParent.transform);
        CreateMenuPulseStars(envParent.transform);
        SetupMenuLighting();
        SetupMenuCamera();

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

        Debug.Log("Menu space background created!");
    }

    [MenuItem("Space/Remove Menu Background")]
    public static void RemoveMenuScene()
    {
        GameObject env = GameObject.Find("SpaceEnvironment_Menu");
        if (env != null)
        {
            Object.DestroyImmediate(env);
            Debug.Log("Menu space background removed.");
        }
    }

    private static void CreateMenuStarField(Transform parent)
    {
        GameObject starFieldObj = new GameObject("MenuStarField");
        starFieldObj.transform.SetParent(parent);
        starFieldObj.transform.localPosition = Vector3.zero;

        StarField starField = starFieldObj.AddComponent<StarField>();

        SerializedObject so = new SerializedObject(starField);
        SetFloat(so, "starCount", 2500);
        SetFloat(so, "fieldRadius", 200f);
        SetFloat(so, "parallaxFactor", 0.05f);
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void CreateMenuNebula(Transform parent)
    {
        GameObject nebulaObj = new GameObject("MenuNebula");
        nebulaObj.transform.SetParent(parent);
        nebulaObj.transform.localPosition = Vector3.zero;

        DynamicSpaceBackground bg = nebulaObj.AddComponent<DynamicSpaceBackground>();

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
        SetFloat(so, "nebulaRotationSpeed", 0.15f);
        SetFloat(so, "nebulaPulseSpeed", 0.15f);
        so.ApplyModifiedPropertiesWithoutUndo();

        Camera cam = Camera.main;
        if (cam != null)
        {
            SerializedObject camSo = new SerializedObject(bg);
            SerializedProperty camProp = camSo.FindProperty("cameraTransform");
            if (camProp != null)
            {
                camProp.objectReferenceValue = cam.transform;
                camSo.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }

    private static void CreateMenuSpaceDust(Transform parent)
    {
        GameObject dustObj = new GameObject("MenuSpaceDust");
        dustObj.transform.SetParent(parent);
        dustObj.transform.localPosition = Vector3.zero;

        SpaceDust dust = dustObj.AddComponent<SpaceDust>();

        SerializedObject so = new SerializedObject(dust);
        SetFloat(so, "maxParticles", 400);
        SetFloat(so, "spawnRadius", 40f);
        SetFloat(so, "particleSize", 0.05f);
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void CreateMenuPulseStars(Transform parent)
    {
        GameObject pulseObj = new GameObject("MenuPulseStars");
        pulseObj.transform.SetParent(parent);
        pulseObj.transform.localPosition = Vector3.zero;

        StarPulse sp = pulseObj.AddComponent<StarPulse>();

        SerializedObject so = new SerializedObject(sp);
        SetFloat(so, "pulseStarCount", 15);
        SetFloat(so, "spawnRadius", 100f);
        SetFloat(so, "starSize", 0.4f);
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetupMenuLighting()
    {
        Light[] lights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
        foreach (Light light in lights)
        {
            if (light.type == LightType.Directional)
            {
                SerializedObject so = new SerializedObject(light);
                SerializedProperty colorProp = so.FindProperty("m_Color");
                if (colorProp != null)
                    colorProp.colorValue = new Color(0.85f, 0.9f, 1f, 1f);

                SerializedProperty intensityProp = so.FindProperty("m_Intensity");
                if (intensityProp != null)
                    intensityProp.floatValue = 1.2f;

                SerializedProperty tempProp = so.FindProperty("m_ColorTemperature");
                if (tempProp != null)
                    tempProp.floatValue = 7500f;

                SerializedProperty useTempProp = so.FindProperty("m_UseColorTemperature");
                if (useTempProp != null)
                    useTempProp.boolValue = true;

                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.05f, 0.05f, 0.1f, 1f);
        RenderSettings.ambientIntensity = 0.5f;
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogColor = new Color(0.02f, 0.02f, 0.05f, 1f);
        RenderSettings.fogDensity = 0.003f;
        RenderSettings.reflectionIntensity = 0.3f;
    }

    private static void SetupMenuCamera()
    {
        Camera cam = Camera.main;
        if (cam == null)
            return;

        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.01f, 0.01f, 0.03f, 1f);
    }

    private static void SetFloat(SerializedObject so, string name, float value)
    {
        SerializedProperty prop = so.FindProperty(name);
        if (prop != null)
            prop.floatValue = value;
    }
}
