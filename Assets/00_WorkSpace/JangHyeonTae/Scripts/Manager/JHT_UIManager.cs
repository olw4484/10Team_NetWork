using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class JHT_UIManager : MonoBehaviour
{
    //#region singleton
    //private static JHT_UIManager uiInstance;
    //public static JHT_UIManager UIInstance { get { return uiInstance; } }
    //
    //private void Awake()
    //{
    //    if (uiInstance == null)
    //    {
    //        uiInstance = this;
    //        DontDestroyOnLoad(gameObject);
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }
    //}
    //#endregion


    private JHT_PopupCanvas popUpCanvas;
    public JHT_PopupCanvas PopUpCanvas
    {
        get
        {
            if (popUpCanvas != null)
            {
                return popUpCanvas;
            }

            popUpCanvas = FindObjectOfType<JHT_PopupCanvas>();

            if (popUpCanvas != null)
            {
                return popUpCanvas;
            }

            JHT_PopupCanvas prefab = Resources.Load<JHT_PopupCanvas>("PopUpCanvas");
            return Instantiate(prefab);
        }
    }

    public T ShowPopUp<T>(string objName) where T : JHT_BaseUI
    {
        Transform child = PopUpCanvas.GetComponentsInChildren<Transform>(true)
                                .FirstOrDefault(t => t.name == objName); //물어볼거

        T instance = child.GetComponent<T>();

        //T prefab = Resources.Load<T>($"PopUpUI/{objName}");
        //T instance = Instantiate(prefab, PopUpCanvas.transform);

        if (instance == null)
            Debug.Log("null");

        PopUpCanvas.GetComponent<JHT_PopupCanvas>().AddUI(instance);
        return instance;
    }

    public void ClosePopUp()
    {
        PopUpCanvas.GetComponent<JHT_PopupCanvas>().RemoveUI();
    }

}
