using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class FinalResult : MonoBehaviour
{
    public GameObject gameClearText; // ゲームクリア時に表示するテキスト
    public GameObject gameOverText; // ゲームオーバー時に表示するテキスト
    public AudioSource gameOverBGM; // ゲームオーバー時に再生するBGM
    void Start()
    {
        if(Enemy_Status.isGameClear)
        {
            gameClearText.SetActive(true);
        }
        else
        {
            gameOverText.SetActive(true);
            gameOverBGM.Play();
        }
    }
}