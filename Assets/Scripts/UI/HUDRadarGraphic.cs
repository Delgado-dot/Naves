using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CanvasRenderer))]
public sealed class HUDRadarGraphic : MaskableGraphic
{
    [SerializeField, Min(1f)] private float velocidadBarrido = 75f;
    [SerializeField, Min(0.1f)] private float velocidadPulso = 2.2f;

    private void Start()
    {
        CrearRosaDeNavegacion();
    }

    private void Update()
    {
        SetVerticesDirty();
    }

    private void CrearRosaDeNavegacion()
    {
        if (transform.Find("RosaNavegacion") != null)
            return;

        GameObject root = new("RosaNavegacion", typeof(RectTransform));
        root.layer = gameObject.layer;
        root.transform.SetParent(transform, false);
        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        CrearEtiqueta(root.transform, "Norte", "N", new Vector2(0f, 0.405f), 30f, Color.white);
        CrearEtiqueta(root.transform, "Este", "E", new Vector2(0.405f, 0f), 30f, Color.white);
        CrearEtiqueta(root.transform, "Sur", "S", new Vector2(0f, -0.405f), 30f, Color.white);
        CrearEtiqueta(root.transform, "Oeste", "O", new Vector2(-0.405f, 0f), 30f, Color.white);

        string[] grados = { "045", "135", "225", "315" };
        float[] angulos = { 45f, -45f, -135f, 135f };
        for (int i = 0; i < grados.Length; i++)
        {
            float radianes = angulos[i] * Mathf.Deg2Rad;
            Vector2 posicion = new Vector2(Mathf.Cos(radianes), Mathf.Sin(radianes)) * 0.34f;
            CrearEtiqueta(root.transform, "Grado_" + grados[i], grados[i], posicion, 16f,
                new Color(0.38f, 0.88f, 1f, 0.9f));
        }
    }

    private static void CrearEtiqueta(Transform parent, string nombre, string contenido, Vector2 posicionNormalizada,
        float tamano, Color color)
    {
        GameObject labelObject = new(nombre, typeof(RectTransform), typeof(TextMeshProUGUI));
        labelObject.layer = parent.gameObject.layer;
        labelObject.transform.SetParent(parent, false);

        RectTransform rect = labelObject.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f) + posicionNormalizada;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(48f, 26f);

        TextMeshProUGUI label = labelObject.GetComponent<TextMeshProUGUI>();
        label.text = contenido;
        label.fontSize = tamano;
        label.fontStyle = FontStyles.Bold;
        label.alignment = TextAlignmentOptions.Center;
        label.color = color;
        label.outlineColor = new Color32(0, 22, 48, 255);
        label.outlineWidth = 0.22f;
        label.raycastTarget = false;
        label.enableWordWrapping = false;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        Rect r = GetPixelAdjustedRect();
        Vector2 c = r.center;
        float radius = Mathf.Min(r.width, r.height) * 0.46f;
        Color fill = new(0.003f, 0.025f, 0.065f, 0.93f);
        Color cyan = new(0.02f, 0.72f, 1f, 0.9f);
        float pulse = 0.5f + 0.5f * Mathf.Sin(Time.time * velocidadPulso * Mathf.PI * 2f);
        AddRing(vh, c, radius + 8f, 6f + pulse * 3f, 96, new Color(cyan.r, cyan.g, cyan.b, .08f + pulse * .12f));
        AddDisk(vh, c, radius, 96, fill);
        AddRing(vh, c, radius, 4f, 96, cyan);
        AddRing(vh, c, radius - 7f, 1.3f, 96, new Color(cyan.r, cyan.g, cyan.b, .68f));
        AddRing(vh, c, radius * .72f, 1.1f, 96, new Color(cyan.r, cyan.g, cyan.b, .3f));
        AddRing(vh, c, radius * .4f, 1f, 96, new Color(cyan.r, cyan.g, cyan.b, .24f));
        AddLine(vh, c + Vector2.left * radius, c + Vector2.right * radius, 1f, new Color(cyan.r, cyan.g, cyan.b, .38f));
        AddLine(vh, c + Vector2.down * radius, c + Vector2.up * radius, 1f, new Color(cyan.r, cyan.g, cyan.b, .38f));
        for (int i = 0; i < 8; i++)
        {
            float a = i * Mathf.PI / 4f;
            Vector2 d = new(Mathf.Cos(a), Mathf.Sin(a));
            AddLine(vh, c, c + d * radius, .8f, new Color(cyan.r, cyan.g, cyan.b, .1f));
            AddLine(vh, c + d * radius * .88f, c + d * radius, 2.3f, cyan);
            AddLine(vh, c + d * (radius + 5f), c + d * (radius + 13f), 3.5f, cyan);
        }

        float sweepAngle = Time.time * velocidadBarrido * Mathf.Deg2Rad;
        AddWedge(vh, c, radius * .93f, sweepAngle - .58f, sweepAngle,
            new Color(.03f, .65f, 1f, .14f));
        for (int trail = 4; trail >= 0; trail--)
        {
            float angle = sweepAngle - trail * 0.09f;
            Vector2 direction = new(Mathf.Cos(angle), Mathf.Sin(angle));
            float alpha = Mathf.Lerp(.04f, .86f, 1f - trail / 4f);
            float width = Mathf.Lerp(.8f, 2.8f, 1f - trail / 4f);
            AddLine(vh, c, c + direction * radius * .94f, width, new Color(.1f, .85f, 1f, alpha));
        }

        AddRing(vh, c, 7f + pulse * 2f, 1.5f, 24, new Color(.25f, .9f, 1f, .45f + pulse * .25f));
        AddShipMarker(vh, c, new Color(.55f, .94f, 1f, 1f));
    }

    private static void AddWedge(VertexHelper vh, Vector2 c, float radius, float from, float to, Color color)
    {
        const int segments = 18;
        int start = vh.currentVertCount;
        vh.AddVert(c, new Color(color.r, color.g, color.b, 0f), Vector2.zero);
        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            float angle = Mathf.Lerp(from, to, t);
            Color edge = new(color.r, color.g, color.b, color.a * t);
            vh.AddVert(c + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius, edge, Vector2.zero);
            if (i > 0) vh.AddTriangle(start, start + i, start + i + 1);
        }
    }

    private static void AddShipMarker(VertexHelper vh, Vector2 c, Color color)
    {
        int start = vh.currentVertCount;
        Vector2[] points =
        {
            c + new Vector2(0f, 13f), c + new Vector2(-5f, 2f),
            c + new Vector2(-11f, -5f), c + new Vector2(-4f, -3f),
            c + new Vector2(0f, -9f), c + new Vector2(4f, -3f),
            c + new Vector2(11f, -5f), c + new Vector2(5f, 2f)
        };
        foreach (Vector2 point in points) vh.AddVert(point, color, Vector2.zero);
        vh.AddTriangle(start, start + 1, start + 7);
        vh.AddTriangle(start + 1, start + 3, start + 4);
        vh.AddTriangle(start + 1, start + 4, start + 7);
        vh.AddTriangle(start + 4, start + 5, start + 7);
        vh.AddTriangle(start + 1, start + 2, start + 3);
        vh.AddTriangle(start + 5, start + 6, start + 7);
    }

    private static void AddDisk(VertexHelper vh, Vector2 c, float radius, int n, Color color)
    {
        int s = vh.currentVertCount; vh.AddVert(c, color, Vector2.zero);
        for (int i = 0; i <= n; i++) { float a = i * Mathf.PI * 2f / n; vh.AddVert(c + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius, color, Vector2.zero); if (i > 0) vh.AddTriangle(s, s + i, s + i + 1); }
    }
    private static void AddRing(VertexHelper vh, Vector2 c, float radius, float width, int n, Color color)
    { for (int i = 0; i < n; i++) { float a = i * Mathf.PI * 2f / n, b = (i + 1) * Mathf.PI * 2f / n; AddLine(vh, c + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius, c + new Vector2(Mathf.Cos(b), Mathf.Sin(b)) * radius, width, color); } }
    private static void AddLine(VertexHelper vh, Vector2 a, Vector2 b, float width, Color color)
    { Vector2 n = new Vector2(-(b.y-a.y), b.x-a.x).normalized * width*.5f; int s=vh.currentVertCount; vh.AddVert(a-n,color,Vector2.zero); vh.AddVert(a+n,color,Vector2.zero); vh.AddVert(b+n,color,Vector2.zero); vh.AddVert(b-n,color,Vector2.zero); vh.AddTriangle(s,s+1,s+2); vh.AddTriangle(s,s+2,s+3); }
}
