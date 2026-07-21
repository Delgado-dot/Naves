using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadarController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private RectTransform radarArea;
    [SerializeField] private Image playerArrowImage;

    [Header("Settings")]
    [SerializeField] private float worldRange = 150f;
    [SerializeField] private float blipSize = 10f;
    [SerializeField] private Color blipColor = new Color(1f, 0.2f, 0.2f, 1f);
    [SerializeField, Min(0.05f)] private float refreshInterval = 0.25f;

    private Sprite dotSprite;
    private readonly Dictionary<Transform, RectTransform> blips = new Dictionary<Transform, RectTransform>();
    private readonly List<Transform> staleKeys = new List<Transform>();
    private readonly HashSet<Transform> targets = new HashSet<Transform>();
    private float nextRefreshTime;

    private void Awake()
    {
        dotSprite = HUDTextureUtility.CreateCircleSprite(32, Color.white);

        if (playerArrowImage != null)
            playerArrowImage.sprite = HUDTextureUtility.CreateTriangleSprite(32, new Color(0.4f, 0.85f, 1f, 1f));

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    private void Update()
    {
        if (player == null || radarArea == null)
            return;

        if (Time.unscaledTime >= nextRefreshTime)
        {
            RefreshTargets();
            nextRefreshTime = Time.unscaledTime + refreshInterval;
        }

        float radius = Mathf.Min(radarArea.rect.width, radarArea.rect.height) * 0.46f;
        foreach (Transform enemyTransform in targets)
        {
            if (enemyTransform == null)
                continue;

            Vector3 offset = enemyTransform.position - player.position;

            float localX = Vector3.Dot(offset, player.right);
            float localZ = Vector3.Dot(offset, player.forward);
            Vector2 flatOffset = new Vector2(localX, localZ);

            float distance = Mathf.Min(flatOffset.magnitude, worldRange);
            Vector2 direction = flatOffset.sqrMagnitude > 0.0001f ? flatOffset.normalized : Vector2.zero;
            Vector2 anchoredPosition = direction * (distance / worldRange) * radius;

            if (!blips.TryGetValue(enemyTransform, out RectTransform blip))
            {
                blip = CreateBlip();
                blips[enemyTransform] = blip;
            }

            blip.anchoredPosition = anchoredPosition;
            float blipPulse = 0.85f + Mathf.Sin(Time.unscaledTime * 6f + enemyTransform.GetInstanceID()) * 0.2f;
            blip.localScale = Vector3.one * blipPulse;
        }

        staleKeys.Clear();
        foreach (KeyValuePair<Transform, RectTransform> pair in blips)
        {
            if (pair.Key == null || !targets.Contains(pair.Key))
                staleKeys.Add(pair.Key);
        }

        foreach (Transform key in staleKeys)
        {
            if (blips[key] != null)
                Destroy(blips[key].gameObject);
            blips.Remove(key);
        }
    }

    public void RegistrarObjetivo(Transform objetivo)
    {
        if (objetivo != null && objetivo != player)
            targets.Add(objetivo);
    }

    public void QuitarObjetivo(Transform objetivo)
    {
        if (objetivo != null)
            targets.Remove(objetivo);
    }

    private void RefreshTargets()
    {
        targets.Clear();

        SpaceEnemy[] enemies = FindObjectsByType<SpaceEnemy>(FindObjectsSortMode.None);
        foreach (SpaceEnemy enemy in enemies)
            RegistrarObjetivo(enemy.transform);
    }

    private RectTransform CreateBlip()
    {
        GameObject blipObj = new GameObject("Blip", typeof(RectTransform), typeof(Image));
        blipObj.transform.SetParent(radarArea, false);

        Image image = blipObj.GetComponent<Image>();
        image.sprite = dotSprite;
        image.color = blipColor;
        image.raycastTarget = false;

        RectTransform rect = blipObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = Vector2.one * blipSize;

        return rect;
    }
}
