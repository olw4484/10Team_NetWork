using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SelectionUtility
{
    public static bool IsInSelectionBox(Vector2 start, Vector2 end, Vector3 screenPos)
    {
        float xMin = Mathf.Min(start.x, end.x);
        float xMax = Mathf.Max(start.x, end.x);
        float yMin = Mathf.Min(start.y, end.y);
        float yMax = Mathf.Max(start.y, end.y);

        return screenPos.x >= xMin && screenPos.x <= xMax &&
               screenPos.y >= yMin && screenPos.y <= yMax;
    }
}
