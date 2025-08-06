using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroHPBarUI : MonoBehaviour
{
    private Camera mainCam;
    private RectTransform rect;
    public Vector3 offset;
    public Image hpImage;
    public Image mpImage;
    public Transform targetTransform;

    private void Start()
    {
        mainCam = Camera.main;
        rect = GetComponent<RectTransform>();
        offset = new Vector3(0f, 60f, 0f);
    }

    private void LateUpdate()
    {
        if (targetTransform == null) return;

        Vector3 screenPos = mainCam.WorldToScreenPoint(targetTransform.position);
        rect.position = screenPos + offset;
        //Vector3 worldPos = targetTransform.position + offset;
        //rect.position = worldPos;
        //transform.LookAt(mainCam.transform);
        //float distance = Vector3.Distance(mainCam.transform.position, rect.position);
        //float pixelHeight = 100f; // 원하는 화면상의 높이 (px)
        //float worldHeight = 2f * distance * Mathf.Tan(mainCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        //float scaleFactor = (pixelHeight / Screen.height) * worldHeight;

        //rect.localScale = Vector3.one * scaleFactor;
    }
}
