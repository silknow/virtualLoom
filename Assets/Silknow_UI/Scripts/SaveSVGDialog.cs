

using System.Collections;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;

[RequireComponent(typeof(Button))]
public class SaveSVGDialog : MonoBehaviour, IPointerDownHandler {
    // Sample text data
    private string _data = "Example text created by StandaloneFileBrowser";

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);

    // Browser plugin should be called in OnPointerDown.
    public void OnPointerDown(PointerEventData eventData) {
        WizardController.instance.ToggleProcessingWindow();
        
        StartCoroutine(SaveWebSVG(0.5f));
       
    }
    IEnumerator SaveWebSVG(float delayTime)
    {
        //yield return new WaitForSeconds(delayTime);
        var path = Application.persistentDataPath + "/textile.svg";
        if (!string.IsNullOrEmpty(path))
        {
            WizardController.instance.GenerateSVG(path);
            //yield return new WaitForSeconds(2);
            if (File.Exists(path))
            {
                byte[] byteArray = File.ReadAllBytes(path);
                DownloadFile(gameObject.name, "OnFileDownload", "textile.svg", byteArray, byteArray.Length);
            }
        }

        WizardController.instance.ToggleProcessingWindow();
        yield return null;
    }

    // Called from browser
    public void OnFileDownload() {
        //output.text = "File Successfully Downloaded";
    }
#else
    //
    // Standalone platforms & editor
    //
    public void OnPointerDown(PointerEventData eventData) { }

    // Listen OnClick event in standlone builds
    void Start() {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void OnClick() {
        
        var path = StandaloneFileBrowser.SaveFilePanel("Save SVG as", "", "textile", "svg");
        if (!string.IsNullOrEmpty(path)) {
            WizardController.instance.ToggleProcessingWindow();
            StartCoroutine(SaveLocalSVG(path,0.5f));

        }
    }

    IEnumerator SaveLocalSVG(string path, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        WizardController.instance.GenerateSVG(path);
        WizardController.instance.ToggleProcessingWindow();
    }
#endif
}