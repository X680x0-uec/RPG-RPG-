using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneChanger : MonoBehaviour
{    
    [SerializeField] private SceneManager SceneManager;
    public void SceneChange(string sceneName)
    {
        SceneManager.LoadScene(sceneName);

    }
}