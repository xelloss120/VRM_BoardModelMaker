using System.IO;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Import : MonoBehaviour
{
    [SerializeField] Texture TexThumbnail;
    [SerializeField] Texture TexMain;
    [SerializeField] Texture TexBack;
    [SerializeField] Texture TexJoy;
    [SerializeField] Texture TexAngry;
    [SerializeField] Texture TexSorrow;
    [SerializeField] Texture TexFun;

    [SerializeField] RawImage RawThumbnail;
    [SerializeField] RawImage RawMain;
    [SerializeField] RawImage RawBack;
    [SerializeField] RawImage RawJoy;
    [SerializeField] RawImage RawAngry;
    [SerializeField] RawImage RawSorrow;
    [SerializeField] RawImage RawFun;

    [SerializeField] Material MatMain;
    [SerializeField] Material MatBack;
    [SerializeField] Material MatJoy;
    [SerializeField] Material MatAngry;
    [SerializeField] Material MatSorrow;
    [SerializeField] Material MatFun;

    [SerializeField] GameObject Loading;

    void Start()
    {
        // 動確後にデフォルトのテクスチャを再適用
        MatMain.SetTexture("_MainTex", TexMain);
        MatMain.SetTexture("_ShadeTexture", TexMain);
        MatBack.SetTexture("_MainTex", TexBack);
        MatBack.SetTexture("_ShadeTexture", TexBack);
        MatJoy.SetTexture("_MainTex", TexJoy);
        MatJoy.SetTexture("_ShadeTexture", TexJoy);
        MatAngry.SetTexture("_MainTex", TexAngry);
        MatAngry.SetTexture("_ShadeTexture", TexAngry);
        MatSorrow.SetTexture("_MainTex", TexSorrow);
        MatSorrow.SetTexture("_ShadeTexture", TexSorrow);
        MatFun.SetTexture("_MainTex", TexFun);
        MatFun.SetTexture("_ShadeTexture", TexFun);
    }

    public void OnClickThumbnail()
    {
        StartCoroutine(Load("Thumbnail", RawThumbnail, null));
    }

    public void OnClickMain()
    {
        StartCoroutine(Load("Main", RawMain, MatMain));
    }

    public void OnClickBack()
    {
        StartCoroutine(Load("Back", RawBack, MatBack));
    }

    public void OnClickJoy()
    {
        StartCoroutine(Load("Joy", RawJoy, MatJoy));
    }

    public void OnClickAngry()
    {
        StartCoroutine(Load("Angry", RawAngry, MatAngry));
    }

    public void OnClickSorrow()
    {
        StartCoroutine(Load("Sorrow", RawSorrow, MatSorrow));
    }

    public void OnClickFun()
    {
        StartCoroutine(Load("Fun", RawFun, MatFun));
    }

    /// <summary>
    /// 読み込み画像をテクスチャに適用
    /// </summary>
    IEnumerator Load(string name, RawImage raw, Material mat)
    {
        Loading.SetActive(true);
        yield return null;

#if UNITY_EDITOR
        var path = Application.dataPath + "/../_exe/_Tex/" + name + ".png";
#else
        var path = VRM.Samples.FileDialogForWindows.FileDialog("open Texture", ".png");
#endif
        if (!string.IsNullOrEmpty(path))
        {
            var tex = png2tex(path);
            if (tex != null)
            {
                raw.texture = tex;
                if (mat != null)
                {
                    mat.SetTexture("_MainTex", tex);
                    mat.SetTexture("_ShadeTexture", tex);
                }
            }
        }

        Loading.SetActive(false);
        yield return null;
    }

    /// <summary>
    /// png画像ファイル→テクスチャ
    /// </summary>
    Texture png2tex(string path)
    {
        // 画像ファイル読み込み
        var tex = new Texture2D(1, 1);
        var img = File.ReadAllBytes(path);
        tex.LoadImage(img);

        // 長辺側の大きさの正方形に転写
        var max = Mathf.Max(tex.width, tex.height);
        var squ = new Texture2D(max, max);
        var squPixels = squ.GetPixels();

        // 苦し紛れの並列化
        Parallel.For(0, squPixels.Length, i =>
        {
            squPixels[i] = Color.clear;
        });
        squ.SetPixels(squPixels);

        // こっちも並列化したかったけど良く分からなくて断念
        var offsetX = (max - tex.width) / 2;
        var offsetY = (max - tex.height) / 2;
        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                squ.SetPixel(x + offsetX, y + offsetY, tex.GetPixel(x, y));
            }
        }

        squ.Apply();

        return squ;
    }
}
