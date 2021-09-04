
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class ExtendedHotkeys : ScriptableObject
{
    //Snap selected object to the ground
    [MenuItem("Editor Control/Snap To Ground _END")]
    static void SnapToGround()
    {
        foreach(Object o in Selection.objects)
        {
            GameObject go = (GameObject)o;
            if(go != null)
            {
                RaycastHit hit;
                Ray r = new Ray(go.transform.position, Vector3.down);
                if(Physics.Raycast(r, out hit))
                {
                    Undo.RecordObject(go.transform, "Snap to ground");
                    CapsuleCollider cc = go.GetComponent<CapsuleCollider>();
                    if (cc != null)
                    {
                        go.transform.position = hit.point + go.GetComponent<CapsuleCollider>().height * Vector3.up;
                    }
                    else
                    {
                        MeshFilter mf = go.GetComponent<MeshFilter>();
                        if(mf != null)
                        {
                            go.transform.position = hit.point + mf.sharedMesh.bounds.extents.y * Vector3.up;
                        }
                        else
                        {
                            go.transform.position = hit.point;
                        }
                    }
                }
            }
        }
    }

    //Show camera's perspective in scene
    [MenuItem("Editor Control/Switch to Game View &0")]
    static void GameView()
    {
        SceneView scene_view = SceneView.lastActiveSceneView;

        Camera gameCam = Camera.main;

        scene_view.rotation = gameCam.transform.rotation;
        scene_view.orthographic = true;
    }

    //Toggle between isometric and orthographic view
    [MenuItem("Editor Control/Toggle Isometric Orthographic &X")]
    static void ToggleGamePerspective()
    {
        SceneView scene_view = SceneView.lastActiveSceneView;

        scene_view.orthographic = !scene_view.orthographic;
    }

    //Set scene view to top projection
    [MenuItem("Editor Control/Switch to Isometric &G")]
    static void SwitchToGameIsometric()
    {
        SceneView scene_view = SceneView.lastActiveSceneView;

        scene_view.orthographic = false;
        scene_view.rotation = Quaternion.LookRotation(Vector3.down + Vector3.right - Vector3.forward);
    }

    //Set scene view to top projection
    [MenuItem("Editor Control/Switch to Front Projection &H")]
    static void SwitchToFront()
    {
        SceneView scene_view = SceneView.lastActiveSceneView;

        scene_view.orthographic = true;
        scene_view.rotation = Quaternion.LookRotation(Vector3.forward);
    }

    //Set scene view to top projection
    [MenuItem("Editor Control/Switch to Top Projection &J")]
    static void SwitchToTopDown()
    {
        SceneView scene_view = SceneView.lastActiveSceneView;

        scene_view.orthographic = true;
        scene_view.rotation = Quaternion.LookRotation(Vector3.down);
    }

    //Set scene view to top projection
    [MenuItem("Editor Control/Switch to Side Projection &K")]
    static void SwitchToSide()
    {
        SceneView scene_view = SceneView.lastActiveSceneView;

        scene_view.orthographic = true;
        scene_view.rotation = Quaternion.LookRotation(Vector3.right);
    }

    //Set scene view to wireframe
    [MenuItem("Editor Control/Switch to Wireframe &2")]
    static void SwitchToWireframe()
    {
        SceneView scene_view = SceneView.lastActiveSceneView;
        
        scene_view.cameraMode = SceneView.GetBuiltinCameraMode(DrawCameraMode.Wireframe);
    }

    //Set scene view to Unlit
    [MenuItem("Editor Control/Switch to Shaded Wire &3")]
    static void SwitchToShadedWire()
    {
        SceneView scene_view = SceneView.lastActiveSceneView;

        scene_view.cameraMode = SceneView.GetBuiltinCameraMode(DrawCameraMode.TexturedWire);
    }

    //Set scene view to Unlit
    [MenuItem("Editor Control/Switch to Shaded &4")]
    static void SwitchToShaded()
    {
        SceneView scene_view = SceneView.lastActiveSceneView;

        scene_view.cameraMode = SceneView.GetBuiltinCameraMode(DrawCameraMode.Textured);
    }

    //Set scene view to Unlit
    [MenuItem("Editor Control/Switch to Baked Albedo &5")]
    static void SwitchToBakedLightmap()
    {
        SceneView scene_view = SceneView.lastActiveSceneView;

        scene_view.cameraMode = SceneView.GetBuiltinCameraMode(DrawCameraMode.BakedAlbedo);
    }

    //Set scene view to top projection
    [MenuItem("Editor Control/Deselect All &D")]
    static void DeselectAll()
    {
        Selection.objects = new Object[0];
    }

    //Set scene view to top projection
    [MenuItem("Editor Control/Measure Distance &M")]
    static void MeasureDistance()
    {
        GameObject last = null;
        foreach (var o in Selection.objects)
        {
            if(o is GameObject)
            {
                if(last != null)
                {
                    Debug.Log(string.Format("Distance between {0} and {1} is: {2}", o.name, last.name, (((GameObject)o).transform.position - last.transform.position).magnitude));
                }
                last = (GameObject)o;
            }
        }
    }
}
#endif