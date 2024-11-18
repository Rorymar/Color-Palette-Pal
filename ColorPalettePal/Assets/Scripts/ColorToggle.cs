using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;

public class ColorToggle : MonoBehaviour
{
    // Serialized Fields for the UI components
    [SerializeField]
    TMP_InputField[] hexInputs;

    [SerializeField]
    Material[] colorPanels;

    [SerializeField]
    Slider redSlider;

    [SerializeField]
    Slider greenSlider;

    [SerializeField]
    Slider blueSlider;

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
            // Ensure we don’t exceed the defaultHexColors array length
            if (i < defaultHexColors.Length)
            {
                // Set default hex input, apply color to panel
                hexInputs[i].text = defaultHexColors[i];
                UpdatePanelColor(i);
            }

            int index = i;
            hexInputs[i].onEndEdit.AddListener(delegate { UpdatePanelColor(index); });
        }

        // Keep up with slider changes
        //redSlider.onValueChanged.AddListener(delegate { UpdateColors(); });
        //greenSlider.onValueChanged.AddListener(delegate { UpdateColors(); });
        //blueSlider.onValueChanged.AddListener(delegate { UpdateColors(); });
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
        print(hexCode);
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

        for (int i = 0; i < colorPanels.Length; i++)
        {
            // Ensure we stay within bounds
            if (i >= originalColors.Length)
                continue;

            // Start with original color
            Color color = originalColors[i];

            // Apply partial color blindness
            float adjustedRed = color.r * redFactor + (1 - redFactor) * GetGrayScale(color);
            float adjustedGreen = color.g * greenFactor + (1 - greenFactor) * GetGrayScale(color);
            float adjustedBlue = color.b * blueFactor + (1 - blueFactor) * GetGrayScale(color);

            // Update color panel with adjusted color
            Color adjustedColor = new Color(adjustedRed, adjustedGreen, adjustedBlue, color.a);
            colorPanels[i].color = adjustedColor;

            // Update hex input field to display the current color
            hexInputs[i].SetTextWithoutNotify(cc.createHexFromColor(adjustedColor));
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
}
