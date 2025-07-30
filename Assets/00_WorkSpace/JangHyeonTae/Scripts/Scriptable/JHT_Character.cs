using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Character/SelectCharacter")]
public class JHT_Character : ScriptableObject
{
    [field : SerializeField] public string name {private set; get;}
    [field : SerializeField] public Sprite icon {private set; get;}
    [field : SerializeField] public string desc {private set; get;}
}
