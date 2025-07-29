using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Character/SelectCharacter")]
public class JHT_Character : ScriptableObject
{
    public string name;
    public Sprite icon;
    public string desc;
}
