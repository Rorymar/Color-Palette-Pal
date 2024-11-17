using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;
using TMPro;

[RequireComponent(typeof(Button))]
public class PaletteSaver : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private List<TMP_InputField> hex_inputs;
    
    // Sample text data
    private string save_data;

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);

    // Broser plugin should be called in OnPointerDown.
    public void OnPointerDown(PointerEventData eventData) {
        var bytes = Encoding.UTF8.GetBytes(_data);
        DownloadFile(gameObject.name, "OnFileDownload", "sample.txt", bytes, bytes.Length);
    }

    // Called from browser
    public void OnFileDownload() {
        output.text = "File Successfully Downloaded";
    }
#else
    //
    // Standalone platforms & editor
    //
    public void OnPointerDown(PointerEventData eventData) { }

    // Listen OnClick event in standlone builds
    void Start()
    {
        save_data = "";
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void OnClick() {
        //Get all 6 hex values and string them together
        save_data = "Generated Palette:\n";
        for (int i = 0; i < hex_inputs.Count; i++)
        {
            save_data += hex_inputs[i].text;
            if (i < hex_inputs.Count - 1)
            {
                save_data += "\n";
            }
        }

        var extension = new ExtensionFilter[]
        {
            new ExtensionFilter("Text File (.txt)", "txt")
        };
        var path = StandaloneFileBrowser.SaveFilePanel("Title", "", "palette", extension);
        if (!string.IsNullOrEmpty(path)) {
            if (!path.EndsWith(".txt"))
                //Extra check to see if .txt was added, without this, the file browser crashes (on linux at least)
            {
                path += ".txt";
            }
            File.WriteAllText(path, save_data);
        }
    }
#endif
}
