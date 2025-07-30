using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionOwnershipZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var minion = other.GetComponent<MinionController>();
        if (minion != null)
        {
            minion.SetManualControl(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var minion = other.GetComponent<MinionController>();
        if (minion != null)
        {
            minion.SetManualControl(false);
        }
    }
}
