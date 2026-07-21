using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public sealed class HUDTechPanelGraphic : MaskableGraphic
{
    [SerializeField] private Color fondo = new(0.005f, 0.025f, 0.07f, 0.9f);
    [SerializeField] private Color cian = new(0.02f, 0.72f, 1f, 1f);
    [SerializeField] private Color brillo = new(0.12f, 0.86f, 1f, 0.24f);
    [SerializeField] private Color violeta = new(0.62f, 0.22f, 1f, 0.95f);
    [SerializeField, Range(8f, 40f)] private float corte = 22f;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        Rect r = GetPixelAdjustedRect();
        Vector2[] shape = Shape(r, corte);
        AddPolygon(vh, shape, fondo);

        AddLoop(vh, shape, 3f, cian);
        AddLoop(vh, Shape(Inset(r, 8f), Mathf.Max(6f, corte - 4f)), 1.4f, new Color(cian.r, cian.g, cian.b, 0.7f));
        AddLoop(vh, Shape(Expand(r, 5f), corte + 4f), 5f, brillo);

        float yTop = r.yMax - 5f;
        float yBottom = r.yMin + 5f;
        AddLine(vh, new Vector2(r.xMin + corte + 18f, yTop), new Vector2(r.center.x - 25f, yTop), 3f, cian);
        AddLine(vh, new Vector2(r.center.x + 28f, yTop), new Vector2(r.xMax - corte - 18f, yTop), 3f, cian);
        AddLine(vh, new Vector2(r.xMin + corte + 30f, yBottom), new Vector2(r.center.x - 18f, yBottom), 2f, violeta);
        AddLine(vh, new Vector2(r.center.x + 18f, yBottom), new Vector2(r.xMax - corte - 30f, yBottom), 3f, cian);

        AddLine(vh, new Vector2(r.xMin + 8f, r.center.y - 12f), new Vector2(r.xMin + 8f, r.center.y + 12f), 3f, violeta);
        AddLine(vh, new Vector2(r.xMax - 8f, r.center.y - 12f), new Vector2(r.xMax - 8f, r.center.y + 12f), 3f, cian);
    }

    private static Rect Inset(Rect r, float amount) => new(r.xMin + amount, r.yMin + amount, r.width - amount * 2f, r.height - amount * 2f);
    private static Rect Expand(Rect r, float amount) => new(r.xMin - amount, r.yMin - amount, r.width + amount * 2f, r.height + amount * 2f);

    private static Vector2[] Shape(Rect r, float c) => new[]
    {
        new Vector2(r.xMin + c, r.yMin), new Vector2(r.xMax - c, r.yMin),
        new Vector2(r.xMax, r.yMin + c), new Vector2(r.xMax, r.yMax - c),
        new Vector2(r.xMax - c, r.yMax), new Vector2(r.xMin + c, r.yMax),
        new Vector2(r.xMin, r.yMax - c), new Vector2(r.xMin, r.yMin + c)
    };

    private static void AddPolygon(VertexHelper vh, Vector2[] points, Color color)
    {
        int start = vh.currentVertCount;
        foreach (Vector2 point in points) vh.AddVert(point, color, Vector2.zero);
        for (int i = 1; i < points.Length - 1; i++) vh.AddTriangle(start, start + i, start + i + 1);
    }

    private static void AddLoop(VertexHelper vh, Vector2[] points, float width, Color color)
    {
        for (int i = 0; i < points.Length; i++) AddLine(vh, points[i], points[(i + 1) % points.Length], width, color);
    }

    private static void AddLine(VertexHelper vh, Vector2 a, Vector2 b, float width, Color color)
    {
        Vector2 normal = new Vector2(-(b.y - a.y), b.x - a.x).normalized * width * 0.5f;
        int start = vh.currentVertCount;
        vh.AddVert(a - normal, color, Vector2.zero);
        vh.AddVert(a + normal, color, Vector2.zero);
        vh.AddVert(b + normal, color, Vector2.zero);
        vh.AddVert(b - normal, color, Vector2.zero);
        vh.AddTriangle(start, start + 1, start + 2);
        vh.AddTriangle(start, start + 2, start + 3);
    }
}
