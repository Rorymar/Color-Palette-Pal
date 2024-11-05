using UnityEngine;
using UnityEngine.UI;

// adapted from https://github.com/DaltonLens/libDaltonLens/blob/master/libDaltonLens.c

public class ImageFilter : MonoBehaviour
{
    public RawImage rawImage; // Original image input
    public RawImage filteredImageDisplay; // Display for the processed image
    private Texture2D previousTexture;

    // Enum for color blindness types
    public enum ColorBlindType
    {
        Protanopia,
        Deuteranopia,
        Tritanopia,
        Achromatopsia,
        None
    }

    private ColorBlindType currentType = ColorBlindType.None; // Default type

    void Start()
    {
        // Check if rawImage is assigned; if not, try to find it on the GameObject
        if (rawImage == null)
        {
            rawImage = GetComponent<RawImage>();
            if (rawImage == null)
            {
                Debug.LogError("RawImage component is not assigned and cannot be found on the GameObject.");
                return;
            }
        }

        // Check if filteredImageDisplay is assigned
        
        if (filteredImageDisplay == null)
        {
            Debug.LogError("FilteredImageDisplay is not assigned in the Inspector.");
            return;
        }

        // Set previous texture if rawImage has a texture
        if (rawImage.texture != null)
        {
            previousTexture = rawImage.texture as Texture2D;
        }
    }

    void Update()
    {
        // Check if rawImage and its texture are not null and if the texture has changed
        if (rawImage != null && rawImage.texture != null)
        {
            if(previousTexture != rawImage.texture){
                previousTexture = rawImage.texture as Texture2D;
                filteredImageDisplay.texture = rawImage.texture;
            }
            if(filteredImageDisplay.texture == null){
                filteredImageDisplay.texture = rawImage.texture;
            }
        }
    }

    private void ProcessTexture(Texture2D texture)
    {
        // Define transformation matrices for each type of color blindness
        float[,] protanMatrix = new float[,]
        {
            { 0.14980f, 1.19548f, -0.34528f },
            { 0.10764f, 0.84864f, 0.04372f },
            { 0.00384f, -0.00540f, 1.00156f }
        };

        float[,] deutanMatrix = new float[,]
        {
            { 0.36477f, 0.86381f, -0.22858f },
            { 0.26294f, 0.64245f, 0.09462f },
            { -0.02006f, 0.02728f, 0.99278f }
        };

        float[,] tritanMatrix = new float[,]
        {
            { 1.01277f, 0.13548f, -0.14826f },
            { -0.01243f, 0.86812f, 0.14431f },
            { 0.07589f, 0.80500f, 0.11911f }
        };

        float[,] achromatopsiaMatrix = new float[,]
        {
            { 0.299f, 0.587f, 0.114f },
            { 0.299f, 0.587f, 0.114f },
            { 0.299f, 0.587f, 0.114f }
        };

        // Select the matrix based on the current type
        float[,] selectedMatrix = currentType switch
        {
            ColorBlindType.Protanopia => protanMatrix,
            ColorBlindType.Deuteranopia => deutanMatrix,
            ColorBlindType.Tritanopia => tritanMatrix,
            ColorBlindType.Achromatopsia => achromatopsiaMatrix,
            _ => protanMatrix // Default to protan if unspecified
        };

        // Create a new texture with the same dimensions as the original
        Texture2D modifiedTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                // Get the original color of each pixel
                Color originalColor = texture.GetPixel(x, y);

                // Apply the selected color blindness transformation
                Vector3 rgb = new Vector3(originalColor.r, originalColor.g, originalColor.b);
                Vector3 transformedRgb = new Vector3(
                    selectedMatrix[0, 0] * rgb.x + selectedMatrix[0, 1] * rgb.y + selectedMatrix[0, 2] * rgb.z,
                    selectedMatrix[1, 0] * rgb.x + selectedMatrix[1, 1] * rgb.y + selectedMatrix[1, 2] * rgb.z,
                    selectedMatrix[2, 0] * rgb.x + selectedMatrix[2, 1] * rgb.y + selectedMatrix[2, 2] * rgb.z
                );

                Color modifiedColor = new Color(
                    Mathf.Clamp01(transformedRgb.x),
                    Mathf.Clamp01(transformedRgb.y),
                    Mathf.Clamp01(transformedRgb.z),
                    originalColor.a
                );

                // Set the modified pixel in the new texture
                modifiedTexture.SetPixel(x, y, modifiedColor);
            }
        }

        // Apply the changes to the new texture
        modifiedTexture.Apply();

        // Display the modified image in the UI
        filteredImageDisplay.texture = modifiedTexture;
        Debug.Log($"Image processing complete for {currentType}. Displaying modified image.");
    }

    public void resetImage(){
        filteredImageDisplay.texture = previousTexture;
        currentType = ColorBlindType.None;
        Debug.Log($"Reset image display.");
    }

    // Method to change the color blindness type dynamically
    public void SetColorBlindType(string type)
    {
        currentType = type switch
        {
            "Protanopia" => ColorBlindType.Protanopia,
            "Deuteranopia" => ColorBlindType.Deuteranopia,
            "Tritanopia" => ColorBlindType.Tritanopia,
            "Achromatopsia" => ColorBlindType.Achromatopsia
        };
        if (rawImage != null && rawImage.texture != null)
        {
            ProcessTexture(previousTexture); // Re-process the image with the new type
        }
    }
}
