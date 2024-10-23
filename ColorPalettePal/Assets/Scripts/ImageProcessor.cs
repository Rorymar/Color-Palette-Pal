using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ImageProcessor : MonoBehaviour
{
    public RawImage rawImage; // Assign in the Inspector
    private Texture previousTexture;
    public Material[] materials; // Array to hold your materials
    
    // Start is called before the first frame update
    void Start()
    {
        if (rawImage == null)
        {
            rawImage = GetComponent<RawImage>();
        }
        previousTexture = rawImage.texture;

        //DELETE WHEN CACHE IS SET UP(?)
        foreach (var material in materials)
        {
            material.color = Color.white;
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

    private void AnalyzeColors(Color[] pixels)
    {
        //Create a way of storing the color data
        Dictionary<Color, int> colorCounts = new Dictionary<Color, int>();

        //For each pixel, we analyze its color
        foreach (var pixel in pixels)
        {
            //Eliminate excessive similarity.
            //WILL NEED TO STRENGTHEN OR EXPAND ON
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

        //List the top five colors
        var topColors = colorCounts.OrderByDescending(c => c.Value)
                               .Take(6)
                               .Select(c => c.Key)
                               .ToList();
        for (int i = 0; i < materials.Length; i++)
        {
            if (i < topColors.Count)
            {
                materials[i].color = topColors[i];
                Debug.Log($"Assigned Color: {topColors[i]} to Material: {materials[i].name}");
            }
        }
    }
}
