using UnityEngine;

namespace RechargeCustomSkins
{
    // Detects the magenta-guide-line grid every skin sheet template is drawn
    // on: a 2px gutter around and between cells, guide lines in GridLineColor.
    internal static class GridSheet
    {
        public const int Gutter = 2;

        private static readonly Color32 GridLineColor = new Color32(255, 0, 220, 255);
        private const int GridLineSearchRadius = 3;
        private const int GridLineColorTolerance = 60;

        // Tolerant of resize/recompress blur - not an exact pixel match.
        public static bool LooksLikeGrid(Texture2D tex, int cols, int rows, out int cellW, out int cellH)
        {
            cellW = (tex.width - Gutter) / cols - Gutter;
            cellH = (tex.height - Gutter) / rows - Gutter;
            if (cellW <= 0 || cellH <= 0) return false;

            var pixels = tex.GetPixels32();
            int w = tex.width, h = tex.height;
            if (!HasGridLineNear(pixels, w, h, 0, 0)) return false;
            if (!HasGridLineNear(pixels, w, h, 0, h - 1)) return false;
            if (cols > 1)
            {
                int xDivider = Gutter + cellW;
                if (xDivider >= w || !HasGridLineNear(pixels, w, h, xDivider, 0)) return false;
            }
            return true;
        }

        private static bool HasGridLineNear(Color32[] pixels, int w, int h, int x, int y)
        {
            for (int dy = -GridLineSearchRadius; dy <= GridLineSearchRadius; dy++)
            {
                int sy = y + dy;
                if (sy < 0 || sy >= h) continue;
                for (int dx = -GridLineSearchRadius; dx <= GridLineSearchRadius; dx++)
                {
                    int sx = x + dx;
                    if (sx < 0 || sx >= w) continue;
                    if (IsGridLineColor(pixels[sy * w + sx])) return true;
                }
            }
            return false;
        }

        public static bool IsGridLineColor(Color32 c) =>
            System.Math.Abs(c.r - GridLineColor.r) + System.Math.Abs(c.g - GridLineColor.g) + System.Math.Abs(c.b - GridLineColor.b) <= GridLineColorTolerance
            && c.a > 200;
    }
}
