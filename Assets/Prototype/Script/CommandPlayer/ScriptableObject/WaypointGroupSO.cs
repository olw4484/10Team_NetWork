using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WaypointGroup : MonoBehaviour
{
    // �ν����Ϳ��� ���� (��: "Red_Top", "Blue_Mid" ��)
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
