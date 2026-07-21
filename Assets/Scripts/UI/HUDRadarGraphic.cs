using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public sealed class HUDRadarGraphic : MaskableGraphic
{
    [SerializeField, Min(1f)] private float velocidadBarrido = 75f;
    [SerializeField, Min(0.1f)] private float velocidadPulso = 2.2f;

    private void Update()
    {
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        Rect r = GetPixelAdjustedRect();
        Vector2 c = r.center;
        float radius = Mathf.Min(r.width, r.height) * 0.46f;
        Color fill = new(0.005f, 0.035f, 0.09f, 0.88f);
        Color cyan = new(0.02f, 0.72f, 1f, 0.9f);
        float pulse = 0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * velocidadPulso * Mathf.PI * 2f);
        AddRing(vh, c, radius + 9f, 7f + pulse * 4f, 64, new Color(cyan.r, cyan.g, cyan.b, .1f + pulse * .16f));
        AddDisk(vh, c, radius, 64, fill);
        AddRing(vh, c, radius, 4f, 64, cyan);
        AddRing(vh, c, radius - 7f, 1.5f, 64, new Color(cyan.r, cyan.g, cyan.b, .75f));
        AddRing(vh, c, radius * .72f, 1.3f, 64, new Color(cyan.r, cyan.g, cyan.b, .45f));
        AddRing(vh, c, radius * .4f, 1.2f, 64, new Color(cyan.r, cyan.g, cyan.b, .35f));
        AddLine(vh, c + Vector2.left * radius, c + Vector2.right * radius, 1.2f, new Color(cyan.r, cyan.g, cyan.b, .55f));
        AddLine(vh, c + Vector2.down * radius, c + Vector2.up * radius, 1.2f, new Color(cyan.r, cyan.g, cyan.b, .55f));
        for (int i = 0; i < 8; i++)
        {
            float a = i * Mathf.PI / 4f;
            Vector2 d = new(Mathf.Cos(a), Mathf.Sin(a));
            AddLine(vh, c, c + d * radius, 1f, new Color(cyan.r, cyan.g, cyan.b, .16f));
            AddLine(vh, c + d * radius * .86f, c + d * radius, 3f, cyan);
            AddLine(vh, c + d * (radius + 5f), c + d * (radius + 15f), 5f, cyan);
        }

        float sweepAngle = Time.unscaledTime * velocidadBarrido * Mathf.Deg2Rad;
        for (int trail = 5; trail >= 0; trail--)
        {
            float angle = sweepAngle - trail * 0.075f;
            Vector2 direction = new(Mathf.Cos(angle), Mathf.Sin(angle));
            float alpha = Mathf.Lerp(.06f, .9f, 1f - trail / 5f);
            float width = Mathf.Lerp(1f, 3.5f, 1f - trail / 5f);
            AddLine(vh, c, c + direction * radius * .94f, width, new Color(.1f, .85f, 1f, alpha));
        }

        AddRing(vh, c, 4f + pulse * 2f, 2f, 16, new Color(.25f, .9f, 1f, .7f + pulse * .3f));
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
