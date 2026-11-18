using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Enemy_Status : MonoBehaviour
{
    public static List<int> Enemy_HP = new List<int> { 100, 200, 300, 400, 500 };
    public static List<int> Enemy_ATK = new List<int> { 10, 15, 20, 30, 50 };
    public static bool isGameClear = false;

    [Header("★ここに1〜5体目の敵の「画像」を5枚登録する")]
    [SerializeField] private Sprite[] enemySprites;

    [Header("★5体すべて倒した時のクリア文字（任意）")]
    [SerializeField] private GameObject gameClearText;

    [Header("既存のUI設定（そのまま使います）")]
    [SerializeField] private int max_HP;
    [SerializeField] private int current_HP;
    [SerializeField] private Slider HP_bar;
    [SerializeField] TextMeshProUGUI HP_text;
    [SerializeField] public int damage;
    [SerializeField] public AudioSource spinBGM;
    [SerializeField] private SceneChanger SceneChanger;
    [SerializeField] private ScoreManager ScoreManager;

    public static int currentEnemyIndex = 0;
    public static bool islevelup = false;
    public Result resultManager;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        if(ScoreManager.isContinueing == false)
        {
            currentEnemyIndex = 0;
        }
        ScoreManager.isContinueing = false; // コンティニュー後の再プレイ時にリセット
        islevelup = false;
        isGameClear = false; // 再プレイ時にクリアフラグをリセット

        if (SceneChanger == null)
        {
            SceneChanger = FindAnyObjectByType<SceneChanger>();
        }

        if (spinBGM != null)
        {
            spinBGM.Play();
        }

        if (gameClearText != null)
        {
            gameClearText.SetActive(false);
        }

        SetupEnemy(currentEnemyIndex);
    }

    private void SetupEnemy(int index)
    {
        max_HP = Enemy_HP[index];
        current_HP = max_HP;
        damage = Enemy_ATK[index];

        if (spriteRenderer != null && enemySprites != null && index < enemySprites.Length)
        {
            if (enemySprites[index] != null)
            {
                spriteRenderer.sprite = enemySprites[index];
            }
        }

        UpdateHP();
    }

    public bool IsDead()
    {
        return current_HP <= 0;
    }

    public void TakeDamege(int enemy_damage)
    {
        if (current_HP <= 0) return;

        current_HP -= enemy_damage;
        current_HP = Mathf.Clamp(current_HP, 0, max_HP);
        UpdateHP();

        if (current_HP <= 0)
        {
            StartCoroutine(DieSequence());
        }
    }

    private IEnumerator DieSequence()
    {
        yield return new WaitForSeconds(0.4f);

        // 撃破までのターン数に応じたボーナスを加算
        ScoreManager.Instance?.AddDefeatTurnBonus(currentEnemyIndex);

        // 5体目（最後）を倒したときの処理
        if (currentEnemyIndex >= Enemy_HP.Count - 1)
        {
            SoundManager.Instance?.PlaySE(SoundManager.Instance.seGameClear);

            // 全クリア時のコンティニューボーナス加算＆ランキング保存
            ScoreManager.Instance?.FinalizeGameScore(true);

            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }

            if (gameClearText != null)
            {
                gameClearText.SetActive(true);
            }
            isGameClear = true;
            yield return new WaitForSeconds(0.5f);
            SceneChanger.SceneChange("ResultScene");

            yield break;
        }

        SoundManager.Instance?.PlaySE(SoundManager.Instance.seEnemyDefeat);
        islevelup = true;
        if (resultManager != null)
        {
            resultManager.ShowResult();
        }
    }

    public void SpawnNextEnemy()
    {
        currentEnemyIndex++;
        if (currentEnemyIndex < Enemy_HP.Count)
        {
            ScoreManager.Instance?.ResetTurnForNextEnemy();
            SetupEnemy(currentEnemyIndex);
        }
    }

    private void UpdateHP()
    {
        if (HP_bar != null && max_HP > 0)
        {
            HP_bar.value = (float)current_HP / max_HP;
        }
        if (HP_text != null)
        {
            HP_text.text = $"Enemy ({currentEnemyIndex + 1}/{Enemy_HP.Count}): HP {current_HP}";
        }
    }
}