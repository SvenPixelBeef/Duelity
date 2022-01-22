using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class SpriteExtensions
{
    public delegate bool PixelValidator(Texture2D tex, int x, int y);

    public enum SpritePixelPositionMode
    {
        All,
        Edge,
        RightEdge,
        LeftEdge,
        BottomEdge,
        TopEdge,
    }

    public static IEnumerable<Vector2> GetAllPixelPositions(this Sprite sprite, PixelValidator pixelValidator = null)
    {
        Rect rect = sprite.rect;
        Texture2D tex = sprite.texture;

        int x = (int)rect.position.x;
        int y = (int)rect.position.y;

        float pixelSize = 1f / sprite.pixelsPerUnit;

        for (int i = x; i < x + 32; i++)
        {
            for (int j = y; j < y + 32; j++)
            {
                var color = tex.GetPixel(i, j);
                if (color.a == 0)
                    continue;

                if (!pixelValidator?.Invoke(tex, i, j) ?? false)
                    continue;

                // Offset is based on pivot (must be bottom center) and half pixel size to center properly
                Vector3 offset = new Vector3((16 * pixelSize) + (-pixelSize * .5f), -pixelSize * .5f, 0);
                float posX = (i - x) * pixelSize;
                float posY = (j - y) * pixelSize;
                var pos = new Vector3(posX, posY) - offset;
                yield return pos;
            }
        }
    }

    public static IEnumerable<Vector2> GetEdgePixelPositions(this Sprite sprite)
    {
        return GetAllPixelPositions(sprite, IsEdgePixel);
    }

    public static IEnumerable<Vector2> GetRightEdgePixelPositions(this Sprite sprite)
    {
        IEnumerable<Vector2> rightEdgePixelPositions = GetAllPixelPositions(sprite, IsRightEdgePixel);
        IEnumerable<Vector2> ignore = rightEdgePixelPositions.GroupBy(v => v.y)
            .SelectMany(g => g.OrderByDescending(g => g.x).Skip(1));

        return rightEdgePixelPositions.Except(ignore);
    }

    public static IEnumerable<Vector2> GetLeftEdgePixelPositions(this Sprite sprite)
    {
        IEnumerable<Vector2> leftEdgePixelPositions = GetAllPixelPositions(sprite, IsLeftEdgePixel);
        IEnumerable<Vector2> ignore = leftEdgePixelPositions.GroupBy(v => v.y)
            .SelectMany(g => g.OrderBy(g => g.x).Skip(1));

        return leftEdgePixelPositions.Except(ignore);
    }

    public static IEnumerable<Vector2> GetBottomEdgePixelPositions(this Sprite sprite, int maxAllowedYDiffInPixelUnits = 3)
    {
        IEnumerable<Vector2> bottomEdgePixelPositions = GetAllPixelPositions(sprite, IsBottomEdgePixel);
        IEnumerable<Vector2> ignore = bottomEdgePixelPositions.GroupBy(v => v.x)
            .SelectMany(g => g.OrderBy(g => g.y).Skip(1));

        IEnumerable<Vector2> withoutIgnored = bottomEdgePixelPositions.Except(ignore);
        float lowestY = withoutIgnored.OrderBy(v => v.y).First().y;
        float allowedYDiff = (1f / sprite.pixelsPerUnit) * maxAllowedYDiffInPixelUnits;
        return withoutIgnored.Where(v => Mathf.Abs(v.y - lowestY) <= allowedYDiff);
    }

    public static IEnumerable<Vector2> GetTopEdgePixelPositions(this Sprite sprite, int maxAllowedYDiffInPixelUnits = 3)
    {
        IEnumerable<Vector2> topEdgePixelPositions = GetAllPixelPositions(sprite, IsTopEdgePixel);
        IEnumerable<Vector2> ignore = topEdgePixelPositions.GroupBy(v => v.x)
            .SelectMany(g => g.OrderByDescending(g => g.y).Skip(1));

        IEnumerable<Vector2> withoutIgnored = topEdgePixelPositions.Except(ignore);
        float highestY = withoutIgnored.OrderByDescending(v => v.y).First().y;
        float allowedYDiff = (1f / sprite.pixelsPerUnit) * maxAllowedYDiffInPixelUnits;
        return withoutIgnored.Where(v => Mathf.Abs(v.y - highestY) <= allowedYDiff);
    }


    static bool IsEdgePixel(Texture2D tex, int x, int y)
    {
        if (IsLeftEdgePixel(tex, x, y))
            return true;

        if (IsRightEdgePixel(tex, x, y))
            return true;

        if (IsBottomEdgePixel(tex, x, y))
            return true;

        if (IsTopEdgePixel(tex, x, y))
            return true;

        return false;
    }

    static bool IsRightEdgePixel(Texture2D tex, int x, int y)
    {
        int neighbor = x + 1;
        return neighbor > tex.width - 1
            || tex.GetPixel(neighbor, y).a == 0;
    }

    static bool IsLeftEdgePixel(Texture2D tex, int x, int y)
    {
        int neighbor = x - 1;
        return neighbor < 0
            || tex.GetPixel(neighbor, y).a == 0;
    }

    static bool IsTopEdgePixel(Texture2D tex, int x, int y)
    {
        int neighbor = y + 1;
        return neighbor > tex.height - 1
            || tex.GetPixel(x, neighbor).a == 0;
    }

    static bool IsBottomEdgePixel(Texture2D tex, int x, int y)
    {
        int neighbor = y - 1;
        return neighbor < 0
            || tex.GetPixel(x, neighbor).a == 0;
    }
}
