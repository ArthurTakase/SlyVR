using UnityEngine;

public class TPToScene : MonoBehaviour
{
    public void GoToScene(string sceneName)
    {
        // UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        PlayerMovements.Instance.LoadScene(sceneName);
    }

    public void GoToScene(int sceneIndex)
    {
        PlayerMovements.Instance.LoadScene(sceneIndex);
    }
}