using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Essentially dupe of the color toggle with adjustments for the sliders to be the blindness instead of just the rgb options

public class ColorBlindnessAdjuster : MonoBehaviour
{
    public TMP_InputField[] hexInputs;
    public Image[] colorPanels;
    public Slider protanSlider;
    public Slider deutanSlider;
    public Slider tritanSlider;
    public Slider achromatopsiaSlider;

    private Color[] originalColors;
    private string[] originalHexCodes = { "#F1E8B8", "#1F271B", "#19647E", "#28AFB0", "#BF0667", "#78FFBB" };

    void Start()
    {
        // Initialize the original colors based on the starting hex codes
        originalColors = new Color[originalHexCodes.Length];
        for (int i = 0; i < originalHexCodes.Length; i++)
        {
            ColorUtility.TryParseHtmlString(originalHexCodes[i], out originalColors[i]);
            hexInputs[i].text = originalHexCodes[i];
            colorPanels[i].color = originalColors[i];
        }

        // Attach listeners to sliders
        protanSlider.onValueChanged.AddListener(delegate { OnSliderChanged(protanSlider, "Protan"); });
        deutanSlider.onValueChanged.AddListener(delegate { OnSliderChanged(deutanSlider, "Deutan"); });
        tritanSlider.onValueChanged.AddListener(delegate { OnSliderChanged(tritanSlider, "Tritan"); });
        achromatopsiaSlider.onValueChanged.AddListener(delegate { OnSliderChanged(achromatopsiaSlider, "Achromatopsia"); });
    }

    private void OnSliderChanged(Slider activeSlider, string type)
    {
        // Ensure only one slider can be active at a time
        if (activeSlider.value == 1)
        {
            // Set all other sliders to 0
            protanSlider.value = (activeSlider == protanSlider) ? 1 : 0;
            deutanSlider.value = (activeSlider == deutanSlider) ? 1 : 0;
            tritanSlider.value = (activeSlider == tritanSlider) ? 1 : 0;
            achromatopsiaSlider.value = (activeSlider == achromatopsiaSlider) ? 1 : 0;

            // Apply color blindness filter based on selected type
            ApplyColorBlindnessFilter(type);
        }
        else
        {
            // If no slider is set to 1, revert to original colors
            if (protanSlider.value == 0 && deutanSlider.value == 0 && tritanSlider.value == 0 && achromatopsiaSlider.value == 0)
            {
                RevertToOriginalColors();
            }
        }
    }

    private void ApplyColorBlindnessFilter(string type)
    {
        for (int i = 0; i < colorPanels.Length; i++)
        {
            Color adjustedColor = AdjustForColorBlindness(originalColors[i], type);
            colorPanels[i].color = adjustedColor;
            hexInputs[i].SetTextWithoutNotify(ColorToHex(adjustedColor));
        }
    }

    private void RevertToOriginalColors()
    {
        for (int i = 0; i < colorPanels.Length; i++)
        {
            colorPanels[i].color = originalColors[i];
            hexInputs[i].SetTextWithoutNotify(originalHexCodes[i]);
        }
    }

    // Helper function to convert a color to hex string
    private string ColorToHex(Color color)
    {
        int red = Mathf.RoundToInt(color.r * 255);
        int green = Mathf.RoundToInt(color.g * 255);
        int blue = Mathf.RoundToInt(color.b * 255);
        return $"#{red:X2}{green:X2}{blue:X2}";
    }

    // Adjust a color based on the selected type of color blindness
    private Color AdjustForColorBlindness(Color color, string type)
    {
        Vector3 rgb = new Vector3(color.r, color.g, color.b);
        Vector3 adjustedRgb;

        switch (type)
        {
            case "Protan":
                adjustedRgb = ApplyProtan(rgb);
                break;
            case "Deutan":
                adjustedRgb = ApplyDeutan(rgb);
                break;
            case "Tritan":
                adjustedRgb = ApplyTritan(rgb);
                break;
            case "Achromatopsia":
                adjustedRgb = ApplyAchromatopsia(rgb);
                break;
            default:
                adjustedRgb = rgb;
                break;
        }

        return new Color(adjustedRgb.x, adjustedRgb.y, adjustedRgb.z, color.a);
    }

    // Color blindness adjustment functions
    private Vector3 ApplyProtan(Vector3 rgb)
    {
        return new Vector3(
            0.56667f * rgb.x + 0.43333f * rgb.y,
            0.55833f * rgb.x + 0.44167f * rgb.z,
            rgb.z);
    }

    private Vector3 ApplyDeutan(Vector3 rgb)
    {
        return new Vector3(
            0.625f * rgb.x + 0.375f * rgb.y,
            0.7f * rgb.y + 0.3f * rgb.z,
            rgb.z);
    }

    private Vector3 ApplyTritan(Vector3 rgb)
    {
        return new Vector3(
            rgb.x,
            0.95f * rgb.y + 0.05f * rgb.z,
            0.8f * rgb.z + 0.2f * rgb.y);
    }

    private Vector3 ApplyAchromatopsia(Vector3 rgb)
    {
        float gray = rgb.x * 0.299f + rgb.y * 0.587f + rgb.z * 0.114f;
        return new Vector3(gray, gray, gray);
    }
}
