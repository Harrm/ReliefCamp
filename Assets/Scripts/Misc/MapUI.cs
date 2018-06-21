using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MapUI : MonoBehaviour{
    [SerializeField]
    public CanvasGroup CreateUI;
    [SerializeField]
    public CanvasGroup LoadUI;
    //[SerializeField]
    //public Button CreatButton;
    //[SerializeField]
    //public Button LoadButton;

    private void Start() {
        //OnClickCreate();
    }

    /*public void OnClickCreate() {
        CreateUI.alpha = 1;
        CreateUI.interactable = true;
        CreateUI.blocksRaycasts = true;

        LoadUI.alpha = 0;
        LoadUI.interactable = false;
        LoadUI.blocksRaycasts = false;
    }*/

    /*public void OnClickLoad() {
        CreateUI.alpha = 0;
        CreateUI.interactable = false;
        CreateUI.blocksRaycasts = false;

        LoadUI.alpha = 1;
        LoadUI.interactable = true;
        LoadUI.blocksRaycasts = true;
    }*/

        /*
    public void OnClickNew() {
        Settings.IsLoad = false;
        Settings.LoadPath = "";
        SceneManager.LoadScene("TerrainEditing");
    }
    public void OnClickLoad() {
        Settings.IsLoad = true;
        Settings.LoadPath = "save.xml";
        SceneManager.LoadScene("TerrainEditing");
    }
    */
}

