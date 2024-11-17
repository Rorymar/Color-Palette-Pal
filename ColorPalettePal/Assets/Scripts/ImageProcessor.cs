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
    public RawImage rawImage; // Assign in the Inspector
    private Texture previousTexture;
    private List<Color> colorCounts;
    public Material[] materials; // Array to hold your materials
    public TMP_InputField[] inputFields;

    [SerializeField]
    GameObject[] buttonObjects;
    private Button[] buttons;

    [SerializeField]
    ColorConverter cc;

    //Create the variables for the banlists.
    List<string> blackBanList;
    List<string> whiteBanList;
    List<string> grayBanList;

    // Start is called before the first frame update
    void Start()
    {
        if (rawImage == null)
        {
            rawImage = GetComponent<RawImage>();
        }
        previousTexture = rawImage.texture;

        buttons = new Button[buttonObjects.Length];
        for(int i = 0; i < buttonObjects.Length; i++){
            buttons[i] = buttonObjects[i].GetComponent<Button>();
            buttons[i].interactable = false;
        }
        /*
        //Add when save implemented
        if (saveButton == null){
            saveButton = saveButtonObj.GetComponent<Button>();
        }
        saveButton.interactable = false;
        */

        //Create the Black Ban List
        blackBanList = new List<string>();
        for(int r = 0; r <= 30; r++){
            for(int g = 0; g <= 30; g++){
                for(int b = 0; b <= 30; b++){
                    string hexColor = $"#{r:X2}{g:X2}{b:X2}";
                    blackBanList.Add(hexColor);
                }
            }
        }

        //Create the White Ban List
        whiteBanList = new List<string>();
        for(int r = 225; r <= 255; r++){
            for(int g = 225; g <= 255; g++){
                for(int b = 225; b <= 255; b++){
                    string hexColor = $"#{r:X2}{g:X2}{b:X2}";
                    whiteBanList.Add(hexColor);
                }
            }
        }

        //Create the Gray Ban List
        grayBanList = new List<string>();
        int start = 125;
        int end = 185;
        for(int r = start; r <= end; r++){
            for(int g = r-30; g <= r+30; g++){
                for(int b = r-30; b <= r+30; b++){
                    if(g >= 0 && g <= 255 && b >= 0 && b <= 255){
                        if(Mathf.Abs(r-g) <= 30 && Mathf.Abs(g-b) <= 30 && Mathf.Abs(r - b) <= 30){
                            string hexColor = "#"+r.ToString("X2")+g.ToString("X2")+b.ToString("X2");
                            grayBanList.Add(hexColor);
                        }
                    }
                }
            }
        }

        //DELETE WHEN CACHE IS SET UP(?)
        ResetColors();
    }

    //Triggers when application quits
    void OnApplicationQuit()
    {
        for(int i = 0; i < buttonObjects.Length; i++){
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

            for(int i = 0; i < buttonObjects.Length; i++){
                buttons[i] = buttonObjects[i].GetComponent<Button>();
                buttons[i].interactable = true;
            }
            
            previousTexture = rawImage.texture;
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
        List<string> banList = new List<string>();
        for (int i = 0; i < materials.Length; i++)
        {
            banList.Add(cc.createHexFromColor(materials[i].color));
        }
        if(colorCounts != null){
            SelectColors(colorCounts,banList,randomizeTimes: 1);
        }
    }

    //Removes all blacks from palette
    public void RemoveColorGroupBlack(){
        List<Color> preselects = buildPreselects(blackBanList);
        SelectColors(colorCounts,blackBanList,randomizeTimes: 1, preselects);
    }

    //Remove all whites from palette
    public void RemoveColorGroupWhite(){
        List<Color> preselects = buildPreselects(whiteBanList);
        SelectColors(colorCounts,whiteBanList,randomizeTimes: 1,preselects);
    }

    //Remove all grays from palette
    public void RemoveColorGroupGray(){
        List<Color> preselects = buildPreselects(grayBanList);
        SelectColors(colorCounts,grayBanList,randomizeTimes: 1, preselects);
    }

    //Builds the list of colors that don't need to be removed from the current line-up.
    public List<Color> buildPreselects(List<string> banList){
        List<Color> preselects = new List<Color>();
        foreach(Material material in materials){
            if(checkViability(cc.createHexFromColor(material.color),banList)){
                preselects.Add(material.color);
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

    //Selects the colors for the palette
    private void SelectColors(List<Color> sortedColors, List<string> banList = null, int randomizeTimes = 0, List<Color> preselects = null){
        
        //Starting threshold and empty list
        float threshold = 0.3f;
        int colorsToGrab = 6;
        List<Color> selectedColors = new List<Color>();

        //Random the color list.
        for(int i = 0; i < randomizeTimes; i++){
            sortedColors = SlightlyRandomizeList(sortedColors);
        }

        //Run through the colors, getting a list of frequent colors that aren't overly similar
        while(selectedColors.Count < colorsToGrab && threshold > 0){

            //Clear the list
            selectedColors.Clear();
            if(preselects != null){
                selectedColors.AddRange(preselects);
            }

            //Loop through the colors till you have six or run out
            foreach (var color in sortedColors){
                if(banList != null && !checkViability(cc.createHexFromColor(color),banList)){
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
        DisplayColors(selectedColors);
    }

    //Verifies if the current color is in the banned list of colors
    private bool checkViability(string hexCode, List<string> BanList){
        foreach(string otherHex in BanList){
            if(hexCode.Equals(otherHex,StringComparison.OrdinalIgnoreCase)){
                return false;
            }
        }
        return true;
    }

    //Calculate the degree of similarity between two colors.
    private float ColorDifference(Color a, Color b){
        return Mathf.Sqrt(Mathf.Pow(a.r - b.r, 2) + Mathf.Pow(a.g-b.g,2)+Mathf.Pow(a.b-b.b,2));
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

        //For the removals, order of extisting colors should be maintained
        List<Color> reorderedColors = new List<Color>();
        List<bool> used = new List<bool>(new bool[selectedColors.Count]);

        for(int i = 0; i < materials.Length; i++){
            bool matchFound = false;
            for(int j = 0; j < selectedColors.Count; j++){
                if(!used[j] && materials[i].color == selectedColors[j]){
                    reorderedColors.Add(selectedColors[j]);
                    used[j] = true;
                    matchFound = true;
                    break;
                }
            }
            if(!matchFound){
                reorderedColors.Add(blank);
            }
        }
        for(int i = 0; i < selectedColors.Count; i++){
            if(!used[i]){
                for(int j = 0; j < reorderedColors.Count; j++){
                    if(reorderedColors[j] == blank){
                        reorderedColors[j] = selectedColors[i];
                    }
                }
            }
        }

        selectedColors = reorderedColors;

        //Sets the colors
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

}
