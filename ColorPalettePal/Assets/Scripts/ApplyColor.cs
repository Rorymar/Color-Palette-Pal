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

    public FlexibleColorPicker color_picker;

    //List of Color Channel input fields
    [SerializeField]
    List<TMP_InputField> input_fields;

    //Private Variables for changing the colors via color picker
    private List<bool> can_change;

    private Color previousColor;

    //Private variables for locking / unlocking the colors
    private List<bool> locked;

    [SerializeField]
    List<GameObject> checkBoxes;

    // Start is called before the first frame update
    void Start()
    {
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
            input_fields[i].SetTextWithoutNotify(createHexFromColor(channelMats[i].color));
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
                input_fields[i].SetTextWithoutNotify(createHexFromColor(channelMats[i].color));

            }
        }

        previousColor = color_picker.color;
    }

    //Functions to monitor whether a color is toggled or not
    public void onlyEdit(int chNum)
    {
        if(checkBoxes[chNum].GetComponent<UnityEngine.UI.Toggle>().isOn)
        {
            for (int i = 0; i < checkBoxes.Count; i++)
            {
                if (i != chNum)
                {
                    checkBoxes[i].GetComponent<UnityEngine.UI.Toggle>().isOn = false;
                }
            }
        }
        //print(chNum);
        return;
    }

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
            channelMats[0].color = createColorFromHex(changed);   
        }
    }
    public void changeCol2Hex(string changed)
    {
        if (changed.Length == 7)
        {
            channelMats[1].color = createColorFromHex(changed);   
        }
    }
    public void changeCol3Hex(string changed)
    {
        if (changed.Length == 7)
        {
            channelMats[2].color = createColorFromHex(changed);   
        }
    }
    public void changeCol4Hex(string changed)
    {
        if (changed.Length == 7)
        {
            channelMats[3].color = createColorFromHex(changed);   
        }
    }
    public void changeCol5Hex(string changed)
    {
        if (changed.Length == 7)
        {
            channelMats[4].color = createColorFromHex(changed);   
        }
    }
    public void changeCol6Hex(string changed)
    {
        if (changed.Length == 7)
        {
            channelMats[5].color = createColorFromHex(changed);   
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
                    GetComponent<TMP_InputField>().text = createHexFromColor(col_to_set);
            }
        }
    }
    
    //Private functions used by others
    private Color createColorFromHex(string hex)
    {
        var red = int.Parse(hex.Substring(1, 2), NumberStyles.HexNumber) / 255f;
        var green = int.Parse(hex.Substring(3, 2), NumberStyles.HexNumber) / 255f;
        var blue = int.Parse(hex.Substring(5, 2), NumberStyles.HexNumber) / 255f;
        return new Color(red, green, blue);
    }

    private string createHexFromColor(Color color)
    {
        int red = (int)(color.r * 255);
        int green = (int)(color.g * 255);
        int blue = (int)(color.b * 255);
        string hex = "#" + red.ToString("X2") + green.ToString("X2")
                     + blue.ToString("X2");
        return hex.ToUpper();
    }
}
