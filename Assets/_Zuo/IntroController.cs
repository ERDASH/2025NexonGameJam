using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class IntroController : MonoBehaviour
{
    private void Start()
    {
        Application.targetFrameRate = 120;
    }
    private void Update()
    {
        if (Input.anyKeyDown)
        {
            ButtonSkip_Click();
        }
    }

    public void ButtonSkip_Click()
    {
        SceneManager.LoadScene("Scene_Title");
        Debug.Log("clicked");
    }
}