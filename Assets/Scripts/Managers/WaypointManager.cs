using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class WaypointManager : MonoBehaviour
{
    public List<Waypoint> waypoints = new List<Waypoint>();

    public void AddWaypoint(Vector3 position)
    {
        GameObject waypointObject = new GameObject("Waypoint-" + waypoints.Count);
        waypointObject.transform.position = position;
        waypointObject.transform.SetParent(transform);
        Waypoint waypoint = waypointObject.AddComponent<Waypoint>();
        waypoints.Add(waypoint);
    }

    private void OnDrawGizmos()
    {
        // draw spheres at each waypoint
        foreach (var waypoint in waypoints)
        {
            if (waypoint == null)
            {
                continue;
            }
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(waypoint.transform.position, 0.1f);
        }

        // draw lines between waypoints
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            if (waypoints[i] == null || waypoints[i + 1] == null)
            {
                continue;
            }
            Gizmos.color = Color.red;
            //Gizmos.DrawLine(waypoints[i].transform.position, waypoints[i + 1].transform.position);
            DrawThickLine(waypoints[i].transform.position, waypoints[i + 1].transform.position, 1f);
        }
    }

    public static void DrawThickLine(Vector3 start, Vector3 end, float thickness)
    {
        Camera c = Camera.current;
        if (c == null) return;

        // Only draw on normal cameras
        if (c.clearFlags == CameraClearFlags.Depth || c.clearFlags == CameraClearFlags.Nothing)
        {
            return;
        }

        // Only draw the line when it is the closest thing to the camera
        // (Remove the Z-test code and other objects will not occlude the line.)
        var prevZTest = Handles.zTest;
        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;

        Handles.color = Gizmos.color;
        Handles.DrawAAPolyLine(thickness * 10, new Vector3[] { start, end });

        Handles.zTest = prevZTest;
    }


    public Vector3 GetWaypointPosition(int index)
    {
        if (index < 0 || index >= waypoints.Count)
        {
            return Vector3.zero;
        }
        return waypoints[index].transform.position;
    }

    public int Count()
    {
        return waypoints.Count;
    }
}
