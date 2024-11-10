using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class ImageProcessor : MonoBehaviour
{
    public RawImage rawImage; // Assign in the Inspector
    private Texture previousTexture;
    private List<Color> colorCounts;
    public Material[] materials; // Array to hold your materials
    public TMP_InputField[] inputFields;

    [SerializeField]
    ColorConverter cc;

    // Start is called before the first frame update
    void Start()
    {
        if (rawImage == null)
        {
            rawImage = GetComponent<RawImage>();
        }
        previousTexture = rawImage.texture;

        //DELETE WHEN CACHE IS SET UP(?)
        ResetColors();
    }

    //Triggers when application quits
    void OnApplicationQuit()
    {
        ResetColors();
    }

    // Update is called once per frame
    void Update()
    {
        if (rawImage.texture != previousTexture)
        {
            previousTexture = rawImage.texture;
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
            ProcessTexture((Texture2D)rawImage.texture);
        }
    }

    //Revert the colors to white.
    private void ResetColors()
    {
        for (int i = 0; i < materials.Length; i++){
            materials[i].color = Color.white;
            inputFields[i].SetTextWithoutNotify(cc.createHexFromColor(materials[i].color));
        }
    }

    //Process the texture of the image
    private void ProcessTexture(Texture2D texture)
    {
        // Read pixel data from the texture
        Color[] pixels = texture.GetPixels();
        Debug.Log("Pixels Retrieved! Analyzing...");
        Color[] quantizedPixels = QuantizeImageColors(pixels, numShades: 16);
        //Analyze the colors for commonalities
        colorCounts = AnalyzeColors(quantizedPixels);
        Debug.Log("Generating a starting Palette...");
        SelectColors(colorCounts,randomizeTimes: 1);
    }

    //Regenerate the Palette, not producing the same colors as before.
    public void RegenColors(){
        Debug.Log("Generating a new Palette...");
        List<Color> banList = new List<Color>();
        for (int i = 0; i < materials.Length; i++)
        {
            banList.Add(materials[i].color);
        }
        if(colorCounts != null){
            SelectColors(colorCounts,banList,randomizeTimes: 1);
        }
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
    private List<Color> AnalyzeColors(Color[] pixels){
                //Create a way of storing the color data
        Dictionary<Color, int> colorCounts = new Dictionary<Color, int>();

        //For each pixel, we analyze its color
        foreach (var pixel in pixels)
        {
            //Eliminate excessive similarity.
            Color simplifiedPixel = new Color(
            Mathf.Round(pixel.r * 255) / 255,
            Mathf.Round(pixel.g * 255) / 255,
            Mathf.Round(pixel.b * 255) / 255
            );

            //Increment counts
            if (colorCounts.ContainsKey(simplifiedPixel))
            {
                colorCounts[simplifiedPixel]++;
            }
            else
            {
                colorCounts[simplifiedPixel] = 1;
            }
        }

        //Sort the colors by their frequency
        return colorCounts.OrderByDescending(kvp => kvp.Value)
                                          .Select(kvp => kvp.Key)
                                          .ToList();
    }

    private void SelectColors(List<Color> sortedColors, List<Color> banList = null, int randomizeTimes = 0, int colorsToGrab = 6){
        
        //Starting threshold and empty list
        float threshold = 0.3f;
        List<Color> selectedColors = new List<Color>();

        //Random the color list.
        for(int i = 0; i < randomizeTimes; i++){
            sortedColors = SlightlyRandomizeList(sortedColors);
        }

        //Run through the colors, getting a list of frequent colors that aren't overly similar
        while(selectedColors.Count < colorsToGrab && threshold > 0){

            //Clear the list
            selectedColors.Clear();

            //Loop through the colors till you have six or run out
            foreach (var color in sortedColors){
                if(banList != null && banList.Contains(color)){
                    continue;
                }
                if(selectedColors.Count < colorsToGrab)
                {
                    //Only add if sufficiently different from the previous color.
                    if(selectedColors.All(c => ColorDifference(c,color) > threshold) )
                    {
                        selectedColors.Add(color);
                        materials[selectedColors.Count-1].color = color;
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
        //Display the colors in the UI
        DisplayColors(selectedColors);
    }

    //Randomizes the list of sorted colors to induce a degree of variance in results
    private List<Color> SlightlyRandomizeList(List<Color> sortedColors, double randomnessFactor = 0.5)
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
    private void DisplayColors(List<Color> selectedColors){
        //Assign the six colors
        Color blank = new Color();
        blank = Color.clear;
        while(selectedColors.Count < materials.Length){
            selectedColors.Add(blank);
        }
        for (int i = 0; i < materials.Length; i++)
        {
            if (i < selectedColors.Count)
            {
                materials[i].color = selectedColors[i];
                inputFields[i].SetTextWithoutNotify(cc.createHexFromColor(materials[i].color));
                //Debug.Log($"Assigned Color: {selectedColors[i]} to Material: {materials[i].name}");
            }
        }
    }

    //Calculate the degree of similarity between two colors.
    private float ColorDifference(Color a, Color b){
        return Mathf.Sqrt(Mathf.Pow(a.r - b.r, 2) + Mathf.Pow(a.g-b.g,2)+Mathf.Pow(a.b-b.b,2));
    }

}
