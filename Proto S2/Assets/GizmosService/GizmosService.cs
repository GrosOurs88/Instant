using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class GizmosService : MonoBehaviour 
{
  static public GizmosService _instance;

  public enum GizmoAction{ COLOR, WIRECUBE, WIRESPHERE, CUBE, SPHERE, PLANE, LINE, TEXT, ARROW, CONE };
  
  public List<Hashtable> gizmosInfos = new List<Hashtable>();

  bool isFirstUpdate = true;

  [ExecuteInEditMode]
  IEnumerator Start()
  {
    while(true)
    {
      yield return new WaitForEndOfFrame();
      gizmosInfos.Clear();  
    }
  }
  
  public static GizmosService instance()
  {
    if(_instance == null)
    {
      GameObject o = GameObject.Find("GizmosService");
      if(o == null)
      {
        o = new GameObject("GizmosService");
        _instance = o.AddComponent<GizmosService>();
      }else
      {
        _instance = o.GetComponent<GizmosService>();
      }
      
      
    }
    return _instance;
  }


  public static void Plane(Vector3 pos, Vector3 normal, float size)
  {
    Hashtable o = new Hashtable(); 
    o["action"] = GizmoAction.PLANE;
    o["position"] = pos;
    o["normal"] = normal;
    o["size"] = size;
    instance().gizmosInfos.Add(o); 
  }


  public static void Text(string text, Vector3 pos)
  {
    Hashtable o = new Hashtable(); 
    o["action"] = GizmoAction.TEXT;
    o["position"] = pos;
    o["text"] = text;
    instance().gizmosInfos.Add(o);
  }

  public static void Color(Color c)
  {
    Hashtable o = new Hashtable(); 
    o["action"] = GizmoAction.COLOR;
    o["color"] = c;
    instance().gizmosInfos.Add(o);
  }

  public static void WireCube(Vector3 pos, float size)
  {
    Hashtable o = new Hashtable();
    o["action"] = GizmoAction.WIRECUBE;
    o["position"] = pos;
    o["size"] = size;
    instance().gizmosInfos.Add(o);
  }  

  public static void Sphere(Vector3 pos, float radius)
  {
    Hashtable o = new Hashtable();
    o["action"] = GizmoAction.SPHERE;
    o["position"] = pos;
    o["radius"] = radius;
    instance().gizmosInfos.Add(o);
  }

  public static void WireSphere(Vector3 pos, float radius)
  {
    Hashtable o = new Hashtable();
    o["action"] = GizmoAction.WIRESPHERE;
    o["position"] = pos;
    o["radius"] = radius;
    instance().gizmosInfos.Add(o);
  }

  public static void Arrow(Vector3 pos, Vector3 dir)
  {
    Hashtable o = new Hashtable();
    o["action"] = GizmoAction.ARROW;
    o["position"] = pos;
    o["dir"] = dir;
    instance().gizmosInfos.Add(o);
  }

  // draw a cone that "looks" towards dir
  // and has a "angle" degree visibility around that direction
  // distance gives the size of the cone
  // and "normal" the orientation of the plane on which the
  // cone is drawn
  public static void Cone(Vector3 pos, Vector3 dir, Vector3 normal, float distance, float angle)
  {
    Hashtable o = new Hashtable();
    o["action"] = GizmoAction.CONE;
    o["position"] = pos;
    o["dir"] = dir;
    o["normal"] = normal;
    o["distance"] = distance;
    o["angle"] = angle;
    instance().gizmosInfos.Add(o);
  }



  public static void Line(Vector3 p1, Vector3 p2)
  {
    Hashtable o = new Hashtable();
    o["action"] = GizmoAction.LINE;
    o["p1"] = p1;
    o["p2"] = p2;
    instance().gizmosInfos.Add(o);
  }


  /** DRAW FUNCTIONS **/

  void DrawPlane(Vector3 pos, Vector3 normal, float size)
  {
    Gizmos.matrix = Matrix4x4.TRS(pos, 
                                  Quaternion.LookRotation(normal),
                                  Vector3.one);
    // scale the cube so that the axis aligned with plane normal
    // is flatened
    Vector3 planeGizmoScale = Vector3.one;
    planeGizmoScale -= normal;
    Gizmos.DrawCube(pos, planeGizmoScale * size + normal * 0.01f);
    // restore gizmos matrix
    Gizmos.matrix = Matrix4x4.TRS(Vector3.zero, 
                                  Quaternion.identity,
                                  Vector3.one);
  }

  void DrawArrow(Vector3 pos, Vector3 dir)
  {
#if UNITY_EDITOR
    dir.Normalize();
    //float size = UnityEditor.HandleUtility.GetHandleSize(pos);
    Quaternion rot = Quaternion.LookRotation(dir);
    UnityEditor.Handles.ArrowCap(0, pos, rot, 1);
#endif
  }

  void DrawCone(Vector3 pos, Vector3 dir, 
                Vector3 normal, float distance, 
                float angle)
  {
#if UNITY_EDITOR
    Vector3 leftVector = Quaternion.AngleAxis(-angle, normal) * dir * distance;
    Gizmos.DrawLine(pos,
      pos + leftVector);
    // right
    Vector3 rightVector = Quaternion.AngleAxis(angle, normal) * dir * distance;
    Gizmos.DrawLine(pos,
      pos + rightVector);
    // circular part (draw a number of segments from rightmost point to leftmost point)
    Vector3 lastPt = pos + rightVector;
    int numSegments = 20;
    float angleStep = angle * 2.0f / numSegments; //  how many degrees do we rotate at each step
    for (int i = 0; i <= numSegments; i++)
    {
      Vector3 newPt = pos + Quaternion.AngleAxis(-angleStep * i, normal) * rightVector;
      Gizmos.DrawLine(lastPt, newPt);
      lastPt = newPt;
    }
#endif
  }

  void DrawText(string text, Vector3 worldPos)
  {
#if UNITY_EDITOR
    UnityEditor.Handles.Label(worldPos, text);
#endif
  }

  void OnDrawGizmos()
  {
    for(int i = 0; i < gizmosInfos.Count; i++)
    {
      Hashtable info = gizmosInfos[i];
      GizmoAction action = (GizmoAction)info["action"];
      switch(action)
      {
        case GizmoAction.COLOR:
        Gizmos.color = (Color)info["color"];
#if UNITY_EDITOR
        UnityEditor.Handles.color = (Color)info["color"];
#endif
        break;

        case GizmoAction.PLANE:
        DrawPlane((Vector3)info["position"], (Vector3)info["normal"], (float)info["size"]);
        break;

        case GizmoAction.LINE:
        Gizmos.DrawLine((Vector3)info["p1"], (Vector3)info["p2"]);
        break;        

        case GizmoAction.ARROW:
        DrawArrow((Vector3)info["position"], (Vector3)info["dir"]);
        break;

        case GizmoAction.CONE:
        DrawCone((Vector3)info["position"], (Vector3)info["dir"],
                  (Vector3)info["normal"],(float)info["distance"],
                  (float)info["angle"]);
        break;

        case GizmoAction.TEXT:
        DrawText((string)info["text"], (Vector3)info["position"]);
        break;

        case GizmoAction.SPHERE :
        Gizmos.DrawSphere((Vector3)info["position"], (float)info["radius"]);
        break;

        case GizmoAction.WIRESPHERE : 
        Gizmos.DrawWireSphere((Vector3)info["position"], (float)info["radius"]);
        break;

        case GizmoAction.WIRECUBE : 
        Gizmos.DrawWireCube((Vector3)info["position"], Vector3.one * (float)info["size"]);
        break;
      }
    }
  }
}
