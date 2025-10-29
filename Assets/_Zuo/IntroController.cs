using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class IntroController : MonoBehaviour
{
    public GameObject transitionPrefab;
    private void Start()
    {
       // Application.targetFrameRate = 120;
      //  Screen.SetResolution(1280, 720, false);
    }
    private void Update()
    {
        /*
        if (Input.anyKeyDown)
        {
            ButtonSkip_Click();
        }*/
    }

    public void ButtonSkip_Click()
    {
        global.mapChange = "Scene_Title";


        if (!string.IsNullOrEmpty(global.mapChange))
        {
            Instantiate(transitionPrefab);
        }
        //SceneManager.LoadScene("Scene_Title");
        Debug.Log("clicked");
    }
}