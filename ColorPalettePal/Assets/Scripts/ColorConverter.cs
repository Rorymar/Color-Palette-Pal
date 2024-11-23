using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class ColorConverter : MonoBehaviour
{
    //convert a string hex code into a Color
    public Color createColorFromHex(string hex)
    {
        var red = int.Parse(hex.Substring(1, 2), NumberStyles.HexNumber) / 255f;
        var green = int.Parse(hex.Substring(3, 2), NumberStyles.HexNumber) / 255f;
        var blue = int.Parse(hex.Substring(5, 2), NumberStyles.HexNumber) / 255f;
        return new Color(red, green, blue);
    }

    //convert a Color into a string hex code
    public string createHexFromColor(Color color)
    {
        int red = (int)(color.r * 255);
        int green = (int)(color.g * 255);
        int blue = (int)(color.b * 255);
        string hex = "#" + red.ToString("X2") + green.ToString("X2")
                     + blue.ToString("X2");
        return hex.ToUpper();
    }
}
