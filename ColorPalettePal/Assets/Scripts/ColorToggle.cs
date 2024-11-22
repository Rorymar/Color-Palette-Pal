using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;
using System.ComponentModel.Design.Serialization;

public class ColorToggle : MonoBehaviour
{
    // Serialized Fields for the UI components
    [SerializeField]
    TMP_InputField[] hexInputs; //Fields the user inputs hex codes into

    [SerializeField]
    Material[] colorPanels; //The color display rectangles

    [SerializeField]
    Slider redSlider;

    [SerializeField]
    Slider greenSlider;

    [SerializeField]
    Slider blueSlider;

    [SerializeField]
    Slider monoSlider;

    [SerializeField]
    TMP_Text colorBlindnessType; //textbox to display type of color blindness being simulated

    [SerializeField]
    ColorConverter cc;

    // To store the original colors from hex input
    private Color[] originalColors;

    // Default hex colors to start with for demo purposes
    private readonly string[] defaultHexColors = { "#FF0000", "#FFFF00", "#00FF00", "#00FFFF", "#0000FF", "#FF00FF" };

    void Start()
    {
        // Initialize the original colors array with the length of hexInputs
        originalColors = new Color[hexInputs.Length];

        // Set default color for each panel and hex input
        for (int i = 0; i < hexInputs.Length; i++)
        {
            // Ensure we don't exceed the defaultHexColors array length
            if (i < defaultHexColors.Length)
            {
                // Set default hex input, apply color to panel
                hexInputs[i].text = defaultHexColors[i];
                UpdatePanelColor(i);
            }

            int index = i;
            hexInputs[i].onEndEdit.AddListener(delegate { UpdatePanelColor(index); });
        }
        
        colorBlindnessType.text = "Normal Vision";
    }

    // Convert hex to color and update panel color
    public void UpdatePanelColor(int index)
    {
        // Bounds check to prevent out-of-range errors
        if (index < 0 || index >= hexInputs.Length || index >= colorPanels.Length)
        {
            Debug.LogError("Index out of bounds: " + index);
            return;
        }

        string hexCode = hexInputs[index].text;
        //print(hexCode);
        if (ColorUtility.TryParseHtmlString(hexCode, out Color color1))
        {
            Color color = cc.createColorFromHex(hexCode);
            // Store original color, update panel, apply color blindness adjustments
            originalColors[index] = color;
            colorPanels[index].color = color;
            UpdateColors();
        }
        else
        {
            Debug.LogError("Invalid hex code entered: " + hexCode);
        }
    }

    // Adjust colors based on slider values
    public void UpdateColors()
    {
        // Convert sliders to factor
        float redFactor = GetFactorFromSliderValue((int)redSlider.value);
        float greenFactor = GetFactorFromSliderValue((int)greenSlider.value);
        float blueFactor = GetFactorFromSliderValue((int)blueSlider.value);
        float monoFactor = GetFactorFromSliderValue((int)monoSlider.value);

        for (int i = 0; i < colorPanels.Length; i++)
        {
            // Ensure we stay within bounds
            if (i >= originalColors.Length)
                continue;

            // Start with original color
            Color color = originalColors[i];
            Vector3 rgb = new Vector3(color.r, color.g, color.b);

            Vector3 adjustedrgb;
            if (redFactor != 1f)
            {
                // Apply protan color blindness
                adjustedrgb = ApplyBrettel(rgb, Brettel1997.Protan);
                adjustedrgb.x += color.r * redFactor;
            } else if (greenFactor != 1f) {
                // Apply deuteran color blindness
                adjustedrgb = ApplyBrettel(rgb, Brettel1997.Deutan);
                adjustedrgb.y += color.g * greenFactor;
            } else if (blueFactor != 1f) {
                // Apply tritan color blindness
                adjustedrgb = ApplyBrettel(rgb, Brettel1997.Tritan);
                adjustedrgb.z += color.b * blueFactor;
            } else if (redFactor == 1f && greenFactor == 1f && blueFactor == 1f && monoFactor == 1f) {
                // Normal Vision
                adjustedrgb = rgb;
            } else {
                // Apply total color blindness
                adjustedrgb.x = color.r * monoFactor + (1 - monoFactor) * GetGrayScale(color);
                adjustedrgb.y = color.g * monoFactor + (1 - monoFactor) * GetGrayScale(color);
                adjustedrgb.z = color.b * monoFactor + (1 - monoFactor) * GetGrayScale(color);
            }

            // Update color panel with adjusted color
            Color adjustedColor = new Color(adjustedrgb.x, adjustedrgb.y, adjustedrgb.z);
            colorPanels[i].color = adjustedColor;

            // Update hex input field to display the current color
            //hexInputs[i].SetTextWithoutNotify(cc.createHexFromColor(adjustedColor));
        }
    }

    //on slider change, sets all other sliders to 0 and adjusts the text in the color blindness textbox
    public void zeroSliders(int numSlide)
    {
        if (numSlide == 1)
        { // Changing Red Slider
            greenSlider.value = 0;
            blueSlider.value = 0;
            monoSlider.value = 0;

            if (redSlider.value == 4)
            {
                colorBlindnessType.text = "Protanopia";
            } else if (redSlider.value >= 2)
            {
                colorBlindnessType.text = "Protanomaly";
            } else
            {
                colorBlindnessType.text = "Normal Vision";
            }
        }
        else if (numSlide == 2)
        { // Changing Green Slider
            redSlider.value = 0;
            blueSlider.value = 0;
            monoSlider.value = 0;

            if (greenSlider.value == 4)
            {
                colorBlindnessType.text = "Deuteranopia";
            }
            else if (greenSlider.value >= 2)
            {
                colorBlindnessType.text = "Deuteranomaly";
            }
            else
            {
                colorBlindnessType.text = "Normal Vision";
            }
        }
        else if (numSlide == 3)
        { // Changing Blue Slider
            redSlider.value = 0;
            greenSlider.value = 0;
            monoSlider.value = 0;

            if (blueSlider.value == 4)
            {
                colorBlindnessType.text = "Tritanopia";
            }
            else if (blueSlider.value >= 2)
            {
                colorBlindnessType.text = "Tritanomaly";
            }
            else
            {
                colorBlindnessType.text = "Normal Vision";
            }
        }
        else
        { // Changing Monochrome Slider
            redSlider.value = 0;
            greenSlider.value = 0;
            blueSlider.value = 0;

            if (monoSlider.value == 4)
            {
                colorBlindnessType.text = "Achromatopsia";
            }
            else if (monoSlider.value >= 2)
            {
                colorBlindnessType.text = "Achromatomaly";
            }
            else
            {
                colorBlindnessType.text = "Normal Vision";
            }
        }
    }

    // Convert a color to grayscale
    // Blending and this overall section adapted from https://support.ptc.com/help/mathcad/r10.0/en/index.html#page/PTC_Mathcad_Help/example_grayscale_and_color_in_images.html
    private float GetGrayScale(Color color)
    {
        return color.r * 0.299f + color.g * 0.587f + color.b * 0.114f;
    }

    // Convert slider values (0-4) to a corresponding factor for blending
    private float GetFactorFromSliderValue(int sliderValue)
    {
        switch (sliderValue)
        {
            case 0: return 1.0f; // 0% blindness, full color
            case 1: return 0.75f; // 25% blindness
            case 2: return 0.5f;  // 50% blindness
            case 3: return 0.25f; // 75% blindness
            case 4: return 0.0f;  // 100% blindness, complete gray
            default: return 1.0f; // Fallback to full color
        }
    }

    // apply filter based on selected option (for protan, deutan, and tritan)
    private Vector3 ApplyBrettel(Vector3 rgb, Brettel1997.Params parameters)
    {
        float dotProduct = Vector3.Dot(rgb, parameters.SeparationPlane);
        float[,] matrix = dotProduct >= 0 ? parameters.Matrix1 : parameters.Matrix2;

        return new Vector3(
            matrix[0, 0] * rgb.x + matrix[0, 1] * rgb.y + matrix[0, 2] * rgb.z,
            matrix[1, 0] * rgb.x + matrix[1, 1] * rgb.y + matrix[1, 2] * rgb.z,
            matrix[2, 0] * rgb.x + matrix[2, 1] * rgb.y + matrix[2, 2] * rgb.z
        );
    }
}