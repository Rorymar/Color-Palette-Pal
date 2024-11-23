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
    FlexibleColorPicker color_picker;

    //List of Color Channel input fields
    [SerializeField]
    List<TMP_InputField> input_fields;

    //List of color panel selection checkboxes
    [SerializeField]
    List<GameObject> checkBoxes;

    //A class that converts hex codes to colors and vice versa
    [SerializeField]
    ColorConverter cc;

    [SerializeField]
    PaletteHistory hh;

    //Private Variables for changing the colors via color picker
    private List<bool> can_change;

    private Color previousColor;

    //Private variables for locking / unlocking the colors
    private List<bool> locked;

    // Start is called before the first frame update
    void Start()
    {
        //Make all colors uneditable and unlocked to start out
        can_change = new List<bool>();
        locked = new List<bool>();
        for (int i = 0; i < 6; i++)
        {
            can_change.Add(false);
            locked.Add(false);
        }
        unlockAllCols();

        //Set channel colors to default and input fields to start
        for (int i = 0; i < channelMats.Count; i++)
        {
            channelMats[i].color = color_picker.color;
            input_fields[i].SetTextWithoutNotify(cc.createHexFromColor(channelMats[i].color));
        }
        previousColor = color_picker.color;
    }

    // Update is called once per frame
    void Update()
    {
        //Checks if each of the colors can be changed, and the current color is not the previous color
        //If this holds, change the color
        for (int i = 0; i < 6; i++)
        {
            if (can_change[i] && !previousColor.Equals(color_picker.color))
            {
                channelMats[i].color = color_picker.color;
                input_fields[i].SetTextWithoutNotify(cc.createHexFromColor(channelMats[i].color));
                //hh.newEntry();
            }
        }

        previousColor = color_picker.color;
    }

    //Triggers when application quits
    void OnApplicationQuit()
    {
        for(int i = 0; i < 6; i++) {
            channelMats[i].color = cc.createColorFromHex("#E6B1B1");
        }
    }

    //Functions to monitor whether a color is editable or not
    public void onlyEdit(int chNum)
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
                    color_picker.color = channelMats[i].color;
                }
            }
        }
        return;
    }

    //mark a color panel as editable when its check box is marked
    public void toggleColor(int n)
    {
        can_change[n] = !can_change[n];
    }

    //Functions to lock/unlock each color
    public void unlockCol(int n)
    {
        locked[n] = !locked[n];
    }

    public void unlockAllCols()
    {
        for(int i = 0; i < 6; i++)
        {
            locked[i] = false;
        }
    }

    //Change all of the lock images to unlocked images, and set all colors as able to be randomized
    public void setLocksImageToUnlocked()
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
    public void changeCol1Hex(string changed)
    {
        if (changed.Length == 7)
        {
            channelMats[0].color = cc.createColorFromHex(changed);   
            //hh.newEntry();
        }
    }
    public void changeCol2Hex(string changed)
    {
        if (changed.Length == 7)
        {
            channelMats[1].color = cc.createColorFromHex(changed);  
            //hh.newEntry(); 
        }
    }
    public void changeCol3Hex(string changed)
    {
        if (changed.Length == 7)
        {
            channelMats[2].color = cc.createColorFromHex(changed);   
            //hh.newEntry();
        }
    }
    public void changeCol4Hex(string changed)
    {
        if (changed.Length == 7)
        {
            channelMats[3].color = cc.createColorFromHex(changed);   
            //hh.newEntry();
        }
    }
    public void changeCol5Hex(string changed)
    {
        if (changed.Length == 7)
        {
            channelMats[4].color = cc.createColorFromHex(changed); 
            //hh.newEntry();  
        }
    }
    public void changeCol6Hex(string changed)
    {
        if (changed.Length == 7)
        {
            channelMats[5].color = cc.createColorFromHex(changed);   
            //hh.newEntry();
        }
    }

    public void randomizeUnlockedColors()
    //Retrives all materials and states on whether locked, then randomizes the color on the unlocked colors
    {
        List<Material> mat_list = new List<Material>();
        List<bool> locked_list = new List<bool>();
        Random random_num_gen= new Random();
        
        for(int i = 0; i < 6; ++i)
        {
            locked_list.Add(locked[i]);
        }
        
        for (int i = 0; i < 6; ++i)
        {
            mat_list.Add(transform.GetChild(i).gameObject.transform.GetChild(0).gameObject
                .GetComponent<UnityEngine.UI.Image>().material);
        }
        for (int i = 0; i < 6; ++i)
        {
            if (!locked_list[i])
            {
                double red = random_num_gen.NextDouble();
                double blue = random_num_gen.NextDouble();
                double green = random_num_gen.NextDouble();
                Color col_to_set = new Color((float)red, (float)blue, (float)green);
                mat_list[i].color = col_to_set;
                transform.GetChild(i).gameObject.transform.GetChild(2).gameObject.
                    GetComponent<TMP_InputField>().text = cc.createHexFromColor(col_to_set);
            }
        }
    }
}
