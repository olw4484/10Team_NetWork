using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class YSJ_PopupBaseUI : JHT_BaseUI
{
    #region Field
    [SerializeField] private float _showTime = 1.0f;
    [SerializeField] private Color _backgroundColor = new Color(0f, 0f, 0f, 0.5f); // ������ ����
    [SerializeField] private GameObject _popupBody;

    private GameObject _background;

    public override YSJ_UIType UIType => YSJ_UIType.Popup;
    public int PopupIndex { get; private set; } = -1;

    #endregion

    #region Unity Method
    private void Start() => Open();

    #endregion

    #region BaseUI Method
    public override void Open()
    {
        base.Open();
        CreateBackground();
        StartCoroutine(ShowAnimation());
    }
    public override void Close()
    {
        base.Close();
        if (_background != null)
            Destroy(_background);
        Destroy(this.gameObject);
    }

    #endregion 

    #region Popup Method
    public void SetPopupIndex(int index)
    {
        PopupIndex = index;
    }

    public void OnClickClose() => Close();

    private IEnumerator ShowAnimation()
    {
        if (_popupBody == null) yield break;

        float elapsed = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one;

        _popupBody.transform.localScale = startScale;

        while (elapsed < _showTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _showTime);
            _popupBody.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        _popupBody.transform.localScale = endScale;
    }

    private void CreateBackground()
    {
        // �ؽ�ó ����
        var bgTex = new Texture2D(1, 1);
        bgTex.SetPixel(0, 0, _backgroundColor);
        bgTex.Apply();

        // ��� ������Ʈ ����
        _background = new GameObject("PopupBackground");
        var image = _background.AddComponent<Image>();
        var sprite = Sprite.Create(bgTex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
        image.sprite = sprite;

        // ��Ƽ���� �и�
        image.material = new Material(image.material);
        image.material.mainTexture = bgTex;

        // ���� ���� + ���� ���̵�
        image.color = _backgroundColor;
        image.canvasRenderer.SetAlpha(0f);
        image.CrossFadeAlpha(1f, 0.4f, false);

        _background.transform.SetParent(this.transform, false);
        _background.transform.localScale = Vector3.one;

        // ȭ�� ��ü ũ��� ����
        var rectTransform = _background.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        // �˾� �ٷ� �ڷ� ��ġ
        _background.transform.SetAsFirstSibling();
    }

    #endregion
}
