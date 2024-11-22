using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        history.Add(new List<string>(1));
        history[0].Add("You don't have any palettes yet!");
        for(int i = 1; i < histSize; i++) {
            history.Add(new List<string>(1));
            history[i].Add("");
        }
        

        displayHistory();
        //print(history[0][0]);
    }

    //takes a list of six hex codes as input and adds the list to history, with new entries being added to the beginning
    public void newEntry() {
        List<string> newPal = new List<string>();
        for(int i = 0; i < 6; i++) {
            if(hexSources[i].text == "Hex Code" || newPal.Contains(hexSources[i].text)){
                continue;
            }
            newPal.Add(hexSources[i].text);
        }

        //Used to retrieve extant hex group if it exists
        List<string> sortednewPal = newPal.OrderBy(s => s).ToList();
        int index = history.FindIndex(l => l.OrderBy(s => s).SequenceEqual(sortednewPal));
        if(index < 0){
            index = history.Count - 1;
        }
        //Shift down and update
        for(int i = index; i > 0; i--) {
            if(history[i-1][0] == "You don't have any palettes yet!"){
                continue;
            }
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
            for(int j = 1; j < history[i].Count; j++) {
                outStr = outStr + ", " + history[i][j];
            }
            histDisplays[i].text = outStr;
        }
        return;
    }
}