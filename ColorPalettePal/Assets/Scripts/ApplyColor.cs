using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class ApplyColor : MonoBehaviour
{
    //Public Variables
    public Material color1_material;
    public Material color2_material;
    public Material color3_material;
    public Material color4_material;
    public Material color5_material;
    public Material color6_material;
    public FlexibleColorPicker color_picker;
    public GameObject palette_manager;
    
    //Private Variables
    private bool can_change_col1;
    private bool can_change_col2;
    private bool can_change_col3;
    private bool can_change_col4;
    private bool can_change_col5;
    private bool can_change_col6;
    private Color previousColor;
    
    // Start is called before the first frame update
    void Start()
    {
        var col1Input = palette_manager.transform.GetChild(0).gameObject.GetComponent<InputField>();
        col1Input.text = "SAMPLE";
        can_change_col1 = true;
        can_change_col2 = true;
        can_change_col3 = true;
        can_change_col4 = true;
        can_change_col5 = true;
        can_change_col6 = true;
        color1_material.color = color_picker.color;
        color2_material.color = color_picker.color;
        color3_material.color = color_picker.color;
        color4_material.color = color_picker.color;
        color5_material.color = color_picker.color;
        color6_material.color = color_picker.color;
        previousColor = color_picker.color;
    }

    // Update is called once per frame
    void Update()
    {
        //Checks if each of the colors can be changed, and the current color is not the previous color
        //If this holds, change the color
        if (can_change_col1 && !previousColor.Equals(color_picker.color))
        {
            color1_material.color = color_picker.color;
            
        }

        if (can_change_col2 && !previousColor.Equals(color_picker.color))
        {
            color2_material.color = color_picker.color;
        }

        if (can_change_col3 && !previousColor.Equals(color_picker.color))
        {
            color3_material.color = color_picker.color;
        }

        if (can_change_col4 && !previousColor.Equals(color_picker.color))
        {
            color4_material.color = color_picker.color;
        }

        if (can_change_col5 && !previousColor.Equals(color_picker.color))
        {
            color5_material.color = color_picker.color;
        }

        if (can_change_col6 && !previousColor.Equals(color_picker.color))
        {
            color6_material.color = color_picker.color;
        }

    previousColor = color_picker.color;
    }

    //Functions to monitor whether a color is toggled or not
    public void toggleColor1(bool canChange1)
    {
        can_change_col1 = !can_change_col1;
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
    
    //Functions to change colors based on hex input
    public void changeCol1Hex(string changed)
    {
        if (changed.Length == 7)
        {
            color1_material.color = createColorFromHex(changed);   
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
}
