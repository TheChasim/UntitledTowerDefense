using UnityEngine;
using System.Collections.Generic;

public static class SpriteColorUtils
{
    public static Color GetDominantColorFast(Sprite sprite, int samples = 8, int precision = 16, float alphaThreshold = 0.2f)
    {
        if (sprite == null || sprite.texture == null)
            return Color.magenta;

        Texture2D tex = sprite.texture;
        if (!tex.isReadable)
            return Color.magenta; // Fallback si Read/Write n'est pas activé

        Rect rect = sprite.textureRect;
        Dictionary<int, int> colors = new Dictionary<int, int>();

        for (int y = 0; y < samples; y++)
        {
            for (int x = 0; x < samples; x++)
            {
                float u = (x + 0.5f) / samples;
                float v = (y + 0.5f) / samples;

                int px = Mathf.FloorToInt(rect.x + u * rect.width);
                int py = Mathf.FloorToInt(rect.y + v * rect.height);

                Color p = tex.GetPixel(px, py);

                if (p.a < alphaThreshold)
                    continue;

                // Quantification pour regrouper les couleurs similaires
                int qr = Mathf.RoundToInt(p.r * precision);
                int qg = Mathf.RoundToInt(p.g * precision);
                int qb = Mathf.RoundToInt(p.b * precision);

                int key = (qr << 16) | (qg << 8) | qb;

                if (colors.TryGetValue(key, out int count))
                    colors[key] = count + 1;
                else
                    colors[key] = 1;
            }
        }

        if (colors.Count == 0)
            return Color.white;

        // Trouver la couleur la plus fréquente
        int bestKey = 0;
        int bestCount = -1;

        foreach (var kv in colors)
        {
            if (kv.Value > bestCount)
            {
                bestCount = kv.Value;
                bestKey = kv.Key;
            }
        }

        // Déquantification -> retour en float
        float inv = 1f / precision;

        float r = ((bestKey >> 16) & 255) * inv;
        float g = ((bestKey >> 8) & 255) * inv;
        float b = (bestKey & 255) * inv;

        return new Color(r, g, b, 1f);
    }

}
