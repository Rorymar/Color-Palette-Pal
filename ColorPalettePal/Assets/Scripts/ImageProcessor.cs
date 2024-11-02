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
    public Material[] materials; // Array to hold your materials
    public TMP_InputField[] inputFields;
    
    // Start is called before the first frame update
    void Start()
    {
        if (rawImage == null)
        {
            rawImage = GetComponent<RawImage>();
        }
        previousTexture = rawImage.texture;

        //DELETE WHEN CACHE IS SET UP(?)
        for (int i = 0; i < materials.Length; i++){
            materials[i].color = Color.white;
            inputFields[i].SetTextWithoutNotify(createHexFromColor(materials[i].color));
        }
    }

    void OnApplicationQuit()
    {
        for (int i = 0; i < materials.Length; i++){
            materials[i].color = Color.white;
            inputFields[i].SetTextWithoutNotify(createHexFromColor(materials[i].color));
        }
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

    private void OnTextureChanged()
    {
        if (rawImage.texture != null)
        {
            // Call your method to process the texture
            Debug.Log("Image loaded! Beginning processing...");
            ProcessTexture((Texture2D)rawImage.texture);
        }
    }

    private void ProcessTexture(Texture2D texture)
    {
        // Read pixel data from the texture
        Color[] pixels = texture.GetPixels();
        Debug.Log("Pixels Retrieved! Analyzing...");
        //Analyze the colors for commonalities
        AnalyzeColors(pixels);
    }

    private void AnalyzeColors(Color[] pixels, int colorsToGrab = 6)
    {
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
        var sortedColors = colorCounts.OrderByDescending(kvp => kvp.Value)
                                          .Select(kvp => kvp.Key)
                                          .ToList();
        
        //Starting threshold of color difference.
        float threshold = 0.3f;

        //The list that will hold the colors
        List<Color> selectedColors = new List<Color>();

        //Run through the colors, getting a list of six frequent colors that aren't overly similar
        while(selectedColors.Count < colorsToGrab && threshold > 0){

            //Clear the list
            selectedColors.Clear();

            //Loop through the colors till you have six or run out
            foreach (var color in sortedColors){
                if(selectedColors.Count < colorsToGrab)
                {
                    //Only add if sufficiently different from the previous color.
                    if(selectedColors.All(c => ColorDifference(c,color) > threshold))
                    {
                        selectedColors.Add(color);
                        materials[selectedColors.Count-1].color = color;
                    }
                }
                else
                {
                    break;
                }
            }
            //If failed to complete a palette, lower the threshold and try again.
            threshold -= 0.05f;
        }

        //Assign the six colors
        for (int i = 0; i < materials.Length; i++)
        {
            if (i < colorsToGrab)
            {
                materials[i].color = selectedColors[i];
                inputFields[i].SetTextWithoutNotify(createHexFromColor(materials[i].color));
                Debug.Log($"Assigned Color: {selectedColors[i]} to Material: {materials[i].name}");
            }
        }
    }

    //Get hex code of a color
    private string createHexFromColor(Color color)
    {
        int red = (int)(color.r * 255);
        int green = (int)(color.g * 255);
        int blue = (int)(color.b * 255);
        string hex = "#" + red.ToString("X2") + green.ToString("X2")
                     + blue.ToString("X2");
        return hex.ToUpper();
    }

    //Calculate the degree of similarity between two colors.
    private float ColorDifference(Color a, Color b){
        return Mathf.Sqrt(Mathf.Pow(a.r - b.r, 2) + Mathf.Pow(a.g-b.g,2)+Mathf.Pow(a.b-b.b,2));
    }

}
