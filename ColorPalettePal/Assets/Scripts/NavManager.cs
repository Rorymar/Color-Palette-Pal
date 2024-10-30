using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NavManager : MonoBehaviour
{
    public static string NavState = "Base";
    public GameObject BaseNav;   // Navigation for the BaseNav Group
    public GameObject CFNav;  // Navigation for the CFNav Group
    public GameObject CANav;  // Navigation for the CANav Group

    // Start is called before the first frame update
    void Start()
    {
        if(NavState == "CF"){
            SetChildrenActive(BaseNav, false);
            SetChildrenActive(CFNav, true);
            SetChildrenActive(CANav, false);
        }
        else if(NavState == "CA"){
            SetChildrenActive(BaseNav, false);
            SetChildrenActive(CFNav, false);
            SetChildrenActive(CANav, true);
        }
        else{
            SetChildrenActive(BaseNav, true);
            SetChildrenActive(CFNav, false);
            SetChildrenActive(CANav, false);
        }
    }

    private void SetChildrenActive(GameObject parent, bool isActive)
    {
        // Enable or disable all child objects
        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(isActive);
        }
    }

    //BASE
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

    //CF
    public void CFToBase()
    {
        SetChildrenActive(CFNav, false);
        SetChildrenActive(BaseNav, true);
        NavState = "Base";
    }

    public void CFToPaletteScratch()
    {
        SceneManager.LoadScene("testScene");
    }

    public void CFToPaletteImage()
    {
        ;
    }
    
    //CA
    public void CAToBase(){
        SetChildrenActive(CANav, false);
        SetChildrenActive(BaseNav, true);
        NavState = "Base";
    }

    public void CAToColorBlind(){
        ;
    }

    public void CAToAdjustPalette(){
        ;
    }

    public void CAToImageFilter(){
        ;
    }
}
