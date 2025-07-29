using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface KMS_ISelectable
{
    public enum SelectableType { None, Minion, Building }
    void Select();
    void Deselect();
    SelectableType GetSelectableType(); // À¯´Ö, °Ç¹° µî
}
