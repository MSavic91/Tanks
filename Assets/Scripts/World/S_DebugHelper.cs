using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class S_DebugHelper : MonoBehaviour {
    /// <summary>
    /// Draw a sphere provided the radius and an origin.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="radius"></param>
    public static void DrawSphere(Vector3 position, float radius)
    {
#if UNITY_EDITOR
        for (int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 20; j++)
            {
                Vector3 end = Vector3.forward * radius;
                Vector3 start = Vector3.forward * radius;
                if (i == 0)
                {
                    start = Quaternion.Euler(360f / 20f * (j - 1), 0f, 0f) * start;
                    end = Quaternion.Euler(360f / 20f * j, 0f, 0f) * end;
                }
                else if (i == 1)
                {
                    start = Quaternion.Euler(0f, 360f / 20f * (j - 1), 0f) * start;
                    end = Quaternion.Euler(0f, 360f / 20f * j, 0f) * end;
                }
                else if (i == 2)
                {
                    start = Quaternion.Euler(360f / 20f * (j - 1), 90f, 0f) * start;
                    end = Quaternion.Euler(360f / 20f * j, 90f, 0f) * end;
                }
                Debug.DrawLine(position + start, position + end, Color.green);
            }
        }
#endif
    }
    /// <summary>
    /// Draw a sphere provided the radius and an origin.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="radius"></param>
    public static void DrawHandlesSphere(Vector3 position, float radius, Color color)
    {
#if UNITY_EDITOR
        Color oldColor = Handles.color;
        Handles.color = color;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                Vector3 end = Vector3.forward * radius;
                Vector3 start = Vector3.forward * radius;
                if (i == 0)
                {
                    start = Quaternion.Euler(360f / 20f * (j - 1), 0f, 0f) * start;
                    end = Quaternion.Euler(360f / 20f * j, 0f, 0f) * end;
                }
                else if (i == 1)
                {
                    start = Quaternion.Euler(0f, 360f / 20f * (j - 1), 0f) * start;
                    end = Quaternion.Euler(0f, 360f / 20f * j, 0f) * end;
                }
                else if (i == 2)
                {
                    start = Quaternion.Euler(0f, 90f, 360f / 20f * (j - 1)) * start;
                    end = Quaternion.Euler(0f, 90f, 360f / 20f * j) * end;
                }
                Handles.DrawLine(position + start, position + end);
            }
        }
        Handles.color = oldColor;
#endif
    }

    public static void DrawHandlesCircle(Vector3 position, float radius, Color color)
    {
#if UNITY_EDITOR
        Color oldColor = Handles.color;
        Handles.color = color;
        for (int i = 0; i < 20; i++)
        {
            Vector3 end = Vector3.forward * radius;
            Vector3 start = Vector3.forward * radius;
                
            start = Quaternion.Euler(0f, 360f / 20f * (i - 1), 0f) * start;
            end = Quaternion.Euler(0f, 360f / 20f * i, 0f) * end;
                
            Handles.DrawLine(position + start, position + end);
        }
        Handles.color = oldColor;
#endif
    }

    /// <summary>
    /// Draw a cylinder from A to B with radius.
    /// </summary>
    /// <param name="cylinderBase"></param>
    /// <param name="cylinderCap"></param>
    /// <param name="radius"></param>
    public static void DrawCylinder(Vector3 A, Vector3 B, float radius)
    {
#if UNITY_EDITOR
        Vector3 dir = (B - A).normalized;
        for(int i = 0; i < 8; i++)
        {
            Vector3 A0 = A + Quaternion.AngleAxis(360f / 8f * (i - 1), dir.normalized) * Vector3.up * radius;
            Vector3 A1 = A + Quaternion.AngleAxis(360f / 8f * i, dir.normalized) * Vector3.up * radius;
            Vector3 B0 = B + Quaternion.AngleAxis(360f / 8f * (i - 1), dir.normalized) * Vector3.up * radius;
            Vector3 B1 = B + Quaternion.AngleAxis(360f / 8f * i, dir.normalized) * Vector3.up * radius;

            Debug.DrawLine(A0, A1, Color.green, 0.5f);
            Debug.DrawLine(B0, B1, Color.green, 0.5f);
            Debug.DrawLine(A0, B0, Color.green, 0.5f);
        }
#endif
    }

    /// <summary>
    /// Draw a sine arc between A and B.
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    public static void DrawArc(Vector3 A, Vector3 B)
    {
#if UNITY_EDITOR
        Color oldColor = Handles.color;
        Handles.color = Color.yellow;
        float dist = (A - B).magnitude;
        for (int i = 0; i < 99; i++)
        {
            float height1 = dist * Mathf.Sin(i / 100f * Mathf.PI) / 3f;
            float height2 = dist * Mathf.Sin((i + 1) / 100f * Mathf.PI) / 3f;

            Handles.DrawLine(Vector3.Lerp(A, B, i / 100f) + Vector3.up * height1, Vector3.Lerp(A, B, (i + 1) / 100f) + Vector3.up * height2);
        }
        Handles.color = oldColor;
#endif
    }

    /// <summary>
    /// Draw a sine arc between A and B.
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    public static void DrawViewCone(Vector3 origin, Vector3 direction, float width, float range)
    {
#if UNITY_EDITOR

        Color oldColor = Handles.color;
        Handles.color = Color.green;
        for (int i = 0; i < 11; i++)
        {
            Vector3 A = origin;
            Vector3 B = direction * range;
            B = Quaternion.AngleAxis(-width / 2f + (width * ((i + 0.5f) / 11f)), Vector3.up) * B;
            B = A + B;
            Handles.DrawLine(A, B);
        }

        for(int i = 0; i < 40f; i++)
        {
            Vector3 A = origin + Quaternion.AngleAxis(360f / 40f * i, Vector3.up) * Vector3.forward * range;
            Vector3 B = origin + Quaternion.AngleAxis(360f / 40f * (i + 1), Vector3.up) * Vector3.forward * range;
            Handles.DrawLine(A, B);
        }
        Handles.color = oldColor;
#endif
    }

    /// <summary>
    /// Draw an arrow at origin, pointing in the direction.
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    public static void DrawArrow(Vector3 origin, Vector3 direction)
    {
#if UNITY_EDITOR
        Vector3 A = origin;
        Vector3 B = origin + direction.normalized * 2f;
        Handles.DrawLine(A, B);
        A = B + Quaternion.AngleAxis(-45f, Vector3.up) * -direction.normalized;
        Handles.DrawLine(A, B);
        A = B + Quaternion.AngleAxis(45f, Vector3.up) * -direction.normalized;
        Handles.DrawLine(A, B);
#endif
    }

    /// <summary>
    /// Draw a connection between origin and target.
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="target"></param>
    public static void DrawConnection(Vector3 origin, Vector3 target)
    {
#if UNITY_EDITOR
        Vector3 A = origin;
        Vector3 B = target;
        Vector3 AB = B - A;
        Handles.DrawLine(A, B);
        Vector3 A1 = A + Quaternion.AngleAxis(90f, Vector3.up) * AB.normalized;
        Vector3 A2 = A - Quaternion.AngleAxis(90f, Vector3.up) * AB.normalized;
        Handles.DrawLine(A1, A2);
        A = B + Quaternion.AngleAxis(-45f, Vector3.up) * -AB.normalized;
        Handles.DrawLine(A, B);
        A = B + Quaternion.AngleAxis(45f, Vector3.up) * -AB.normalized;
        Handles.DrawLine(A, B);
#endif
    }
}
