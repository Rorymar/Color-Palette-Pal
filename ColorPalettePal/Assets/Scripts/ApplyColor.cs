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
    //Public Variable Materials
    public Material color1_material;
    public Material color2_material;
    public Material color3_material;
    public Material color4_material;
    public Material color5_material;
    public Material color6_material;
    public FlexibleColorPicker color_picker;

    //Public Variable input fields
    public TMP_InputField input_field1;
    public TMP_InputField input_field2;
    public TMP_InputField input_field3;
    public TMP_InputField input_field4;
    public TMP_InputField input_field5;
    public TMP_InputField input_field6;

    //Private Variables for changing the colors via color picker
    private bool can_change_col1;
    private bool can_change_col2;
    private bool can_change_col3;
    private bool can_change_col4;
    private bool can_change_col5;
    private bool can_change_col6;
    private Color previousColor;

    //Private variables for locking / unlocking the colors
    private bool col1_locked;
    private bool col2_locked;
    private bool col3_locked;
    private bool col4_locked;
    private bool col5_locked;
    private bool col6_locked;

    [SerializeField]
    List<GameObject> checkBoxes;

    // Start is called before the first frame update
    void Start()
    {
        can_change_col1 = false;
        can_change_col2 = false;
        can_change_col3 = false;
        can_change_col4 = false;
        can_change_col5 = false;
        can_change_col6 = false;
        unlockAllCols();
        color1_material.color = color_picker.color;
        color2_material.color = color_picker.color;
        color3_material.color = color_picker.color;
        color4_material.color = color_picker.color;
        color5_material.color = color_picker.color;
        color6_material.color = color_picker.color;
        previousColor = color_picker.color;

        //Set input fields to start
        input_field1.SetTextWithoutNotify(createHexFromColor(color1_material.color));
        input_field2.SetTextWithoutNotify(createHexFromColor(color2_material.color));
        input_field3.SetTextWithoutNotify(createHexFromColor(color3_material.color));
        input_field4.SetTextWithoutNotify(createHexFromColor(color4_material.color));
        input_field5.SetTextWithoutNotify(createHexFromColor(color5_material.color));
        input_field6.SetTextWithoutNotify(createHexFromColor(color6_material.color));
    }

    // Update is called once per frame
    void Update()
    {
        //Checks if each of the colors can be changed, and the current color is not the previous color
        //If this holds, change the color
        if (can_change_col1 && !previousColor.Equals(color_picker.color))
        {
            color1_material.color = color_picker.color;
            input_field1.SetTextWithoutNotify(createHexFromColor(color1_material.color));

        }

        if (can_change_col2 && !previousColor.Equals(color_picker.color))
        {
            color2_material.color = color_picker.color;
            input_field2.SetTextWithoutNotify(createHexFromColor(color2_material.color));
        }

        if (can_change_col3 && !previousColor.Equals(color_picker.color))
        {
            color3_material.color = color_picker.color;
            input_field3.SetTextWithoutNotify(createHexFromColor(color3_material.color));
        }

        if (can_change_col4 && !previousColor.Equals(color_picker.color))
        {
            color4_material.color = color_picker.color;
            input_field4.SetTextWithoutNotify(createHexFromColor(color4_material.color));
        }

        if (can_change_col5 && !previousColor.Equals(color_picker.color))
        {
            color5_material.color = color_picker.color;
            input_field5.SetTextWithoutNotify(createHexFromColor(color5_material.color));
        }

        if (can_change_col6 && !previousColor.Equals(color_picker.color))
        {
            color6_material.color = color_picker.color;
            input_field6.SetTextWithoutNotify(createHexFromColor(color6_material.color));
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

    public void toggleColor1(bool canChange1)
    {
        can_change_col1 = !can_change_col1;
        //print(canChange1);
    }

    public void toggleColor2(bool canChange2)
    {
        can_change_col2 = !can_change_col2;
    }

    public void toggleColor3(bool canChange3)
    {
        can_change_col3 = !can_change_col3;
    }

    public void toggleColor4(bool canChange4)
    {
        can_change_col4 = !can_change_col4;
    }

    public void toggleColor5(bool canChange5)
    {
        can_change_col5 = !can_change_col5;
    }

    public void toggleColor6(bool canChange6)
    {
        can_change_col6 = !can_change_col6;
    }

    //Functions to lock/unlock each color
    public void unlockCol1(bool lockStatus)
    {
        col1_locked = !col1_locked;
    }
    public void unlockCol2(bool lockStatus)
    {
        col2_locked = !col2_locked;
    }
    public void unlockCol3(bool lockStatus)
    {
        col3_locked = !col3_locked;
    }

    public void unlockCol4(bool lockStatus)
    {
        col4_locked = !col4_locked;
    }

    public void unlockCol5(bool lockStatus)
    {
        col5_locked = !col5_locked;
    }

    public void unlockCol6(bool lockStatus)
    {
        col6_locked = !col6_locked;
    }

    public void unlockAllCols()
    {
        col1_locked = false;
        print(this.col1_locked);
        col2_locked = false;
        col3_locked = false;
        col4_locked = false;
        col5_locked = false;
        col6_locked = false;
    }

    public void setLocksImageToUnlocked()
    {
        print("In image, col1: " + col1_locked);
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
            color1_material.color = createColorFromHex(changed);   
        }
    }
    public void changeCol2Hex(string changed)
    {
        if (changed.Length == 7)
        {
            color2_material.color = createColorFromHex(changed);   
        }
    }
    public void changeCol3Hex(string changed)
    {
        if (changed.Length == 7)
        {
            color3_material.color = createColorFromHex(changed);   
        }
    }
    public void changeCol4Hex(string changed)
    {
        if (changed.Length == 7)
        {
            color4_material.color = createColorFromHex(changed);   
        }
    }
    public void changeCol5Hex(string changed)
    {
        if (changed.Length == 7)
        {
            color5_material.color = createColorFromHex(changed);   
        }
    }
    public void changeCol6Hex(string changed)
    {
        if (changed.Length == 7)
        {
            color6_material.color = createColorFromHex(changed);   
        }
    }

    public void randomizeUnlockedColors()
    //Retrives all materials and states on whether locked, then randomizes the color on the unlocked colors
    {
        List<Material> mat_list = new List<Material>();
        List<bool> locked_list = new List<bool>();
        Random random_num_gen= new Random();
        
        locked_list.Add(col1_locked);
        print("Color 1: " + col1_locked);
        locked_list.Add(col2_locked);
        locked_list.Add(col3_locked);
        locked_list.Add(col4_locked);
        locked_list.Add(col5_locked);
        locked_list.Add(col6_locked);
        
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
