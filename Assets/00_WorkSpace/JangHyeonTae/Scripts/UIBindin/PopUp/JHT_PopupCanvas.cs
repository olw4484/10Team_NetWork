using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JHT_PopupCanvas : MonoBehaviour
{
    private Stack<JHT_BaseUI> popUpStack = new Stack<JHT_BaseUI>();

    public void AddUI(JHT_BaseUI ui)
    {
        if (popUpStack.Count > 0)
        {
            JHT_BaseUI top = popUpStack.Peek();
            top.gameObject.SetActive(false);
        }

        popUpStack.Push(ui);
        ui.gameObject.SetActive(true);
    }

    public void RemoveUI()
    {
        if (popUpStack.Count == 0)
            return;

        JHT_BaseUI top = popUpStack.Pop();
        top.gameObject.SetActive(false);

        if (popUpStack.Count > 0)
        {
            top = popUpStack.Peek();
            top.gameObject.SetActive(true);
        }
    }

}
