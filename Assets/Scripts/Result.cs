using UnityEngine;
using TMPro;

public class Result : MonoBehaviour
{
    [Header("UIの参照")]
    [SerializeField] public GameObject ButtonParent;
    public GameObject resultPanel;
    [SerializeField] private Enemy_Status enemyStatus;

    void Start()
    {
        resultPanel.SetActive(false);

        // 自動でEnemy_Statusを探してつなぐ
        if (enemyStatus == null)
        {
            enemyStatus = FindAnyObjectByType<Enemy_Status>();
        }
    }

    public void ShowResult()
    {
        resultPanel.SetActive(true);
    }

    // "NEXT"ボタンが押されたときの処理
    public void OnClickNextButton()
    {
        SoundManager.Instance?.PlaySE(SoundManager.Instance.seNextButton); // ★追加: NEXTボタン音
        Debug.Log("新しい敵が現れた！");
        resultPanel.SetActive(false);
        ButtonParent.SetActive(true);

        // 次の敵を出現させる
        if (enemyStatus != null)
        {
            enemyStatus.SpawnNextEnemy();
        }
    }
}