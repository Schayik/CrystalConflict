using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class Canvas : MonoBehaviour {

    [DllImport("__Internal")]
    private static extern void OpenPage(string str);

    public void LoadLevel(int level)
    {
        SceneManager.LoadScene(level);
    }

    public void Exit()
    {
        OpenPage("home.html");
    }

}
