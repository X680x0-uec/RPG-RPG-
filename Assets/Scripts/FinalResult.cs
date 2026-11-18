using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class FinalResult : MonoBehaviour
{
    public GameObject gameClearText;  // ゲームクリア時に表示するテキスト
    public GameObject gameOverText;   // ゲームオーバー時に表示するテキスト
    public GameObject continueButton; // ★追加：コンティニューボタンの参照
    public AudioSource gameOverBGM;   // ゲームオーバー時に再生するBGM
    public AudioSource gameClearBGM;   // ゲームクリア時に再生するBGM

    void Start()
    {
        if (Enemy_Status.isGameClear)
        {
            if (gameClearText != null) gameClearText.SetActive(true);
            if (continueButton != null) continueButton.SetActive(false); // クリア時は非表示
            if (gameClearBGM != null) gameClearBGM.Play();
        }
        else
        {
            if (gameOverText != null) gameOverText.SetActive(true);
            if (continueButton != null) continueButton.SetActive(true);  // ゲームオーバー時は表示
            if (gameOverBGM != null) gameOverBGM.Play();
        }
    }
}