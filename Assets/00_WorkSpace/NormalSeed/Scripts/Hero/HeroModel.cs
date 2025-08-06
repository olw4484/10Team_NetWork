using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroModel : MonoBehaviour
{
    // 기본 스탯
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public float MaxHP { get; set; }
    [field: SerializeField] public float MaxMP { get; set; }
    [field: SerializeField] public float MoveSpd { get; set; }
    [field: SerializeField] public float Atk { get; set; }
    [field: SerializeField] public float AtkRange { get; set; }
    [field: SerializeField] public float AtkSpd { get; set; }
    [field: SerializeField] public float Def { get; set; }
    [field: SerializeField] public float HPGen { get; set; }
    [field: SerializeField] public float MPGen { get; set; }

    // Observable Properties
    public ObservableProperty<float> CurHP { get; private set; } = new();
    public ObservableProperty<float> CurMP { get; private set; } = new();
    public ObservableProperty<int> Level { get; private set; } = new();

    private Dictionary<int, HeroStat> levelStats = new();

    public void GetInitStats(int heroType)
    {
        string csvName = heroType switch
        {
            0 => "Hero1Stats",
            1 => "Hero2Stats",
            2 => "Hero3Stats",
            _ => null
        };

        if (csvName == null) return;

        LoadStatsFromCSV(csvName);

        if (!levelStats.ContainsKey(1)) return;

        HeroStat stat = levelStats[1]; // 1레벨 정보

        Name = stat.Name;
        MaxHP = stat.MaxHP;
        MaxMP = stat.MaxMP;
        MoveSpd = stat.MoveSpd;
        Atk = stat.Atk;
        AtkRange = stat.AtkRange;
        AtkSpd = stat.AtkSpd;
        Def = stat.Def;
        HPGen = stat.HPGen;
        MPGen = stat.MPGen;

        Level.Value = 1;
    }

    /// <summary>
    /// CSV 파일로 저장되어 있는 Hero의 Stats를 불러오는 메서드
    /// </summary>
    /// <param name="fileName"></param>
    private void LoadStatsFromCSV(string fileName)
    {
        TextAsset csvFile = Resources.Load<TextAsset>($"HeroStats/{fileName}");
        // Stat 초기화
        levelStats.Clear();

        string[] lines = csvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] tokens = lines[i].Split(',');

            if (tokens.Length < 11) continue;

            int level = int.Parse(tokens[0]);
            HeroStat stat = new(
                tokens[1],
                int.Parse(tokens[2]),
                int.Parse(tokens[3]),
                float.Parse(tokens[4]),
                float.Parse(tokens[5]),
                float.Parse(tokens[6]),
                float.Parse(tokens[7]),
                float.Parse(tokens[8]),
                float.Parse(tokens[9]),
                float.Parse(tokens[10])
            );

            levelStats[level] = stat;
        }
    }
    /// <summary>
    /// 레벨에 따라 Stat을 불러오는 메서드
    /// </summary>
    /// <param name="heroType"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public HeroStat GetStatByLevel(int heroType, int level)
    {
        string csvName = heroType switch
        {
            0 => "Hero1Stats",
            1 => "Hero2Stats",
            2 => "Hero3Stats",
            _ => null
        };

        if (csvName == null) return default;

        if (levelStats.Count == 0 || !levelStats.ContainsKey(level))
        {
            LoadStatsFromCSV(csvName);
        }

        levelStats.TryGetValue(level, out HeroStat stat);
        return stat;
    }
}

public struct HeroStat
{
    public string Name;
    public int MaxHP, MaxMP;
    public float MoveSpd, Atk, AtkRange, AtkSpd, Def, HPGen, MPGen;

    public HeroStat(string name, int maxHP, int maxMP, float moveSpd, float atk, float atkRange, float atkSpd, float def, float hpGen, float mpGen)
    {
        Name = name;
        MaxHP = maxHP;
        MaxMP = maxMP;
        MoveSpd = moveSpd;
        Atk = atk;
        AtkRange = atkRange;
        AtkSpd = atkSpd;
        Def = def;
        HPGen = hpGen;
        MPGen = mpGen;
    }
}
