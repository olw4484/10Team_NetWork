using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroMPBarUI : MonoBehaviour
{
    private Canvas canvas;
    private RectTransform rect;
    public Vector3 offset;
    public Image hpImage;
    public Transform targetTransform;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        rect = GetComponent<RectTransform>();
        offset = new Vector3(0, 60, 0);
    }

    //private void LateUpdate()
    //{
    //    Vector3 screenPos = Camera.main.WorldToScreenPoint(targetTransform.position);
    //    rect.position = screenPos + offset;
    //}
}
