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
        EditorApplication.delayCall += CreatePlayerShipPrefab;
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

        var firePointLeft = new GameObject("FirePointLeft");
        firePointLeft.transform.SetParent(playerShip.transform);
        firePointLeft.transform.localPosition = new Vector3(-1.5f, 0f, 0f);
        firePointLeft.transform.localRotation = Quaternion.identity;

        var firePointRight = new GameObject("FirePointRight");
        firePointRight.transform.SetParent(playerShip.transform);
        firePointRight.transform.localPosition = new Vector3(1.5f, 0f, 0f);
        firePointRight.transform.localRotation = Quaternion.identity;

        PrefabUtility.SaveAsPrefabAsset(playerShip, PrefabPath);

        Object.DestroyImmediate(playerShip);
    }
}
