using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarkerColorChanger : MonoBehaviour
{

    private bool is_black;
    [SerializeField] private GameObject marker;

    [SerializeField] private List<Image> marker_images;
    // Start is called before the first frame update
    void Start()
    {
        is_black = false;
        changeColOnPosition();
    }

    // Update is called once per frame
    void Update()
    {
        changeColOnPosition();
    }

    void changeColOnPosition()
    {
        if (!is_black && marker.transform.localPosition.y > 70)
        {
            is_black = true;
            for (int i = 0; i < marker_images.Count; i++)
            {
                marker_images[i].color = Color.black;
            }
        } else if (is_black && marker.transform.localPosition.y < 70)
        {
            is_black = false;
            for (int i = 0; i < marker_images.Count; i++)
            {
                marker_images[i].color = Color.white;
            }
        }
    }
}
