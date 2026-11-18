using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // 通常のシーン切り替え
    public void SceneChange(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // 「最初から遊ぶ」「タイトルへ戻る」ボタン用（スコアとコンティニュー回数をリセット）
    public void StartNewGame(string sceneName)
    {
        ScoreManager.ResetForNewGame();
        SceneManager.LoadScene(sceneName);
    }

    // 「コンティニュー」ボタン用（コンティニュー回数を+1してバトル再開）
    public void ContinueGame(string sceneName)
    {
        ScoreManager.RegisterContinue();
        SceneManager.LoadScene(sceneName);
    }
}