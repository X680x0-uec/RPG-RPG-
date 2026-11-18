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

    [SerializeField] AudioSource gameOverBGM;
    [SerializeField] public AudioSource gameWhileBGM; // ゲーム中のBGMの参照
    [SerializeField] int attack_power; // Playerの攻撃力
    [SerializeField] int attack_bonus; // attackが三つそろった時のボーナス値
    [SerializeField] float heal_bonus; // healが三つそろった時のボーナス値
    [SerializeField] Enemy_Status enemy; // Enemy_Statusスクリプトの参照
    [SerializeField] Slot_Results slot_results; // Slot_Resultsスクリプトの参照
    [SerializeField] Level_up Level_UP; // Level_upスクリプトの参照
    [SerializeField] private GameObject defaultButtonParent; // 操作ボタンの親（保険用）

    public static float speed_bonus = 1.0f; // スピードボーナスの初期値
    int attack_count; // attackが出た数
    int block_count; // blockが出た数
    int heal_count; // healが出た数
    int attack_level_count;
    int block_level_count;
    int heal_level_count;

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

        UpdateHP();
        slot_results = FindAnyObjectByType<Slot_Results>();
    }

    void Update()
    {
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

        attack_level_count = 0;
        block_level_count = 0;
        heal_level_count = 0;
        GetSlotResults(); // 各カウント数の取得

        int Level = Level_up.LevelUP_Ability.Count;
        for (int i = 0; i < Level; i++)
        {
            int abilityIndex = Level_up.LevelUP_Ability[i];
            switch (abilityIndex)
            {
                case 0:
                    attack_level_count++;
                    break;
                case 1:
                    block_level_count++;
                    break;
                case 2:
                    heal_level_count++;
                    break;
            }
        }
        Debug.Log($"attack_level_count: {attack_level_count}, block_level_count: {block_level_count}, heal_level_count: {heal_level_count}");

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
        TakeDamege(block_count, Enemy_Status.Enemy_ATK[Enemy_Status.currentEnemyIndex]); // ダメージ処理

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
        if (attack_count == 2)
        {
            SoundManager.Instance?.PlaySE(SoundManager.Instance.seAttackMid);
        }
        else if (attack_count == 3)
        {
            SoundManager.Instance?.PlaySE(SoundManager.Instance.seAttackHigh);
        }
        else if (block_count == 2)
        {
            SoundManager.Instance?.PlaySE(SoundManager.Instance.seBlockMid);
        }
        else if (block_count == 3)
        {
            SoundManager.Instance?.PlaySE(SoundManager.Instance.seBlockHigh);
        }
        else if (heal_count == 2)
        {
            SoundManager.Instance?.PlaySE(SoundManager.Instance.seHealMid);
        }
        else if (heal_count == 3)
        {
            SoundManager.Instance?.PlaySE(SoundManager.Instance.seHealHigh);
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
        }
    }

    public void Attack(int attack_count)
    {
        if (attack_count == 3)
        {
            attack_count *= attack_bonus;
            Debug.Log("attackが三つそろった! 相手に大ダメージ!");
        }
        int total_attack = attack_power * (attack_count + 1) + attack_level_count * 10;
        int enemy_damage = (int)(total_attack * speed_bonus); // 攻撃力の計算
        enemy.TakeDamege(enemy_damage);
    }

    public void TakeDamege(int block_count, int damage)
    {
        if (block_count != 3)
        {
            // 防御1つで20%、2つで40%軽減 ＋ レベルアップ1回につき固定5ダメージ軽減
            float blockRate = Mathf.Clamp01(block_count * 0.2f * speed_bonus);
            int reducedDamage = Mathf.RoundToInt(damage * (1f - blockRate)) - (block_level_count * 5);
            current_HP -= Mathf.Max(0, reducedDamage);
        }
        current_HP = Mathf.Clamp(current_HP, 0, max_HP);
        UpdateHP(); // HPを更新

        if (current_HP <= 0)
        {
            GameOver();
        }
    }

    private void Heal(int heal_count)
    {
        float total_heal = heal_count * 10f;
        if (heal_count == 3)
        {
            float bonus = (heal_bonus > 0f) ? heal_bonus : 2.0f;
            total_heal *= bonus;
            Debug.Log("healが三つそろった!");
        }
        // 現在HP全体ではなく、回復量にだけ speed_bonus を掛ける
        int healAmount = Mathf.RoundToInt((total_heal + heal_level_count * 10) * speed_bonus);
        current_HP += healAmount;
        current_HP = Mathf.Clamp(current_HP, 0, max_HP);
        UpdateHP();
    }

    private void UpdateHP()
    {
        HP_bar.value = (float)current_HP / max_HP;
        HP_text.text = $"Player: HP {current_HP}";
    }

    private void GameOver()
    {
        if (gameWhileBGM != null)
        {
            gameWhileBGM.Stop(); // ゲーム中のBGMを停止
        }
        Debug.Log("GAME OVER...");
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true); // テキストを表示する
        }
        if (gameOverBGM != null)
        {
            gameOverBGM.Play();
        }
    }
}