using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface KMS_IExpReceiver
{
    void AddExp(int amount);
    int GetCurrentExp();
    int GetLevel();
}
