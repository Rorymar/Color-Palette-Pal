using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarkerColorChanger : MonoBehaviour
{

    private bool isBlack;
    [SerializeField] 
    private GameObject marker;
    [SerializeField] 
    private List<Image> markerImages;
    // Start is called before the first frame update
    void Start()
    {
        isBlack = false;
        ChangeColOnPosition();
    }

    // Update is called once per frame
    void Update()
    {
        ChangeColOnPosition();
    }

    //Change color of the marker if it is high/low enough
    void ChangeColOnPosition()
    {
        if (!isBlack && marker.transform.localPosition.y > 70)
        {
            isBlack = true;
            for (int i = 0; i < markerImages.Count; i++)
            {
                markerImages[i].color = Color.black;
            }
        } else if (isBlack && marker.transform.localPosition.y <= 70)
        {
            isBlack = false;
            for (int i = 0; i < markerImages.Count; i++)
            {
                markerImages[i].color = Color.white;
            }
        }
    }
}
