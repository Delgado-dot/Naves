using UnityEngine;

public static class HUDTextureUtility
{
    public static Sprite CreateCircleSprite(int diameter, Color color)
    {
        Texture2D texture = new Texture2D(diameter, diameter, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        float radius = diameter * 0.5f;
        Vector2 center = new Vector2(radius, radius);

        for (int y = 0; y < diameter; y++)
        {
            for (int x = 0; x < diameter; x++)
            {
                float dist = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
                float alpha = Mathf.Clamp01(radius - dist);
                texture.SetPixel(x, y, new Color(color.r, color.g, color.b, color.a * alpha));
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, diameter, diameter), new Vector2(0.5f, 0.5f));
    }

    public static Sprite CreateRingSprite(int diameter, float thickness, Color color)
    {
        Texture2D texture = new Texture2D(diameter, diameter, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        float radius = diameter * 0.5f;
        Vector2 center = new Vector2(radius, radius);

        for (int y = 0; y < diameter; y++)
        {
            for (int x = 0; x < diameter; x++)
            {
                float dist = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
                float edgeDist = radius - dist;
                float alpha;

                if (edgeDist >= 0f && edgeDist <= thickness)
                    alpha = 1f;
                else if (edgeDist > thickness)
                    alpha = Mathf.Clamp01(1f - (edgeDist - thickness));
                else
                    alpha = Mathf.Clamp01(1f + edgeDist);

                texture.SetPixel(x, y, new Color(color.r, color.g, color.b, color.a * alpha));
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, diameter, diameter), new Vector2(0.5f, 0.5f));
    }

    public static Sprite CreateTriangleSprite(int size, Color color)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        Vector2 top = new Vector2(size * 0.5f, size * 0.95f);
        Vector2 bottomLeft = new Vector2(size * 0.15f, size * 0.05f);
        Vector2 bottomRight = new Vector2(size * 0.85f, size * 0.05f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 p = new Vector2(x + 0.5f, y + 0.5f);
                bool inside = PointInTriangle(p, top, bottomLeft, bottomRight);
                texture.SetPixel(x, y, inside ? color : new Color(0f, 0f, 0f, 0f));
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    public static Sprite CreateCutCornerSprite(int size, int cornerCut, Color color)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        int max = size - 1;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                bool excluded =
                    (x + y < cornerCut) ||
                    ((max - x) + y < cornerCut) ||
                    (x + (max - y) < cornerCut) ||
                    ((max - x) + (max - y) < cornerCut);

                texture.SetPixel(x, y, excluded ? new Color(0f, 0f, 0f, 0f) : color);
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0,
            SpriteMeshType.FullRect, new Vector4(cornerCut, cornerCut, cornerCut, cornerCut));
    }

    public static Sprite CreateRadarFaceSprite(int size, Color fillColor, Color lineColor, float lineThickness)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        float radius = size * 0.5f;
        Vector2 center = new Vector2(radius, radius);
        float half = lineThickness * 0.5f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 p = new Vector2(x + 0.5f, y + 0.5f);
                float dist = Vector2.Distance(p, center);
                float circleAlpha = Mathf.Clamp01(radius - dist);

                if (circleAlpha <= 0f)
                {
                    texture.SetPixel(x, y, new Color(0f, 0f, 0f, 0f));
                    continue;
                }

                bool onCrosshair = Mathf.Abs(p.x - center.x) <= half || Mathf.Abs(p.y - center.y) <= half;
                Color pixelColor = onCrosshair ? lineColor : fillColor;
                pixelColor.a *= circleAlpha;
                texture.SetPixel(x, y, pixelColor);
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    public static Sprite CreateClockIconSprite(int size, Color color)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        float radius = size * 0.5f;
        Vector2 center = new Vector2(radius, radius);
        float ringThickness = size * 0.09f;
        float handThickness = size * 0.07f;

        Vector2 hourHandEnd = center + new Vector2(0f, radius * 0.35f);
        Vector2 minuteHandEnd = center + new Vector2(radius * 0.4f, radius * 0.15f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 p = new Vector2(x + 0.5f, y + 0.5f);
                float dist = Vector2.Distance(p, center);
                float edgeDist = radius - dist;

                bool onRing = edgeDist >= 0f && edgeDist <= ringThickness;
                bool onHour = NearSegment(p, center, hourHandEnd, handThickness);
                bool onMinute = NearSegment(p, center, minuteHandEnd, handThickness);

                texture.SetPixel(x, y, (onRing || onHour || onMinute) ? color : new Color(0f, 0f, 0f, 0f));
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    public static Sprite CreateShieldIconSprite(int size, Color borderColor, Color fillColor)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        Vector2[] outer = ShieldPoints(size, 1f);
        Vector2[] inner = ShieldPoints(size, 0.78f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 p = new Vector2(x + 0.5f, y + 0.5f);

                if (PointInPolygon(p, inner))
                    texture.SetPixel(x, y, fillColor);
                else if (PointInPolygon(p, outer))
                    texture.SetPixel(x, y, borderColor);
                else
                    texture.SetPixel(x, y, new Color(0f, 0f, 0f, 0f));
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    private static Vector2[] ShieldPoints(int size, float scale)
    {
        Vector2 center = new Vector2(size * 0.5f, size * 0.48f);
        Vector2[] normalized =
        {
            new Vector2(0.18f, 1.00f),
            new Vector2(0.82f, 1.00f),
            new Vector2(0.82f, 0.55f),
            new Vector2(0.50f, 0.02f),
            new Vector2(0.18f, 0.55f),
        };

        Vector2[] points = new Vector2[normalized.Length];
        for (int i = 0; i < normalized.Length; i++)
        {
            Vector2 world = new Vector2(normalized[i].x * size, normalized[i].y * size);
            points[i] = center + (world - center) * scale;
        }

        return points;
    }

    private static bool PointInPolygon(Vector2 point, Vector2[] polygon)
    {
        bool inside = false;
        int j = polygon.Length - 1;

        for (int i = 0; i < polygon.Length; i++)
        {
            Vector2 pi = polygon[i];
            Vector2 pj = polygon[j];

            if ((pi.y > point.y) != (pj.y > point.y) &&
                point.x < (pj.x - pi.x) * (point.y - pi.y) / (pj.y - pi.y) + pi.x)
            {
                inside = !inside;
            }

            j = i;
        }

        return inside;
    }

    private static bool NearSegment(Vector2 p, Vector2 a, Vector2 b, float thickness)
    {
        Vector2 ab = b - a;
        float sqrLen = ab.sqrMagnitude;
        float t = sqrLen > 0.0001f ? Mathf.Clamp01(Vector2.Dot(p - a, ab) / sqrLen) : 0f;
        Vector2 proj = a + t * ab;
        return Vector2.Distance(p, proj) <= thickness * 0.5f;
    }

    private static bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        float d1 = Sign(p, a, b);
        float d2 = Sign(p, b, c);
        float d3 = Sign(p, c, a);

        bool hasNeg = d1 < 0f || d2 < 0f || d3 < 0f;
        bool hasPos = d1 > 0f || d2 > 0f || d3 > 0f;

        return !(hasNeg && hasPos);
    }

    private static float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    }
}
