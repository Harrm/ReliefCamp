using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    [SerializeField]
    GameObject UI;

    public void OnClick()
    {
        if(UI.GetComponent<Settings>().OnClickStart()) {
            Settings.LoadPath = "";
            SceneManager.LoadScene("TerrainEditing");
        }
    }

    public void OnClickLoad() {
        if(UI.GetComponent<Settings>().OnClickStart()) {
            Settings.LoadPath = "save.savf";
            SceneManager.LoadScene("TerrainEditing");
        }
    }
}