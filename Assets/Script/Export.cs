using System.IO;
using UnityEngine;
using UnityEngine.UI;
using VRM;

public class Export : MonoBehaviour
{
    [SerializeField] GameObject Model;

    [SerializeField] RawImage Thumbnail;
    [SerializeField] InputField Title;
    [SerializeField] InputField Version;
    [SerializeField] InputField Author;
    [SerializeField] InputField Contact;
    [SerializeField] InputField Reference;
    [SerializeField] Dropdown AllowedUser;
    [SerializeField] Dropdown Violent;
    [SerializeField] Dropdown Sexual;
    [SerializeField] Dropdown Commercial;
    [SerializeField] InputField OtherPermissionUrl;
    [SerializeField] Dropdown License;
    [SerializeField] InputField OtherLicenseUrl;
    [SerializeField] InputField Height;

    [SerializeField] Text ExportText;
    [SerializeField] Text ExportErrorText;

    Vector2 ErrorPos;
    string ErrorMsg;

    void Start()
    {
        Height.text = "150";

#if UNITY_EDITOR
        Height.text = "200";
        Title.text = "Title";
        Version.text = "Version";
        Author.text = "Author";
#endif
        ErrorPos = ExportText.GetComponent<RectTransform>().offsetMax;
        ExportText.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        ErrorMsg = ExportErrorText.text;
        ExportErrorText.text = "";
    }

    /// <summary>
    /// モデルの出力
    /// </summary>
    public void Save()
    {
        if (Title.text == "" || Version.text == "" || Author.text == "")
        {
            ExportText.GetComponent<RectTransform>().offsetMax = ErrorPos;
            ExportErrorText.text = ErrorMsg;
            return;
        }
        ExportText.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        ExportErrorText.text = "";

#if UNITY_EDITOR
        var path = Application.dataPath + "/../_exe/Export.vrm";
#else
        var path = VRM.Samples.FileDialogForWindows.SaveDialog("save VRM", Application.dataPath + "/../");
#endif
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        else if (string.Compare(Path.GetExtension(path), ".vrm", true) != 0)
        {
            path += ".vrm";
        }

        // 表情リセット
        var proxy = Model.GetComponent<VRMBlendShapeProxy>();
        proxy.AccumulateValue(BlendShapePreset.Joy, 0);
        proxy.AccumulateValue(BlendShapePreset.Angry, 0);
        proxy.AccumulateValue(BlendShapePreset.Sorrow, 0);
        proxy.AccumulateValue(BlendShapePreset.Fun, 0);
        proxy.Apply();

        // 身長適用
        var scaling = Instantiate(Model);
        scaling.transform.localScale = Vector3.one * float.Parse(Height.text) / 100;

        // 再正規化
        var model = BoneNormalizer.Execute(scaling, false, false);
        VRMExportSettings.CopyVRMComponents(scaling, model.Root, model.BoneMap);

        // メタ情報
        var meta = model.Root.GetComponent<VRMMeta>().Meta;
        meta.Thumbnail = GetTex2D(Thumbnail.texture);
        meta.Title = Title.text;
        meta.Version = Version.text;
        meta.Author = Author.text;
        meta.ContactInformation = Contact.text;
        meta.Reference = Reference.text;
        meta.AllowedUser = (AllowedUser)AllowedUser.value;
        meta.ViolentUssage = (UssageLicense)Violent.value;
        meta.SexualUssage = (UssageLicense)Sexual.value;
        meta.CommercialUssage = (UssageLicense)Commercial.value;
        meta.OtherPermissionUrl = OtherPermissionUrl.text;
        meta.LicenseType = (LicenseType)License.value;
        meta.OtherLicenseUrl = OtherLicenseUrl.text;

        // 出力
        var vrm = VRMExporter.Export(model.Root);
        var bytes = vrm.ToGlbBytes();
        File.WriteAllBytes(path, bytes);

        Destroy(scaling);
        Destroy(model.Root);
    }

    /// <summary>
    /// TextureからTexture2Dを作成
    /// </summary>
    Texture2D GetTex2D(Texture tex, TextureFormat format = TextureFormat.RGBA32)
    {
        var tex2d = new Texture2D(tex.width, tex.height, format, false);
        var active = RenderTexture.active;
        var rt = new RenderTexture(tex.width, tex.height, 32);
        Graphics.Blit(Thumbnail.texture, rt);
        RenderTexture.active = rt;
        var source = new Rect(0, 0, rt.width, rt.height);
        tex2d.ReadPixels(source, 0, 0);
        tex2d.Apply();
        RenderTexture.active = active;

        return tex2d;
    }
}
