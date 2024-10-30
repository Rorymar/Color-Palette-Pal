using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToolBackOut : MonoBehaviour
{
    //Return the User back to the Navigational Menu Scene
    public void BackButton(){
        SceneManager.LoadScene("NavMenu");
    }
}
