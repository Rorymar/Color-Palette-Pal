using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class ImageHexSwap : MonoBehaviour
{
    private Material panelMaterial;

    [SerializeField]
    ColorConverter cc;

    //A variable for the proper format of a hex code.
    private string hexFormat = @"^#([0-9a-fA-F]{3}|[0-9a-fA-F]{6})(?:[0-9a-fA-F]{2})?$";

    void Start()
    {
        panelMaterial = GetComponent<Image>().material;
    }

    //Updates the color of a material panel from a hex if it is valid
    public void UpdateColor(string hex)
    {
        if(IsValidHex(hex))
        {
            panelMaterial.color = cc.CreateColorFromHex(hex);
        }
        else
        {
            Debug.Log("Error: Invalid Hex Code");
        }
    }

    //Returns if boolean is a valid hex
    private bool IsValidHex(string hex)
    {
        return Regex.IsMatch(hex,hexFormat);
    }
}
