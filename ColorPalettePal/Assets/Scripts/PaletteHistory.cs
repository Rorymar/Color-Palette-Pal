using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PaletteHistory : MonoBehaviour
{
    //IMPORTANT NOTE: the length of histDisplays MUST be the same as histSize!!!
    [SerializeField]
    int histSize;

    [SerializeField]
    List<TMP_Text> histDisplays;

    [SerializeField]
    List<TMP_InputField> hexSources;

    [SerializeField]
    GameObject paletteScreen;

    [SerializeField]
    GameObject historyScreen;

    private List<List<string>> history;
    
    // Start is called before the first frame update
    void Start()
    {
        history = new List<List<string>>();
        for(int i = 0; i < histSize; i++) {
            history.Add(new List<string>(6));
            for(int j = 0; j < 6; j++) {
                history[i].Add("#000000");
            }
        }

        displayHistory();
        //print(history[0][0]);
    }

    //takes a list of six hex codes as input and adds the list to history, with new entries being added to the beginning
    public void newEntry() {
        List<string> newPal = new List<string>(6);
        for(int i = 0; i < 6; i++) {
            newPal.Add(hexSources[i].text);
        }
        
        for(int i = 4; i > 0; i--) {
            history[i] = history[i-1];
        }
        history[0] = newPal;
        displayHistory();
        return;
    }

    public void switchDisplays() {
        paletteScreen.SetActive(!paletteScreen.activeSelf);
        historyScreen.SetActive(!historyScreen.activeSelf);
    }

    //display the hex codes stored in history
    private void displayHistory() {
        for(int i = 0; i < histSize; i++) {
            string outStr = history[i][0];
            for(int j = 1; j < 6; j++) {
                outStr = outStr + ", " + history[i][j];
            }
            histDisplays[i].text = outStr;
        }
        return;
    }
}
