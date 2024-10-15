using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyColor : MonoBehaviour
{
    //Public Variables
    public Material color1Material;
    public Material color2Material;
    public Material color3Material;
    public Material color4Material;
    public Material color5Material;
    public Material color6Material;
    public FlexibleColorPicker colorPicker;
    
    //Private Variables
    private bool canChangeCol1;
    private bool canChangeCol2;
    private bool canChangeCol3;
    private bool canChangeCol4;
    private bool canChangeCol5;
    private bool canChangeCol6;
    private Color previousColor;
    
    // Start is called before the first frame update
    void Start()
    {
        canChangeCol1 = true;
        canChangeCol2 = true;
        canChangeCol3 = true;
        canChangeCol4 = true;
        canChangeCol5 = true;
        canChangeCol6 = true;
        color1Material.color = colorPicker.color;
        color2Material.color = colorPicker.color;
        color3Material.color = colorPicker.color;
        color4Material.color = colorPicker.color;
        color5Material.color = colorPicker.color;
        color6Material.color = colorPicker.color;
        previousColor = colorPicker.color;
    }

    // Update is called once per frame
    void Update()
    {
        //Checks if each of the colors can be changed, and the current color is not the previous color
        //If this holds, change the color
        if (canChangeCol1 && !previousColor.Equals(colorPicker.color))
        {
            color1Material.color = colorPicker.color;
        }

        if (canChangeCol2 && !previousColor.Equals(colorPicker.color))
        {
            color2Material.color = colorPicker.color;
        }

        if (canChangeCol3 && !previousColor.Equals(colorPicker.color))
        {
            color3Material.color = colorPicker.color;
        }

        if (canChangeCol4 && !previousColor.Equals(colorPicker.color))
        {
            color4Material.color = colorPicker.color;
        }

        if (canChangeCol5 && !previousColor.Equals(colorPicker.color))
        {
            color5Material.color = colorPicker.color;
        }

        if (canChangeCol6 && !previousColor.Equals(colorPicker.color))
        {
            color6Material.color = colorPicker.color;
        }

    previousColor = colorPicker.color;
    }

    //Functions to monitor whether a color is toggled or not
    public void toggleColor1(bool canChange1)
    {
        canChangeCol1 = !canChangeCol1;
    }

    public void toggleColor2(bool canChange2)
    {
        canChangeCol2 = !canChangeCol2;
    }

    public void toggleColor3(bool canChange3)
    {
        canChangeCol3 = !canChangeCol3;
    }

    public void toggleColor4(bool canChange4)
    {
        canChangeCol4 = !canChangeCol4;
    }

    public void toggleColor5(bool canChange5)
    {
        canChangeCol5 = !canChangeCol5;
    }

    public void toggleColor6(bool canChange6)
    {
        canChangeCol6 = !canChangeCol6;
    }
}
