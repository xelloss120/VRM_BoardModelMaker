using UnityEngine;

public class AdjViewSize : MonoBehaviour
{
    [SerializeField] RectTransform View;
    [SerializeField] RectTransform Panel;

    /// <summary>
    /// 3Dビューのサイズ調整
    /// </summary>
    void Update()
    {
        var size = Mathf.Min(Panel.rect.width, Panel.rect.height);
        View.localScale = Vector3.one * size / 100;
    }
}
