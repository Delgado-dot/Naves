using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

[InitializeOnLoad]
public static class SetupPlayerShip
{
    private const string PrefabPath = "Assets/Prefabs/PlayerShip.prefab";
    private const string InputActionsGuid = "052faaac586de48259a63d0c4782560b";

    static SetupPlayerShip()
    {
        EditorApplication.delayCall += OnDomainLoad;
    }

    private static void OnDomainLoad()
    {
        CreateProjectilePrefab();
        CreatePlayerShipPrefab();
    }

    private static void CreateProjectilePrefab()
    {
        const string path = "Assets/Prefabs/Projectile.prefab";

        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null)
            return;

        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            AssetDatabase.CreateFolder("Assets", "Prefabs");

        var projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectile.name = "Projectile";
        projectile.transform.localScale = Vector3.one * 0.15f;
        var collider = projectile.GetComponent<Collider>();
        collider.isTrigger = true;
        var rb = projectile.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        projectile.AddComponent<Projectile>();

        PrefabUtility.SaveAsPrefabAsset(projectile, path);
        Object.DestroyImmediate(projectile);
    }

    private static void CreatePlayerShipPrefab()
    {
        if (AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath) != null)
            return;

        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            AssetDatabase.CreateFolder("Assets", "Prefabs");

        var inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(
            AssetDatabase.GUIDToAssetPath(InputActionsGuid));

        var playerShip = new GameObject("PlayerShip");
        var controller = playerShip.AddComponent<PlayerShipController>();

        if (inputActions != null)
        {
            SerializedObject so = new SerializedObject(controller);
            SerializedProperty prop = so.FindProperty("inputActions");
            if (prop != null)
            {
                prop.objectReferenceValue = inputActions;
                so.ApplyModifiedProperties();
            }
        }

        playerShip.AddComponent<PlayerShooter>();

        var cameraPivot = new GameObject("CameraPivot");
        cameraPivot.transform.SetParent(playerShip.transform);
        cameraPivot.transform.localPosition = Vector3.zero;
        cameraPivot.transform.localRotation = Quaternion.identity;

        var mainCamera = new GameObject("MainCamera");
        mainCamera.transform.SetParent(cameraPivot.transform);
        mainCamera.transform.localPosition = new Vector3(0f, 2f, -5f);
        mainCamera.transform.localRotation = Quaternion.identity;
        mainCamera.tag = "MainCamera";
        mainCamera.AddComponent<Camera>();
        mainCamera.AddComponent<AudioListener>();
        mainCamera.AddComponent<UniversalAdditionalCameraData>();

        var cameraComponent = mainCamera.AddComponent<ThirdPersonCamera>();
        SerializedObject camSo = new SerializedObject(cameraComponent);
        SerializedProperty targetProp = camSo.FindProperty("target");
        if (targetProp != null)
        {
            targetProp.objectReferenceValue = playerShip.transform;
            camSo.ApplyModifiedProperties();
        }

        var firePointLeft = new GameObject("FirePointLeft");
        firePointLeft.transform.SetParent(playerShip.transform);
        firePointLeft.transform.localPosition = new Vector3(-1.5f, 0f, 0f);
        firePointLeft.transform.localRotation = Quaternion.identity;

        var firePointRight = new GameObject("FirePointRight");
        firePointRight.transform.SetParent(playerShip.transform);
        firePointRight.transform.localPosition = new Vector3(1.5f, 0f, 0f);
        firePointRight.transform.localRotation = Quaternion.identity;

        var shooter = playerShip.GetComponent<PlayerShooter>();
        ConfigureShooter(shooter, inputActions, firePointLeft.transform, firePointRight.transform);

        PrefabUtility.SaveAsPrefabAsset(playerShip, PrefabPath);

        Object.DestroyImmediate(playerShip);
    }

    private static void ConfigureShooter(PlayerShooter shooter, InputActionAsset inputActions, Transform left, Transform right)
    {
        SerializedObject so = new SerializedObject(shooter);
        so.FindProperty("firePointLeft").objectReferenceValue = left;
        so.FindProperty("firePointRight").objectReferenceValue = right;
        so.FindProperty("projectilePrefab").objectReferenceValue =
            AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Projectile.prefab");
        if (inputActions != null)
            so.FindProperty("inputActions").objectReferenceValue = inputActions;
        so.ApplyModifiedProperties();
    }
}
