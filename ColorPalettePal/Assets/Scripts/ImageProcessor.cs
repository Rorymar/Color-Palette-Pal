using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImageProcessor : MonoBehaviour
{
    [SerializeField]
    RawImage rawImage; // Assign in the Inspector
    
    private Texture previousTexture;
    private List<string> colorCounts;
    
    [SerializeField]
    Material[] materials; // Array to hold your materials
    
    [SerializeField]
    TMP_InputField[] inputFields;

    [SerializeField]
    GameObject[] buttonObjects;
    
    private Button[] buttons;

    [SerializeField]
    ColorConverter cc;

    [SerializeField]
    PaletteHistory hh;

    //Create the variables for the banlists.
    private HashSet <string> blackBanList;
    private HashSet <string> whiteBanList;
    private HashSet <string> grayBanList;

    private bool textureSet = false;

    // Start is called before the first frame update
    void Start()
    {
        if (rawImage == null)
        {
            rawImage = GetComponent<RawImage>();
        }
        previousTexture = rawImage.texture;

        buttons = new Button[buttonObjects.Length];
        for(int i = 0; i < buttonObjects.Length; i++)
        {
            buttons[i] = buttonObjects[i].GetComponent<Button>();
            buttons[i].interactable = false;
        }

        //Create the Ban Lists
        blackBanList = new HashSet<string>(MakeBanList(0,60));
        whiteBanList = new HashSet<string>(MakeBanList(195,255));
        grayBanList = new HashSet<string>(MakeGrayBanList(50,200));

        //DELETE WHEN CACHE IS SET UP(?)
        ResetColors();
    }

    //Create a Non-Gray Ban List
    private List<string> MakeBanList(int minValue, int maxValue)
    {
        
        List<string> banList = new List<string>();

        //Find all values within the range and tolerance and add them to the list.
        for(int r = minValue; r <= maxValue; r++)
        {
            for(int g = minValue; g <= maxValue; g++)
            {
                for(int b = minValue; b <= maxValue; b++)
                {
                    string hexColor = $"#{r:X2}{g:X2}{b:X2}";
                    banList.Add(hexColor);
                }
            }
        }

        return banList;
    }

    //Create a Gray Ban List
    private List<string> MakeGrayBanList(int minValue, int maxValue, int tolerance = 15)
    {
        
        List<string> banList = new List<string>();

        //Find all values within the range and tolerance and add them to the list.
        for(int r = minValue; r <= maxValue; r++)
        {
            for(int g = minValue; g <= maxValue; g++)
            {
                for(int b = minValue; b <= maxValue; b++)
                {
                    if(Math.Abs(r-g) <= tolerance && Math.Abs(g-b) <= tolerance && Math.Abs(r-b) <= tolerance)
                    {
                        string hexColor = $"#{r:X2}{g:X2}{b:X2}";
                        banList.Add(hexColor);
                    }
                }
            }
        }

        return banList;
    }

    //Triggers when application quits
    void OnApplicationQuit()
    {
        for(int i = 0; i < buttonObjects.Length; i++)
        {
            buttons[i].interactable = false;
        }
        ResetColors();
    }

    // Update is called once per frame
    void Update()
    {
        if (rawImage.texture != previousTexture)
        {
            OnTextureChanged();
        }
    }

    //Detect if the texture has changes
    private void OnTextureChanged()
    {
        if (rawImage.texture != null)
        {
            //Call method to process texture
            Debug.Log("Image loaded! Beginning processing...");

            if(textureSet == false){
                for(int i = 0; i < buttonObjects.Length; i++)
                {
                    buttons[i].interactable = true;
                }
                textureSet = true;
            }
            
            previousTexture = rawImage.texture;
            ProcessTexture((Texture2D)rawImage.texture);
        }
    }

    //Revert the colors to white.
    private void ResetColors()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = Color.white;
            inputFields[i].SetTextWithoutNotify(cc.CreateHexFromColor(materials[i].color));
        }
    }

    //Process the texture of the image
    private void ProcessTexture(Texture2D texture)
    {
        // Read pixel data from the texture
        Color[] pixels = texture.GetPixels();
        Debug.Log("Pixels Retrieved! Analyzing...");
        Color[] quantizedPixels = QuantizeImageColors(pixels, numShades: 32);
        //Analyze the colors for commonalities
        colorCounts = AnalyzeColors(quantizedPixels);
        Debug.Log("Generating a starting Palette...");
        SelectColors(colorCounts,randomizeTimes: 1);
    }

    //Regenerate the Palette, not producing the same colors as before.
    public void RegenColors(){
        Debug.Log("Generating a new Palette...");
        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
        HashSet<string> banList = new HashSet<string>();
        for (int i = 0; i < inputFields.Length; i++)
        {
            if(inputFields[i].text != "Hex Code"){
                banList.Add(inputFields[i].text);
            }
        }
        if(colorCounts != null)
        {
            SelectColors(colorCounts,banList,randomizeTimes: 1);
        }
        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = true;
        }
    }

    //Removes all blacks from palette
    public void RemoveColorGroupBlack()
    {
        Debug.Log("Removing blacks...");
        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
        List<string> preselects = BuildPreselects(blackBanList);
        SelectColors(colorCounts,blackBanList,randomizeTimes: 1, preselects);
        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = true;
            if(i == 1)
            {
                buttons[i].interactable = false;
            }
        }
    }

    //Remove all whites from palette
    public void RemoveColorGroupWhite()
    {
        Debug.Log("Removing whites...");
        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
        List<string> preselects = BuildPreselects(whiteBanList);
        SelectColors(colorCounts,whiteBanList,randomizeTimes: 1,preselects);
        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = true;
            if(i == 3)
            {
                buttons[i].interactable = false;
            }
        }
    }

    //Remove all grays from palette
    public void RemoveColorGroupGray()
    {
        Debug.Log("Removing greys...");
        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
        List<string> preselects = BuildPreselects(grayBanList);
        SelectColors(colorCounts,grayBanList,randomizeTimes: 1, preselects);
        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = true;
            if(i == 2)
            {
                buttons[i].interactable = false;
            }
        }
    }

    //Builds the list of colors that don't need to be removed from the current line-up.
    public List<string> BuildPreselects(HashSet<string> banList)
    {
        List<string> preselects = new List<string>();
        foreach(Material material in materials)
        {
            string matHex = cc.CreateHexFromColor(material.color);
            if(CheckViability(matHex,banList) && material.color != Color.clear)
            {
                preselects.Add(matHex);
            }
        }
        return preselects;
    }

    //Simplifies the color information in a color
    private Color QuantizeColor(Color color, int numShades = 16)
    {
        //Reduce color precision by mapping each channel to a smaller set of values
        float stepSize = 1.0f / numShades;

        float r = Mathf.Round(color.r / stepSize) * stepSize;
        float g = Mathf.Round(color.g / stepSize) * stepSize;
        float b = Mathf.Round(color.b / stepSize) * stepSize;

        return new Color(r, g, b);
    }

    //Simplifies the color information in the image.
    private Color[] QuantizeImageColors(Color[] pixels, int numShades = 16)
    {
        List<Color> quantizedColors = new List<Color>();

        foreach (var pixel in pixels)
        {
            //Quantize each pixel color
            Color quantizedColor = QuantizeColor(pixel, numShades);
            quantizedColors.Add(quantizedColor);
        }

        return quantizedColors.ToArray();
    }

    //Analyze the color information in the image uploaded by the user.
    private List<string> AnalyzeColors(Color[] pixels)
    {
                //Create a way of storing the color data
        Dictionary<string, int> colorCounts = new Dictionary<string, int>();

        //For each pixel, we analyze its color
        foreach (var pixel in pixels)
        {
            //Eliminate excessive similarity.
            Color simplifiedPixel = new Color(
            Mathf.Round(pixel.r * 255) / 255,
            Mathf.Round(pixel.g * 255) / 255,
            Mathf.Round(pixel.b * 255) / 255
            );

            string simplifiedHex = cc.CreateHexFromColor(simplifiedPixel);

            //Increment counts
            if (colorCounts.ContainsKey(simplifiedHex))
            {
                colorCounts[simplifiedHex]++;
            }
            else
            {
                colorCounts[simplifiedHex] = 1;
            }
        }

        //Sort the colors by their frequency
        return colorCounts.OrderByDescending(kvp => kvp.Value)
                                          .Select(kvp => kvp.Key)
                                          .ToList();
    }

    //Selects the colors for the palette
    private void SelectColors(List<string> sortedColors, HashSet<string> banList = null, int randomizeTimes = 0, List<string> preselects = null)
    {
        
        //Starting threshold and empty list
        float threshold = 0.2f;
        int colorsToGrab = 6;
        List<string> selectedColors = new List<string>();

        //Random the color list.
        for(int i = 0; i < randomizeTimes; i++)
        {
            sortedColors = SlightlyRandomizeList(sortedColors);
        }

        //Run through the colors, getting a list of frequent colors that aren't overly similar
        while(selectedColors.Count < colorsToGrab && threshold > 0)
        {

            //Clear the list
            selectedColors.Clear();
            Debug.Log("Reset selected colors");
            if(preselects != null)
            {
                selectedColors.AddRange(preselects);
            }

            //Loop through the colors till you have six or run out
            foreach (var color in sortedColors)
            {
                if(selectedColors.Contains(color))
                {
                    continue;
                }
                else if(banList != null && !CheckViability(color,banList))
                {
                    continue;
                }
                if(selectedColors.Count < colorsToGrab)
                {
                    //Only add if sufficiently different from the previous color.
                    if(selectedColors.All(c => ColorDifference(cc.CreateColorFromHex(c),cc.CreateColorFromHex(color)) > threshold) )
                    {
                        selectedColors.Add(color);
                        Debug.Log($"Added Color: {color} to SelectedColors.");
                    }
                }
                else
                {
                    continue;
                }
            }
            //If failed to complete a palette, lower the threshold and try again.
            threshold -= 0.05f;
        }
        DisplayColors(selectedColors);
    }

    //Verifies if the current color is in the banned list of colors
    private bool CheckViability(string hexCode, HashSet<string> BanList)
    {
        return !BanList.Contains(hexCode);
    }

    //Calculate the degree of similarity between two colors.
    private float ColorDifference(Color a, Color b)
    {
        return Mathf.Pow(a.r - b.r, 2) + Mathf.Pow(a.g - b.g, 2) + Mathf.Pow(a.b - b.b, 2);
    }

    //Randomizes the list of sorted colors to induce a degree of variance in results
    private List<string> SlightlyRandomizeList(List<string> sortedColors, double randomnessFactor = 0.5)
    {
        //Creates a new random object
        var random = new System.Random();
        
        //Iterates over the colors and potentially swaps them if random aligns.
        for (int i = 0; i < sortedColors.Count - 1; i++)
        {
            if (random.NextDouble() < randomnessFactor)
            {
                // Swap the current item with the next item
                var temp = sortedColors[i];
                sortedColors[i] = sortedColors[i + 1];
                sortedColors[i + 1] = temp;
            }
        }
        
        return sortedColors;
    }

    //Display the palette of colors. If there aren't six to display, make the material clear.
    private void DisplayColors(List<string> selectedColors)
    {
        //For the removals, order of extisting colors should be maintained
        List<string> reorderedColors = new List<string>(new string[materials.Length]);
        List<bool> used = new List<bool>(new bool[selectedColors.Count]);

        for(int i = 0; i < materials.Length; i++)
        {
            bool matchFound = false;
            for(int j = 0; j < selectedColors.Count; j++)
            {
                if(!used[j] && materials[i].color == cc.CreateColorFromHex(selectedColors[j]))
                {
                    reorderedColors[i] = selectedColors[j];
                    used[j] = true;
                    matchFound = true;
                    break;
                }
            }
            if(!matchFound)
            {
                reorderedColors[i] = null;
            }
        }

        int colorIndex = 0;
        for(int i = 0; i < reorderedColors.Count; i++)
        {
            if(reorderedColors[i] == null){
                while(colorIndex < selectedColors.Count && used[colorIndex])
                {
                    colorIndex++;
                }
                if(colorIndex < selectedColors.Count)
                {
                    reorderedColors[i] = selectedColors[colorIndex];
                    used[colorIndex] = true;
                }
            }
        }

        //Sets the colors
        for (int i = 0; i < materials.Length; i++)
        {
            inputFields[i].SetTextWithoutNotify("Hex Code");
            if (i < reorderedColors.Count  && reorderedColors[i] != null)
            {
                materials[i].color = cc.CreateColorFromHex(reorderedColors[i]);
                inputFields[i].SetTextWithoutNotify(reorderedColors[i]);
            }
            else
            {
                materials[i].color = Color.clear;
            }
            Debug.Log($"Assigned Color: {reorderedColors[i]} to Material: {materials[i].name}");
        }
        hh.newEntry();
    }

}
