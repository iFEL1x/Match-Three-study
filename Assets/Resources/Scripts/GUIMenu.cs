using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GUIMenu : MonoBehaviour
{
    public void OnClickPlay()
    {
        SceneManager.LoadScene(1);
    }

    public void OnCliclExit()
    {
        Application.Quit();
    }
}
