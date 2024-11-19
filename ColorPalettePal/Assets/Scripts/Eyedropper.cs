using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using UnityTest2;

public class Eyedropper : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] 
    private FlexibleColorPicker color_picker;

    private Texture2D screen;
    
    private bool is_enabled;
    void Start()
    {
        is_enabled = false;
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        CustomUtil u = new CustomUtil();
        u.Add(3, 4);
        u.PrintHello();
        print(u.c);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && is_enabled)
        {
            
            is_enabled = false;
        }
    }

    void OnClick()
    {
        is_enabled = true;
    }
}
