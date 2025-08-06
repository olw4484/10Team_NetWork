using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WaypointGroup : MonoBehaviour
{
    // 인스펙터에서 세팅 (예: "Red_Top", "Blue_Mid" 등)
    public string groupId;

    [SerializeField]
    private Transform[] waypoints;

    public int GetWaypointCount() => waypoints.Length;
    public Transform GetWaypoint(int idx)
    {
        if (idx < 0 || idx >= waypoints.Length) return null;
        return waypoints[idx];
    }
    public Transform[] GetAllWaypoints() => waypoints;
}
