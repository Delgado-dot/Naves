using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public sealed class HUDClockGraphic : MaskableGraphic
{
    [SerializeField] private Color fondo = new(0.005f, 0.025f, 0.07f, 0.9f);
    [SerializeField] private Color linea = new(0.85f, 0.97f, 1f, 1f);

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        Rect r = GetPixelAdjustedRect();
        Vector2 center = r.center;
        float radius = Mathf.Min(r.width, r.height) * 0.44f;
        AddCircle(vh, center, radius, 32, fondo, true);
        AddRing(vh, center, radius, 4f, 32, linea);
        AddLine(vh, center, center + new Vector2(0f, radius * 0.58f), 4f, linea);
        AddLine(vh, center, center + new Vector2(radius * 0.48f, -radius * 0.2f), 4f, linea);
        AddCircle(vh, center, 4f, 12, linea, true);
    }

    private static void AddCircle(VertexHelper vh, Vector2 center, float radius, int segments, Color color, bool fill)
    {
        int start = vh.currentVertCount;
        vh.AddVert(center, color, Vector2.zero);
        for (int i = 0; i <= segments; i++)
        {
            float a = i * Mathf.PI * 2f / segments;
            vh.AddVert(center + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius, color, Vector2.zero);
            if (fill && i > 0) vh.AddTriangle(start, start + i, start + i + 1);
        }
    }

    private static void AddRing(VertexHelper vh, Vector2 center, float radius, float width, int segments, Color color)
    {
        for (int i = 0; i < segments; i++)
        {
            float a = i * Mathf.PI * 2f / segments;
            float b = (i + 1) * Mathf.PI * 2f / segments;
            AddLine(vh, center + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius,
                center + new Vector2(Mathf.Cos(b), Mathf.Sin(b)) * radius, width, color);
        }
    }

    private static void AddLine(VertexHelper vh, Vector2 a, Vector2 b, float width, Color color)
    {
        Vector2 n = new Vector2(-(b.y - a.y), b.x - a.x).normalized * width * 0.5f;
        int s = vh.currentVertCount;
        vh.AddVert(a - n, color, Vector2.zero); vh.AddVert(a + n, color, Vector2.zero);
        vh.AddVert(b + n, color, Vector2.zero); vh.AddVert(b - n, color, Vector2.zero);
        vh.AddTriangle(s, s + 1, s + 2); vh.AddTriangle(s, s + 2, s + 3);
    }
}
