using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NavManager : MonoBehaviour
{
    //Static Variable for knowing the correct starting state to load
    public static string NavState = "Base";

    //Navigational Groups
    public GameObject BaseNav;   // Navigation for the BaseNav Group
    public GameObject CFNav;  // Navigation for the CFNav Group
    public GameObject CANav;  // Navigation for the CANav Group

    // Start is called before the first frame update
    void Start()
    {
        //Starting State when returning from a Creation Feature
        if(NavState == "CF"){
            SetChildrenActive(BaseNav, false);
            SetChildrenActive(CFNav, true);
            SetChildrenActive(CANav, false);
        }
        //Starting State when returning from an Accessibility Tool
        else if(NavState == "CA"){
            SetChildrenActive(BaseNav, false);
            SetChildrenActive(CFNav, false);
            SetChildrenActive(CANav, true);
        }
         //Starting State when starting the software
        else{
            SetChildrenActive(BaseNav, true);
            SetChildrenActive(CFNav, false);
            SetChildrenActive(CANav, false);
        }
    }

    //Disables the children of a particular UI Element group.
    private void SetChildrenActive(GameObject parent, bool isActive)
    {
        // Enable or disable all child objects
        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(isActive);
        }
    }

    //Nav Menu Scene Swaps
    public void BaseToCF()
    {
        SetChildrenActive(BaseNav, false);
        SetChildrenActive(CFNav, true);
        NavState = "CF";
    }

    public void BaseToCA()
    {
        SetChildrenActive(BaseNav, false);
        SetChildrenActive(CANav, true);
        NavState = "CA";
    }

    //Creation Features Scene Swaps
    public void CFToBase()
    {
        SetChildrenActive(CFNav, false);
        SetChildrenActive(BaseNav, true);
        NavState = "Base";
    }

    //Swaps scene to the Palette From Scratch Tool
    public void CFToPaletteScratch()
    {
        SceneManager.LoadScene("PaletteFromScratch");
    }

    //Swaps scene to the Palette From Image Tool
    public void CFToPaletteImage()
    {
        SceneManager.LoadScene("PaletteFromImage");
    }
    
    //Accessibility Tools Scene Swaps
    public void CAToBase(){
        SetChildrenActive(CANav, false);
        SetChildrenActive(BaseNav, true);
        NavState = "Base";
    }

    //Swaps scene to the Color Blind Toggle
    public void CAToColorBlind(){
        SceneManager.LoadScene("ColorBlindToggle");
    }

    //Swaps scene to the Palette Adjustor
    public void CAToAdjustPalette(){
        SceneManager.LoadScene("AdjustPalette");
    }

    //Swaps scene to the Image Filter Tool
    public void CAToImageFilter(){
        SceneManager.LoadScene("ImageFilter");
    }
}
