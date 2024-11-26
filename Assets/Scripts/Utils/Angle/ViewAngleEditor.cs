using UnityEditor;
using UnityEngine;

namespace Entity.ViewAngle
{
    // [CustomEditor(typeof(ViewAngleGame))]
    // public class ViewAngleEditor : Editor
    // {
    //     
    //     void OnSceneGUI() {
    //         ViewAngleGame vag = (ViewAngleGame)target;
    //         Handles.color = Color.red;
    //         Handles.DrawWireArc(vag.transform.position,
    //             Vector3.forward, Vector3.right, 360, vag.viewRadius);
    //         
    //         Vector3 viewAngleA = vag.DirFromAngle (-vag.viewAngle / 2, false);
    //         Vector3 viewAngleB = vag.DirFromAngle (vag.viewAngle / 2, false);
    //
    //         Handles.DrawLine (vag.transform.position, vag.transform.position + viewAngleA * vag.viewRadius);
    //         Handles.DrawLine (vag.transform.position, vag.transform.position + viewAngleB * vag.viewRadius);
    //         
    //         
    //         Handles.color = Color.green;
    //         foreach (Transform visibleTarget in vag.visibleTargets) {
    //             Handles.DrawLine (vag.transform.position, visibleTarget.position);
    //         }
    //     }
    // }
}