using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public static WaypointManager Instance { get; private set; }

    // �ν����Ϳ��� WaypointGroup�� ����
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

        // Dict�� ��� (groupId ����)
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

    // groupId�� �˻�
    public WaypointGroup GetWaypointGroup(string groupId)
    {
        waypointDict.TryGetValue(groupId, out var group);
        return group;
    }

    // (Ȯ��) ��/���� ��� ���� ��ȸ (Ű ���� ��Ģ�� ������� ��)
    public WaypointGroup GetWaypointGroup(int teamId, string lane)
    {
        string key = $"Team{teamId}_{lane}";
        waypointDict.TryGetValue(key, out var group);
        return group;
    }
}