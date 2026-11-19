using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Player_Status : MonoBehaviour
{
    [SerializeField] int max_HP; // Playerの最大HP
    [SerializeField] int current_HP; // Playerの現在のHP
    [SerializeField] Slider HP_bar; // HPバーの参照
    [SerializeField] TextMeshProUGUI HP_text; // HPテキストの参照
    [SerializeField] TextMeshProUGUI gameOverText; // GAMEOVERテキストの参照
    [SerializeField] public GameObject continuePanel; // ContinuePanelの参照
    [SerializeField] public SceneChanger SceneChanger;
    [SerializeField] AudioSource gameOverBGM;
    [SerializeField] public AudioSource gameWhileBGM; // ゲーム中のBGMの参照
    [SerializeField] public AudioSource lastBossBGM; // ラスボスBGMの参照
    [SerializeField] int attack_power; // Playerの攻撃力
    [SerializeField] int attack_bonus; // attackが三つそろった時のボーナス値
    [SerializeField] float heal_bonus; // healが三つそろった時のボーナス値
    [SerializeField] Enemy_Status enemy; // Enemy_Statusスクリプトの参照
    [SerializeField] Slot_Restart slot_restart; // Slot_Restartスクリプトの参照
    [SerializeField] Slot_Results slot_results; // Slot_Resultsスクリプトの参照
    [SerializeField] Level_up Level_UP; // Level_upスクリプトの参照
    [SerializeField] private GameObject defaultButtonParent; // 操作ボタンの親（保険用）

    [Header("デメリット絵柄（呪い）1つあたりの自傷ダメージ")]
    [SerializeField] private int curseDamage = 15;

    public static float speed_bonus = 1.0f; // スピードボーナスの初期値
    int attack_count; // attackが出た数
    int block_count;  // blockが出た数
    int heal_count;   // healが出た数
    int curse_count;  // デメリット絵柄が出た数

    [Header("効果音の間隔（秒）")]
    [SerializeField] private float seInterval = 0.4f; // 音が重ならないための待機時間

    void Start()
    {
        current_HP = max_HP; // PlayerのHPを最大HPに設定
        speed_bonus = 1.0f;  // リトライ時の速度ボーナス引き継ぎ防止

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }

        if (SceneChanger == null)
        {
            SceneChanger = FindAnyObjectByType<SceneChanger>();
        }

        UpdateHP();
        slot_results = FindAnyObjectByType<Slot_Results>();
    }

    public void OnAllSlotsStopped(GameObject buttonParent = null)
    {
        StartCoroutine(BattleSequence(buttonParent));
    }

    // 音が重ならないように時間差で順番に処理するコルーチン
    private IEnumerator BattleSequence(GameObject buttonParent)
    {
        if (buttonParent == null)
        {
            buttonParent = defaultButtonParent;
        }

        GetSlotResults(); // 各カウント数の取得

        // ★追加：ターン経過＆絵柄揃いスコア増減（呪い揃いの減少も含む）
        ScoreManager.Instance?.OnTurnPassed();
        ScoreManager.Instance?.AddMatchScore(attack_count, block_count, heal_count, curse_count);

        // 1. 3つ目のリール停止音と重ならないよう少し待つ
        yield return new WaitForSeconds(seInterval);

        // 2. 揃い数に応じた効果音を再生
        PlayMatchSE();

        // 3. 効果音に合わせて少し待ってからバトルの数値処理を実行
        yield return new WaitForSeconds(seInterval);

        if (heal_count > 0) // healの目が出た場合、回復
        {
            Heal(heal_count);
        }

        TakeDamege(block_count, Enemy_Status.Enemy_ATK[Enemy_Status.currentEnemyIndex]); // 敵からのダメージ処理

        if (curse_count > 0) // デメリット絵柄が出た場合、自傷ダメージ
        {
            TakeCurseDamage(curse_count);
        }

        // 生きている時だけ敵へ攻撃する
        if (current_HP > 0)
        {
            if (attack_count > 0)
            {
                Attack(attack_count);
            }

            // 敵を倒していなければ操作ボタンを再表示する
            if (enemy != null && !enemy.IsDead() && !Enemy_Status.islevelup)
            {
                if (buttonParent != null)
                {
                    buttonParent.SetActive(true);
                }
            }
        }
    }

    // 揃い方に応じた効果音を再生するメソッド
    private void PlayMatchSE()
    {
        if (attack_count >= 3)
        {
            SoundManager.Instance?.PlaySE(SoundManager.Instance.seAttackHigh);
        }
        else if (attack_count == 2)
        {
            SoundManager.Instance?.PlaySE(SoundManager.Instance.seAttackMid);
        }
        else if (block_count >= 3)
        {
            SoundManager.Instance?.PlaySE(SoundManager.Instance.seBlockHigh);
        }
        else if (block_count == 2)
        {
            SoundManager.Instance?.PlaySE(SoundManager.Instance.seBlockMid);
        }
        else if (heal_count >= 3)
        {
            SoundManager.Instance?.PlaySE(SoundManager.Instance.seHealHigh);
        }
        else if (heal_count == 2)
        {
            SoundManager.Instance?.PlaySE(SoundManager.Instance.seHealMid);
        }
        else if (attack_count == 1 && block_count == 1 && heal_count == 1)
        {
            SoundManager.Instance?.PlaySE(SoundManager.Instance.seMatchAllOne);
        }
    }

    private void GetSlotResults()
    {
        if (slot_results != null)
        {
            attack_count = slot_results.attack_count;
            block_count = slot_results.block_count;
            heal_count = slot_results.heal_count;
            curse_count = slot_results.curse_count;
        }
    }

    public void Attack(int count)
    {
        if (count >= 3)
        {
            count *= attack_bonus;
            Debug.Log("attackが三つそろった! 相手に大ダメージ!");
        }
        int total_attack = attack_power * (count + 1) + Level_up.attackBonusTotal;
        int enemy_damage = Mathf.RoundToInt(total_attack * speed_bonus * Level_up.GetEffectMultiplier());
        enemy.TakeDamege(enemy_damage);
    }

    public void TakeDamege(int count, int damage)
    {
        if (count < 3)
        {
            float blockRate = Mathf.Clamp01(count * 0.2f * speed_bonus * Level_up.GetEffectMultiplier());
            int reducedDamage = Mathf.RoundToInt(damage * (1f - blockRate)) - Level_up.blockBonusTotal;
            current_HP -= Mathf.Max(0, reducedDamage);
        }
        current_HP = Mathf.Clamp(current_HP, 0, max_HP);
        UpdateHP();

        if (current_HP <= 0)
        {
            ContinuePanel();
        }
    }

    // デメリット絵柄（呪い）による防御無視ダメージ
    private void TakeCurseDamage(int count)
    {
        if (current_HP <= 0) return;

        int totalCurseDamage = count * curseDamage;
        Debug.Log($"呪いの絵柄が{count}個出現！ {totalCurseDamage}のダメージ！");
        current_HP -= totalCurseDamage;
        current_HP = Mathf.Clamp(current_HP, 0, max_HP);
        UpdateHP();

        if (current_HP <= 0)
        {
            ContinuePanel();
        }
    }

    private void Heal(int count)
    {
        float total_heal = count * 10f;
        if (count >= 3)
        {
            float bonus = (heal_bonus > 0f) ? heal_bonus : 2.0f;
            total_heal *= bonus;
            Debug.Log("healが三つそろった!");
        }
        int healAmount = Mathf.RoundToInt((total_heal + Level_up.healBonusTotal) * speed_bonus * Level_up.GetEffectMultiplier());
        current_HP += healAmount;
        current_HP = Mathf.Clamp(current_HP, 0, max_HP);
        UpdateHP();
    }

    private void UpdateHP()
    {
        HP_bar.value = (float)current_HP / max_HP;
        HP_text.text = $"Player: HP {current_HP}";
    }
    private void ContinuePanel()
    {
        if (continuePanel != null)
        {
            continuePanel.SetActive(true);
            gameWhileBGM.Stop(); 
            lastBossBGM.Stop();
        }
    }
    public void ContinueGameButton()
    {
        if (continuePanel != null)
        {
            continuePanel.SetActive(false);
        }
        if (Enemy_Status.currentEnemyIndex == 4)
        {
            // ラスボス戦の場合、ラスボスBGMを再生
            if (lastBossBGM != null)
            {
                lastBossBGM.Play();
            }
        }
        else
        {
            // 通常のゲーム中BGMを再生
            if (gameWhileBGM != null)
            {
                gameWhileBGM.Play();
            }
        }
        ScoreManager.continueCount = ScoreManager.continueCount + 1; // コンティニュー回数をカウント
        slot_restart.RecreateButtons(); // ボタンを再生成
        current_HP = max_HP; // HPを最大値に戻す
        UpdateHP();
    }
    public void GameOver()
    {
        if (gameWhileBGM != null)
        {
            gameWhileBGM.Stop(); // ゲーム中のBGMを停止
        }
        if (lastBossBGM != null)
        {
            lastBossBGM.Stop(); // ★修正：ゲームオーバー時はラスボスBGMも停止
        }
        Debug.Log("GAME OVER...");
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true); // テキストを表示する
        }
        // ★追加：ゲームオーバー時点のスコアをランキングに登録
        ScoreManager.Instance?.FinalizeGameScore(false);

        if (SceneChanger != null)
        {
            SceneChanger.SceneChange("ResultScene"); // シーンに戻る
        }
    }
}