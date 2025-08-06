public interface ILoadingUI
{
    void Init();
    void Show(string message);
    void Hide();
    void UpdateMessage(string message);

    float GetProgress();
    void SetProgress(float value);
}
