using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Random = System.Random;


public class ApplyColor : MonoBehaviour
{
    //List of Color Channel Materials
    [SerializeField]
    List<Material> channelMats;

    [SerializeField]
    FlexibleColorPicker colorPicker;

    [SerializeField]
    GameObject colPickObj;

    //List of Color Channel input fields
    [SerializeField]
    List<TMP_InputField> inputFields;

    //List of color panel selection checkboxes
    [SerializeField]
    List<GameObject> checkBoxes;

    //A class that converts hex codes to colors and vice versa
    [SerializeField]
    ColorConverter cc;

    [SerializeField]
    PaletteHistory hh;

    //Private Variables for changing the colors via color picker
    private List<bool> canChange;

    private Color previousColor;

    //Private variables for locking / unlocking the colors
    private List<bool> locked;

    // Start is called before the first frame update
    void Start()
    {
        //Make all colors uneditable and unlocked to start out
        canChange = new List<bool>();
        locked = new List<bool>();
        for (int i = 0; i < 6; i++)
        {
            canChange.Add(false);
            locked.Add(false);
        }
        UnlockAllCols();

        //Set channel colors to default and input fields to start
        for (int i = 0; i < channelMats.Count; i++)
        {
            channelMats[i].color = colorPicker.color;
            inputFields[i].SetTextWithoutNotify(cc.CreateHexFromColor(channelMats[i].color));
        }
        previousColor = colorPicker.color;

        colPickObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //Checks if each of the colors can be changed, and the current color is not the previous color
        //If this holds, change the color
        for (int i = 0; i < 6; i++)
        {
            if (canChange[i] && !previousColor.Equals(colorPicker.color))
            {
                channelMats[i].color = colorPicker.color;
                inputFields[i].SetTextWithoutNotify(cc.CreateHexFromColor(channelMats[i].color));
                //hh.newEntry();
            }
        }

        previousColor = colorPicker.color;
    }

    //Triggers when application quits
    void OnApplicationQuit()
    {
        for(int i = 0; i < 6; i++) {
            channelMats[i].color = cc.CreateColorFromHex("#E6B1B1");
        }
    }

    //Functions to monitor whether a color is editable or not
    public void OnlyEdit(int chNum)
    {
        if(checkBoxes[chNum].GetComponent<UnityEngine.UI.Toggle>().isOn)
        {
            for (int i = 0; i < checkBoxes.Count; i++)
            {
                if (i != chNum)
                {
                    checkBoxes[i].GetComponent<UnityEngine.UI.Toggle>().isOn = false;
                } else
                {
                    colPickObj.SetActive(true);
                    colorPicker.color = channelMats[i].color;
                }
            }
        }
        return;
    }

    //mark a color panel as editable when its check box is marked
    public void ToggleColor(int n)
    {
        canChange[n] = !canChange[n];
        if(!canChange.Contains(true)) 
        {
            colPickObj.SetActive(false);
        }
    }

    //Functions to lock/unlock each color
    public void UnlockCol(int n)
    {
        locked[n] = !locked[n];
    }

    public void UnlockAllCols()
    {
        for(int i = 0; i < 6; i++)
        {
            locked[i] = false;
        }
    }

    //Change all of the lock images to unlocked images, and set all colors as able to be randomized
    public void SetLocksImageToUnlocked()
    {
        //print("In image, col1: " + col1_locked);
        //Place holder before actual lock images
        transform.GetChild(0).gameObject.transform.GetChild(3).gameObject.GetComponent<UnityEngine.UI.Toggle>
            ().isOn = false;
        transform.GetChild(1).gameObject.transform.GetChild(3).gameObject.GetComponent<UnityEngine.UI.Toggle>
            ().isOn = false;
        transform.GetChild(2).gameObject.transform.GetChild(3).gameObject.GetComponent<UnityEngine.UI.Toggle>
            ().isOn = false;
        transform.GetChild(3).gameObject.transform.GetChild(3).gameObject.GetComponent<UnityEngine.UI.Toggle>
            ().isOn = false;
        transform.GetChild(4).gameObject.transform.GetChild(3).gameObject.GetComponent<UnityEngine.UI.Toggle>
            ().isOn = false;
        transform.GetChild(5).gameObject.transform.GetChild(3).gameObject.GetComponent<UnityEngine.UI.Toggle>
            ().isOn = false;
    }

    //Functions to change colors based on hex input
    public void ChangeCol1Hex(string changed)
    {
        if (changed.Length == 7)
        {
            channelMats[0].color = cc.CreateColorFromHex(changed);   
            //hh.newEntry();
        }
    }
    public void ChangeCol2Hex(string changed)
    {
        if (changed.Length == 7)
        {
            channelMats[1].color = cc.CreateColorFromHex(changed);  
            //hh.newEntry(); 
        }
    }
    public void ChangeCol3Hex(string changed)
    {
        if (changed.Length == 7)
        {
            channelMats[2].color = cc.CreateColorFromHex(changed);   
            //hh.newEntry();
        }
    }
    public void ChangeCol4Hex(string changed)
    {
        if (changed.Length == 7)
        {
            channelMats[3].color = cc.CreateColorFromHex(changed);   
            //hh.newEntry();
        }
    }
    public void ChangeCol5Hex(string changed)
    {
        if (changed.Length == 7)
        {
            channelMats[4].color = cc.CreateColorFromHex(changed); 
            //hh.newEntry();  
        }
    }
    public void ChangeCol6Hex(string changed)
    {
        if (changed.Length == 7)
        {
            channelMats[5].color = cc.CreateColorFromHex(changed);   
            //hh.newEntry();
        }
    }

    public void RandomizeUnlockedColors()
    //Retrives all materials and states on whether locked, then randomizes the color on the unlocked colors
    {
        List<Material> matList = new List<Material>();
        List<bool> lockedList = new List<bool>();
        Random randomNumGen= new Random();
        
        for(int i = 0; i < 6; ++i)
        {
            lockedList.Add(locked[i]);
        }
        
        for (int i = 0; i < 6; ++i)
        {
            matList.Add(transform.GetChild(i).gameObject.transform.GetChild(0).gameObject
                .GetComponent<UnityEngine.UI.Image>().material);
        }
        for (int i = 0; i < 6; ++i)
        {
            if (!lockedList[i])
            {
                double red = randomNumGen.NextDouble();
                double blue = randomNumGen.NextDouble();
                double green = randomNumGen.NextDouble();
                Color colToSet = new Color((float)red, (float)blue, (float)green);
                matList[i].color = colToSet;
                transform.GetChild(i).gameObject.transform.GetChild(2).gameObject.
                    GetComponent<TMP_InputField>().text = cc.CreateHexFromColor(colToSet);
            }
        }
    }
}
