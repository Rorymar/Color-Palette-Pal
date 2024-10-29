using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public string showTag;

    private Button button;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        //Don't trigger if button doesn't have a place to go
        if(showTag != null){

            //Target a particular tag group and activate its buttons
            GameObject targetGroup = GameObject.FindGameObjectWithTag(showTag);
            foreach (Transform child in targetGroup.transform)
            {
                child.gameObject.SetActive(true);
            }

            //Deactive current buttons
            Transform parentTransform = transform.parent;
            if (parentTransform != null)
            {
                foreach (Transform child in parentTransform)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }

    void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonClick);
    }
}
