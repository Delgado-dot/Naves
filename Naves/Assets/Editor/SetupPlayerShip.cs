using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[InitializeOnLoad]
public static class SetupPlayerShip
{
    static SetupPlayerShip()
    {
        EditorApplication.delayCall += CreatePlayerShipPrefab;
    }

    private static void CreatePlayerShipPrefab()
    {
        const string prefabPath = "Assets/Prefabs/PlayerShip.prefab";

        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
            return;

        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            AssetDatabase.CreateFolder("Assets", "Prefabs");

        var playerShip = new GameObject("PlayerShip");
        playerShip.AddComponent<PlayerShipController>();
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

        PrefabUtility.SaveAsPrefabAsset(playerShip, prefabPath);

        Object.DestroyImmediate(playerShip);
    }
}
