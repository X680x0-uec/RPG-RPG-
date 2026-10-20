using UnityEngine;
using TMPro;

public class Result : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("UIの参照")]
    public GameObject resultPanel; // リザルト画面のパネル
    void Start()
    {
        // ゲーム開始時はリザルト画面を隠しておく
        resultPanel.SetActive(false);
    }

    // 敵が倒されたときに呼ばれるメゾット
    public void ShowResult()
    {
        // パネルを表示する
        resultPanel.SetActive(true);
    }

    // "NEXT"ボタンが押されたときの処理
    public void OnClickNextButton()
    {
        Debug.Log("新しい敵が現れた！");
        resultPanel.SetActive(false);
    }
}
