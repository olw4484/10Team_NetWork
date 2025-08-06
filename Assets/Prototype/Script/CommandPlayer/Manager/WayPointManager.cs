using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public static WaypointManager Instance { get; private set; }

    // 인스펙터에서 WaypointGroup들 연결
    [SerializeField] private List<WaypointGroup> waypointGroups;

    private Dictionary<string, WaypointGroup> waypointDict = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Dict에 등록 (groupId 기준)
        foreach (var group in waypointGroups)
        {
            if (group == null || string.IsNullOrWhiteSpace(group.groupId))
                continue;

            if (!waypointDict.ContainsKey(group.groupId))
                waypointDict.Add(group.groupId, group);
            else
                Debug.LogWarning($"[WaypointManager] Duplicate groupId: {group.groupId}");
        }
    }

    // groupId로 검색
    public WaypointGroup GetWaypointGroup(string groupId)
    {
        waypointDict.TryGetValue(groupId, out var group);
        return group;
    }

    // (확장) 팀/라인 기반 빠른 조회 (키 생성 규칙을 맞춰줘야 함)
    public WaypointGroup GetWaypointGroup(int teamId, string lane)
    {
        string key = $"Team{teamId}_{lane}";
        waypointDict.TryGetValue(key, out var group);
        return group;
    }
}