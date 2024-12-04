using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Script for adjusting palette for color blindness
public class ColorBlindnessAdjuster : MonoBehaviour
{
    // all initializations for sliders (backgrounds and text) as well as palette panels and hex code fields
    [SerializeField]
    private TMP_InputField[] hexInputs;
    [SerializeField]
    private Image[] colorPanels;
    [SerializeField]
    private Slider protanSlider, deutanSlider, tritanSlider, achromatopsiaSlider;
    [SerializeField]
    private Image protanBackground, deutanBackground, tritanBackground, achromatopsiaBackground;
    [SerializeField]
    private TMP_Text protanOnText, protanOffText;
    [SerializeField]
    private TMP_Text deutanOnText, deutanOffText;
    [SerializeField]
    private TMP_Text tritanOnText, tritanOffText;
    [SerializeField]
    private TMP_Text achromatopsiaOnText, achromatopsiaOffText;
    [SerializeField]
    private ColorConverter cc;

    // helper initializations: tracks user-entered colors (nullable for empty fields) and bool flag for if toggle is on
    private Color?[] userColors;
    private bool isFilterActive = false;

    private void Start()
    {
        // initialize user colors
        userColors = new Color?[hexInputs.Length];

        // initialize hex input fields and color panels
        for (int i = 0; i < hexInputs.Length; i++)
        {
            // start with empty fields and white panels
            hexInputs[i].text = "#FFFFFF";
            colorPanels[i].color = Color.white;
            colorPanels[i].material.color = Color.white;
        }

        // initialize slider visuals
        UpdateSliderVisuals(protanSlider, protanBackground, protanOnText, protanOffText);
        UpdateSliderVisuals(deutanSlider, deutanBackground, deutanOnText, deutanOffText);
        UpdateSliderVisuals(tritanSlider, tritanBackground, tritanOnText, tritanOffText);
        UpdateSliderVisuals(achromatopsiaSlider, achromatopsiaBackground, achromatopsiaOnText, achromatopsiaOffText);
    }

    private void Update()
    {
        // only process user input when no toggles are active
        if (!isFilterActive)
        {
            for (int i = 0; i < hexInputs.Length; i++)
            {
                string hexText = hexInputs[i].text.Trim();

                // if input is cleared by user, reset that color
                if (string.IsNullOrEmpty(hexText))
                {
                    ResetColorPanel(i);
                    continue;
                }

                // update color, throw invalid hex error in console if not valid
                try
                {
                    Color newColor = cc.CreateColorFromHex(hexText);
                    if (!userColors[i].HasValue || newColor != userColors[i].Value)
                    {
                        UpdateColorPanel(i, newColor);
                    }
                }
                catch
                {
                    Debug.LogWarning($"Invalid hex code entered in input {i}: {hexText}");
                }
            }
        }
    }

    // updating what happens when slider changes, visually and functionally
    public void OnProtanSliderChanged(float value)
    {
        HandleSliderChange(protanSlider, "Protan", value);
        UpdateSliderVisuals(protanSlider, protanBackground, protanOnText, protanOffText);
    }

    public void OnDeutanSliderChanged(float value)
    {
        HandleSliderChange(deutanSlider, "Deutan", value);
        UpdateSliderVisuals(deutanSlider, deutanBackground, deutanOnText, deutanOffText);
    }

    public void OnTritanSliderChanged(float value)
    {
        HandleSliderChange(tritanSlider, "Tritan", value);
        UpdateSliderVisuals(tritanSlider, tritanBackground, tritanOnText, tritanOffText);
    }

    public void OnAchromatopsiaSliderChanged(float value)
    {
        HandleSliderChange(achromatopsiaSlider, "Achromatopsia", value);
        UpdateSliderVisuals(achromatopsiaSlider, achromatopsiaBackground, achromatopsiaOnText, achromatopsiaOffText);
    }

    // handling what happens when a slider changes functionally
    //   resets all other toggles, adds flag that toggle is active, and make adjustments based off user colors
    //   only allow user input in hex codes when all toggles are off
    private void HandleSliderChange(Slider activeSlider, string type, float value)
    {
        if (value == 1)
        {
            protanSlider.value = activeSlider == protanSlider ? 1 : 0;
            deutanSlider.value = activeSlider == deutanSlider ? 1 : 0;
            tritanSlider.value = activeSlider == tritanSlider ? 1 : 0;
            achromatopsiaSlider.value = activeSlider == achromatopsiaSlider ? 1 : 0;

            isFilterActive = true;

            RevertToUserColors();
            ApplyColorBlindnessFilter(type);
        }
        else if (AllSlidersOff())
        {
            isFilterActive = false;
            RevertToUserColors();
        }
    }

    private bool AllSlidersOff()
    {
        return protanSlider.value == 0 && deutanSlider.value == 0 &&
               tritanSlider.value == 0 && achromatopsiaSlider.value == 0;
    }

    private void ApplyColorBlindnessFilter(string type)
    {
        for (int i = 0; i < colorPanels.Length; i++)
        {
            // apply filter toggles to non-empty hex fields
            if (userColors[i].HasValue)
            {
                // use user colors, don't replace with filtered color but display it with hex code
                Color adjustedColor = AdjustForColorBlindness(userColors[i].Value, type);
                colorPanels[i].color = adjustedColor;
                hexInputs[i].SetTextWithoutNotify(cc.CreateHexFromColor(adjustedColor));
            }
        }
    }

    private void RevertToUserColors()
    {
        for (int i = 0; i < colorPanels.Length; i++)
        {
            if (userColors[i].HasValue)
            {
                // display original color/hex from user
                colorPanels[i].color = userColors[i].Value;
                hexInputs[i].SetTextWithoutNotify(cc.CreateHexFromColor(userColors[i].Value));
            }
            else
            {
                // reset empty fields
                ResetColorPanel(i);
            }
        }
    }

    // handling what happens when a slider changes visually - green/red colors and on/off text visibility
    private void UpdateSliderVisuals(Slider slider, Image background, TMP_Text onText, TMP_Text offText)
    {
        if (slider.value == 1)
        {
            background.color = Color.green;
            onText.gameObject.SetActive(true);
            offText.gameObject.SetActive(false);
        }
        else
        {
            background.color = Color.red;
            onText.gameObject.SetActive(false);
            offText.gameObject.SetActive(true);
        }
    }

    // selecting color blindness type
    private Color AdjustForColorBlindness(Color color, string type)
    {
        Vector3 rgb = new Vector3(color.r, color.g, color.b);
        Vector3 adjustedRgb;

        switch (type)
        {
            case "Protan":
                adjustedRgb = ApplyBrettel(rgb, Brettel1997.Protan);
                break;
            case "Deutan":
                adjustedRgb = ApplyBrettel(rgb, Brettel1997.Deutan);
                break;
            case "Tritan":
                adjustedRgb = ApplyBrettel(rgb, Brettel1997.Tritan);
                break;
            case "Achromatopsia":
                adjustedRgb = ApplyAchromatopsia(rgb);
                break;
            default:
                adjustedRgb = rgb;
                break;
        }

        return ClampColor(new Color(adjustedRgb.x, adjustedRgb.y, adjustedRgb.z, color.a));
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

    // adapted from https://mmuratarat.github.io/2020-05-13/rgb_to_grayscale_formulas
    private Vector3 ApplyAchromatopsia(Vector3 rgb)
    {
        float gray = rgb.x * 0.299f + rgb.y * 0.587f + rgb.z * 0.114f;
        return new Vector3(gray, gray, gray);
    }

    private void UpdateColorPanel(int index, Color newColor)
    {
        colorPanels[index].color = newColor;
        userColors[index] = newColor;
    }

    private void ResetColorPanel(int index)
    {
        colorPanels[index].color = Color.white;
        userColors[index] = null;
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
public static class Brettel1997
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
}