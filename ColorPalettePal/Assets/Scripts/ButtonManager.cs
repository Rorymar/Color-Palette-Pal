using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    // Start is called before the first frame update
    private ApplyColor palette;
    void Start()
    {
        GameObject palette_object = GameObject.Find("PaletteManager");
        palette = (ApplyColor) palette_object.GetComponent(typeof(ApplyColor));
        print(palette.color1_material.color);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void randomizePalette()
    {
        palette.randomizeUnlockedColors();
    }

    public void unlockAllColors()
    {
        palette.setLocksImageToUnlocked();
    }
}
