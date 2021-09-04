using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static void SetParentAndSnap(this Transform transform, Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public static Transform Clear(this Transform transform)
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        return transform;
    }

    public static Transform ClearEditor(this Transform transform)
    {
        foreach (Transform child in transform)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }
        return transform;
    }

    public static void SetLayerRecursively(this GameObject root, int newLayer, string ignoreObjectsWithName = "")
    {
        if (ignoreObjectsWithName != string.Empty)
        {
            if (root.name.Contains(ignoreObjectsWithName))
            {
                return;
            }
        }
        root.layer = newLayer;

        foreach (Transform child in root.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer, ignoreObjectsWithName);
        }
    }

    public static string FormatStringValue(this float val)
    {
        int valInt = Mathf.RoundToInt(val * 100f);
        if (valInt >= 500)
        {
            return (valInt / 100).ToString();
        }
        else if (valInt >= 100)
        {
            return (valInt / 100).ToString() + "." + ((valInt % 100) / 10).ToString();
        }
        else if (valInt >= 10)
        {
            return "0." + (valInt / 10).ToString();
        }
        else
        {
            return "0";
        }
    }

    public static string FormatDecimalValue(this float val)
    {
        int valInt = Mathf.RoundToInt(val * 100f);
        if (valInt >= 10000)
        {
            return (valInt / 100).ToString();
        }
        else if (valInt >= 100)
        {
            return (valInt / 100).ToString() + "." + ((valInt % 100) / 10).ToString();
        }
        else if (valInt >= 10)
        {
            return "0." + (valInt / 10).ToString();
        }
        else
        {
            return "0";
        }
    }

    public static string FormatStringPercentage(this float val)
    {
        int valInt = Mathf.RoundToInt(val * 10000f);
        if (valInt >= 500)
        {
            return (valInt / 100).ToString() + "%";
        }
        else if (valInt >= 100)
        {
            return (valInt / 100).ToString() + "." + ((valInt % 100) / 10).ToString() + "%";
        }
        else if (valInt >= 10)
        {
            return "0." + (valInt / 10).ToString() + "%";
        }
        else
        {
            return "0%";
        }
    }

    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }

    public static Transform FindFirstWhichNameContains(this Transform t, string substring)
    {
        if (t.name.Contains(substring))
        {
            return t;
        }

        foreach (Transform tr in t)
        {
            var res = FindFirstWhichNameContains(tr, substring);
            if (res != null)
            {
                return res;
            }
        }

        return null;
    }

    public static bool LineIntersection(Vector2 p1start, Vector2 p1dir, Vector2 p2start, Vector2 p2dir, out Vector2 intersection)
    {
        Vector2 l1s = p1start;
        Vector2 l1e = p1start + p1dir;
        Vector2 l2s = p2start;
        Vector2 l2e = p2start + p2dir;

        float A1 = l1e.y - l1s.y;
        float B1 = l1s.x - l1e.x;
        float C1 = A1 * l1s.x + B1 * l1s.y;
        float A2 = l2e.y - l2s.y;
        float B2 = l2s.x - l2e.x;
        float C2 = A2 * l2s.x + B2 * l2s.y;

        float delta = A1 * B2 - A2 * B1;

        intersection = Vector2.zero;

        if (delta == 0f)
        {
            return false;
        }

        float x = (B2 * C1 - B1 * C2) / delta;
        float y = (A1 * C2 - A2 * C1) / delta;
        intersection = new Vector2(x, y);

        return true;
    }

    public static Vector3[] ToPositions(this Transform[] transforms)
    {
        if (transforms == null)
        {
            return null;
        }
        Vector3[] result = new Vector3[transforms.Length];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = transforms[i].position;
        }
        return result;
    }

    public static float ToAsymptoticRange(this float value, float min, float max)
    {
        return min + (1 - 1 / (value / (max - min) * 2 + 1)) * (max - min);
    }

    public static float ToAsymptoticRangePowered(this float value, float min, float max, float power)
    {
        return min + Mathf.Pow(1 - 1 / (value * 3 / (max - min) * 8 + 1), power) * (max - min);
    }

    public static int ToPowerLevel(this float bladePower)
    {
        return 1 + Mathf.FloorToInt(bladePower - 51.3f) / 5;
    }

    public static string ToFormattedTime(this int span, bool limitSize = false)
    {
        if (span < 60)
        {
            return span.ToString();
        }
        else if (span < 3600)
        {
            int minutes = span / 60;
            int seconds = span % 60;
            return string.Format("{0}:{1:00}", minutes, seconds);
        }
        else
        {
            if (limitSize)
            {
                return string.Format("{0}:{1:00}", span / 3600, (span % 3600) / 60);
            }
            else
            {
                return string.Format("{0}:{1:00}:{2:00}", span / 3600, (span % 3600) / 60, span % 60);
            }
        }
    }
}
