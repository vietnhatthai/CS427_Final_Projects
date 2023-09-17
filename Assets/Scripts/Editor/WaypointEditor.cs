using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaypointManager))]
public class WaypointEditor : Editor
{
    private WaypointManager waypointManager;

    private void OnEnable()
    {
        waypointManager = (WaypointManager)target;
    }

    public override void OnInspectorGUI()
    {
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Clear Waypoints", GUILayout.Height(50)))
        {
            ClearWaypoints();
        }
        GUI.backgroundColor = Color.white;

        GUI.backgroundColor = Color.blue;
        if (GUILayout.Button("Remove Last Waypoint", GUILayout.Height(50)))
        {
            if (waypointManager.waypoints.Count > 0)
            {
                Waypoint waypoint = waypointManager.waypoints[waypointManager.waypoints.Count - 1];
                waypointManager.waypoints.Remove(waypoint);
                DestroyImmediate(waypoint.gameObject);
            }
        }
        GUI.backgroundColor = Color.white;

        //base.OnInspectorGUI();
    }

    private void ClearWaypoints()
    {
        foreach (var waypoint in waypointManager.waypoints)
        {
            DestroyImmediate(waypoint.gameObject);
        }

        waypointManager.waypoints.Clear();
    }

    private void OnSceneGUI()
    {
        HandleWaypointCreation();
    }

    private void HandleWaypointCreation()
    {
        Event e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 0 && e.control)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                waypointManager.AddWaypoint(hit.point);
                e.Use();
            }
        }
    }
}
