using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface KMS_ISelectable
{
    public enum KMS_SelectableType { None, Minion, Building }
    void Select();
    void Deselect();
    KMS_SelectableType GetSelectableType(); // À¯´Ö, °Ç¹° µî
}
