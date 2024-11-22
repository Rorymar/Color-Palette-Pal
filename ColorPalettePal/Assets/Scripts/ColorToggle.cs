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
            // Ensure we don�t exceed the defaultHexColors array length
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
            Vector3 rgb = new Vector3(color.r, color.g, color.b); //NEW

            /*float adjustedRed;
            float adjustedGreen;
            float adjustedBlue;*/
            Vector3 adjustedrgb; //NEW
            if (/*monoFactor == 1f*/ redFactor != 1f)
            {
                // Apply partial color blindness
                /*adjustedRed = color.r * redFactor + (1 - redFactor) * GetGrayScale(color);
                adjustedGreen = color.g * greenFactor + (1 - greenFactor) * GetGrayScale(color);
                adjustedBlue = color.b * blueFactor + (1 - blueFactor) * GetGrayScale(color);*/
                adjustedrgb = ApplyBrettel(rgb, Brettel1997.Protan); //NEW
            } else if (greenFactor != 1f) {
                adjustedrgb = ApplyBrettel(rgb, Brettel1997.Deutan); //NEW
            } else if (blueFactor != 1f) {
                adjustedrgb = ApplyBrettel(rgb, Brettel1997.Tritan); //NEW
            } else if (redFactor == 1f && greenFactor == 1f && blueFactor == 1f && monoFactor == 1f) {
                adjustedrgb = rgb;
            } else {
                // Apply total color blindness
                /*adjustedRed = color.r * monoFactor + (1 - monoFactor) * GetGrayScale(color);
                adjustedGreen = color.g * monoFactor + (1 - monoFactor) * GetGrayScale(color);
                adjustedBlue = color.b * monoFactor + (1 - monoFactor) * GetGrayScale(color);*/
                adjustedrgb = ApplyAchromatopsia(rgb); //NEW
            }

            // Update color panel with adjusted color
            //Color adjustedColor = new Color(adjustedRed, adjustedGreen, adjustedBlue, color.a);
            Color adjustedColor = new Color(adjustedrgb.x, adjustedrgb.y, adjustedrgb.z); //NEW
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

///////////////////////////////////////////////////////////////////////////////////TESTING////////////////////////////////////////////////////////////////////////////////

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

    // adapted from https://mmuratarat.github.io/2020-05-13/rgb_to_grayscale_formulas
    private Vector3 ApplyAchromatopsia(Vector3 rgb)
    {
        float gray = rgb.x * 0.299f + rgb.y * 0.587f + rgb.z * 0.114f;
        return new Vector3(gray, gray, gray);
    }

    private void UpdateColorPanel(int index, Color newColor)
    {
        colorPanels[index].color = newColor;
        //userColors[index] = newColor;
    }

    private void ResetColorPanel(int index)
    {
        colorPanels[index].color = Color.white;
        //userColors[index] = null;
        hexInputs[index].SetTextWithoutNotify("");
    }

    // ensuring filter hex code is not invalid/goes past 6 digits
    private Color ClampColor(Color color)
    {
        return new Color(
            Mathf.Clamp01(color.r),
            Mathf.Clamp01(color.g),
            Mathf.Clamp01(color.b),
            Mathf.Clamp01(color.a)
        );
    }
}

// Brettel 1997 algorithm for color blindness - this is for protan, deutan, and tritan
// adapted from https://github.com/MaPePeR/jsColorblindSimulator/blob/master/brettel_colorblind_simulation.js
/*public static class Brettel1997
{
    public struct Params
    {
        public float[,] Matrix1;
        public float[,] Matrix2;
        public Vector3 SeparationPlane;
    }

    public static readonly Params Protan = new Params
    {
        Matrix1 = new float[,]
        {
            { 0.14510f, 1.20165f, -0.34675f },
            { 0.10447f, 0.85316f, 0.04237f },
            { 0.00429f, -0.00603f, 1.00174f }
        },
        Matrix2 = new float[,]
        {
            { 0.14115f, 1.16782f, -0.30897f },
            { 0.10495f, 0.85730f, 0.03776f },
            { 0.00431f, -0.00586f, 1.00155f }
        },
        SeparationPlane = new Vector3(0.00048f, 0.00416f, -0.00464f)
    };

    public static readonly Params Deutan = new Params
    {
        Matrix1 = new float[,]
        {
            { 0.36198f, 0.86755f, -0.22953f },
            { 0.26099f, 0.64512f, 0.09389f },
            { -0.01975f, 0.02686f, 0.99289f }
        },
        Matrix2 = new float[,]
        {
            { 0.37009f, 0.88540f, -0.25549f },
            { 0.25767f, 0.63782f, 0.10451f },
            { -0.01950f, 0.02741f, 0.99209f }
        },
        SeparationPlane = new Vector3(-0.00293f, -0.00645f, 0.00938f)
    };

    public static readonly Params Tritan = new Params
    {
        Matrix1 = new float[,]
        {
            { 1.01354f, 0.14268f, -0.15622f },
            { -0.01181f, 0.87561f, 0.13619f },
            { 0.07707f, 0.81208f, 0.11085f }
        },
        Matrix2 = new float[,]
        {
            { 0.93337f, 0.19999f, -0.13336f },
            { 0.05809f, 0.82565f, 0.11626f },
            { -0.37923f, 1.13825f, 0.24098f }
        },
        SeparationPlane = new Vector3(0.03960f, -0.02831f, -0.01129f)
    };
}*/
