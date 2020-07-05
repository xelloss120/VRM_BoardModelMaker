using UnityEngine;
using VRM;

public class SetBlendShapeProxy : MonoBehaviour
{
    [SerializeField] VRMBlendShapeProxy Proxy;

    public void Main()
    {
        Set(0, 0, 0, 0);
    }

    public void Joy()
    {
        Set(1, 0, 0, 0);
    }

    public void Angry()
    {
        Set(0, 1, 0, 0);
    }

    public void Sorrow()
    {
        Set(0, 0, 1, 0);
    }

    public void Fun()
    {
        Set(0, 0, 0, 1);
    }

    /// <summary>
    /// 確認用の表情設定
    /// </summary>
    void Set(float joy, float angry, float sorrow, float fun)
    {
        Proxy.AccumulateValue(BlendShapePreset.Joy, joy);
        Proxy.AccumulateValue(BlendShapePreset.Angry, angry);
        Proxy.AccumulateValue(BlendShapePreset.Sorrow, sorrow);
        Proxy.AccumulateValue(BlendShapePreset.Fun, fun);
        Proxy.Apply();
    }
}
