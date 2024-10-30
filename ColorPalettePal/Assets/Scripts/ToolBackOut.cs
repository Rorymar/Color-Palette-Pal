using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToolBackOut : MonoBehaviour
{
    public void BackButton(){
        SceneManager.LoadScene("UITestScene");
    }
}
