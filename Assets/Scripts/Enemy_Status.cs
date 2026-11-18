using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Enemy_Status : MonoBehaviour
{
    // 5体分のHPと攻撃力（左から1体目、2体目、3体目、4体目、5体目）
    public static List<int> Enemy_HP = new List<int> { 100, 200, 300, 400, 500 };
    public static List<int> Enemy_ATK = new List<int> { 10, 15, 20, 30, 50 };
    public static bool isGameClear = false; // ゲームクリアかどうかのフラグ

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

    public static int currentEnemyIndex = 0; // 現在の敵の番号（0が1体目、4が5体目）
    public static bool islevelup = false;
    public Result resultManager;

    private SpriteRenderer spriteRenderer; // 自分のSpriteRendererを入れる変数

    void Start()
    {
        // 同じオブジェクトに付いているSpriteRendererを自動で見つける
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        // ゲーム開始時に1体目（0番目）にリセット
        currentEnemyIndex = 0;
        islevelup = false;

        if (spinBGM != null)
        {
            spinBGM.Play(); // ゲーム中のBGMを再生
        }

        if (gameClearText != null)
        {
            gameClearText.SetActive(false);
        }

        // 1体目の敵をセットする
        SetupEnemy(currentEnemyIndex);
    }

    // 指定した番号の敵の画像とステータスに切り替える処理
    private void SetupEnemy(int index)
    {
        max_HP = Enemy_HP[index];
        current_HP = max_HP;
        damage = Enemy_ATK[index];

        // SpriteRendererの画像を、今の番号の画像に差し替える
        if (spriteRenderer != null && enemySprites != null && index < enemySprites.Length)
        {
            if (enemySprites[index] != null)
            {
                spriteRenderer.sprite = enemySprites[index];
            }
        }

        UpdateHP();
    }

    // 敵が倒れているかを外部から確認するメソッド
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
            StartCoroutine(DieSequence()); // 揃い音と重ならないよう時間差で撃破処理を実行
        }
    }

    private IEnumerator DieSequence()
    {
        // 揃い効果音と重ならないように0.4秒待つ
        yield return new WaitForSeconds(0.4f);

        // 5体目（最後）を倒したときの処理
        if (currentEnemyIndex >= Enemy_HP.Count - 1)
        {
            SoundManager.Instance?.PlaySE(SoundManager.Instance.seGameClear); // 全クリア音
            // 5体目を倒したら敵の画像だけ非表示にする
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }

            if (gameClearText != null)
            {
                gameClearText.SetActive(true);
            }
            isGameClear = true; // ゲームクリアフラグを立てる
            yield return new WaitForSeconds(3.0f);
            SceneChanger.SceneChange("ResultScene"); // タイトルシーンに戻る

            yield break;
            
        }

        // 1〜4体目を倒したときはリザルト＆レベルアップ画面を出す
        SoundManager.Instance?.PlaySE(SoundManager.Instance.seEnemyDefeat); // 敵撃破音
        islevelup = true;
        if (resultManager != null)
        {
            resultManager.ShowResult();
        }
    }

    // 「NEXT」ボタンが押されたときに次の敵を出す処理
    public void SpawnNextEnemy()
    {
        currentEnemyIndex++;
        if (currentEnemyIndex < Enemy_HP.Count)
        {
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