using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Player_Status : MonoBehaviour
{
    [SerializeField] int max_HP; // Playerの最大HP[cite: 3]
    [SerializeField] int current_HP; // Playerの現在のHP[cite: 3]
    [SerializeField] Slider HP_bar; // HPバーの参照[cite: 3]
    [SerializeField] TextMeshProUGUI HP_text; // HPテキストの参照[cite: 3]
    [SerializeField] TextMeshProUGUI gameOverText; // GAMEOVERテキストの参照[cite: 3]

    [SerializeField] AudioSource gameOverBGM; //[cite: 3]

    [Header("★効果音（SE）の設定")]
    [SerializeField] AudioSource seAudioSource; // 効果音を鳴らすスピーカー
    [SerializeField] AudioClip attackMidSE;     // アタック2個：中攻撃音
    [SerializeField] AudioClip attackBigSE;     // アタック3個：大攻撃音
    [SerializeField] AudioClip blockMidSE;      // ブロック2個：中防御音
    [SerializeField] AudioClip blockBigSE;      // ブロック3個：大防御音
    [SerializeField] AudioClip healMidSE;       // ヒール2個：中回復音
    [SerializeField] AudioClip healBigSE;       // ヒール3個：大回復音
    [SerializeField] AudioClip allOneSE;        // ★今回追加：全種類1個ずつ揃った時の音

    [SerializeField] int attack_power; // Playerの攻撃力[cite: 3]
    [SerializeField] int attack_bonus; // attackが三つそろった時のボーナス値[cite: 3]
    [SerializeField] float heal_bonus; // healが三つそろった時のボーナス値[cite: 3]
    [SerializeField] Enemy_Status enemy; // Enemy_Statusスクリプトの参照[cite: 3]
    [SerializeField] Slot_Results slot_results; // Slot_Resultsスクリプトの参照[cite: 3]
    [SerializeField] Level_up Level_UP; // Level_upスクリプトの参照[cite: 3]
    public static float speed_bonus = 1.0f; // スピードボーナスの初期値[cite: 3]
    int attack_count; // attackが出た数[cite: 3]
    int block_count; // blockが出た数[cite: 3]
    int heal_count; // healが出た数[cite: 3]
    int attack_level_count; //[cite: 3]
    int block_level_count; //[cite: 3]
    int heal_level_count; //[cite: 3]

    void Start()
    {
        current_HP = max_HP; // PlayerのHPを最大HPに設定[cite: 3]
        
        if (gameOverText != null) //[cite: 3]
        {
            gameOverText.gameObject.SetActive(false); //[cite: 3]
        }

        UpdateHP(); // HPを更新[cite: 3]
        slot_results = FindAnyObjectByType<Slot_Results>(); //[cite: 3]
    }

    void Update()
    {
        
    }

    public void OnAllSlotsStopped()
    {
        attack_level_count = 0; //[cite: 3]
        block_level_count = 0; //[cite: 3]
        heal_level_count = 0; //[cite: 3]
        GetSlotResults(); // 各カウント数の取得[cite: 3]

        // ★今回追加：攻撃・防御・回復が1個ずつ揃った時の効果音
        if (attack_count == 1 && block_count == 1 && heal_count == 1)
        {
            PlaySE(allOneSE);
            Debug.Log("全種類1個ずつ揃った！");
        }

        int Level = Level_up.LevelUP_Ability.Count; //[cite: 3]
        for (int i = 0; i < Level; i++) //[cite: 3]
        {
            int abilityIndex = Level_up.LevelUP_Ability[i]; //[cite: 3]
            switch (abilityIndex) //[cite: 3]
            {
                case 0:
                    attack_level_count++; //[cite: 3]
                    break;
                case 1:
                    block_level_count++; //[cite: 3]
                    break;
                case 2:
                    heal_level_count++; //[cite: 3]
                    break;
            }
        }
        Debug.Log($"attack_level_count: {attack_level_count}, block_level_count: {block_level_count}, heal_level_count: {heal_level_count}"); //[cite: 3]
        if (heal_count > 0) // healの目が出た場合、回復[cite: 3]
        {
            Heal(heal_count); //[cite: 3]
        }
        TakeDamege(block_count, Enemy_Status.Enemy_ATK[Enemy_Status.currentEnemyIndex]); // ダメージ処理[cite: 3]
        
        // 生きている時だけ敵へ攻撃するように設定[cite: 3]
        if (current_HP > 0) //[cite: 3]
        {
            Attack(attack_count); //[cite: 3]
        }
    }

    private void GetSlotResults()
    {
        if (slot_results != null) //[cite: 3]
        {
            attack_count = slot_results.attack_count; //[cite: 3]
            block_count = slot_results.block_count; //[cite: 3]
            heal_count = slot_results.heal_count; //[cite: 3]
        }
    }

    public void Attack(int attack_count)
    {
        if (attack_count == 2)
        {
            PlaySE(attackMidSE);
        }
        else if (attack_count == 3) //[cite: 3]
        {
            PlaySE(attackBigSE);
            attack_count *= attack_bonus; //[cite: 3]
            Debug.Log("attackが三つそろった! 相手に大ダメージ!"); //[cite: 3]
        }

        int total_attack = attack_power * (attack_count + 1) + attack_level_count * 10; //[cite: 3]
        int enemy_damage = (int)(total_attack * speed_bonus); // 攻撃力の計算[cite: 3]
        enemy.TakeDamege(enemy_damage); //[cite: 3]
    }
    
    public void TakeDamege(int block_count, int damage)
    {
        if (block_count == 2)
        {
            PlaySE(blockMidSE);
        }
        else if (block_count == 3) //[cite: 3]
        {
            PlaySE(blockBigSE);
        }

        if (block_count != 3) //[cite: 3]
        {
            int total_block = (int)(block_count * 0.2f - block_level_count * 5); //[cite: 3]
            current_HP -= (int)(damage - damage * total_block * speed_bonus); // ダメージ計算[cite: 3]
        }
        current_HP = Mathf.Clamp(current_HP, 0, max_HP); //[cite: 3]
        UpdateHP(); // HPを更新[cite: 3]

        if (current_HP <= 0) //[cite: 3]
        {
            GameOver(); //[cite: 3]
        }
    }

    private void Heal(int heal_count)
    {
        float total_heal = heal_count * 10f; //[cite: 3]

        if (heal_count == 2)
        {
            PlaySE(healMidSE);
        }
        else if (heal_count == 3) //[cite: 3]
        {
            PlaySE(healBigSE);
            total_heal *= heal_bonus; //[cite: 3]
            Debug.Log("healが三つそろった!"); //[cite: 3]
        }

        int healAmount = (int)(total_heal + heal_level_count * 10); //[cite: 3]
        current_HP += healAmount; // 回復量の計算[cite: 3]
        current_HP = (int)(current_HP * speed_bonus); //[cite: 3]
        current_HP = Mathf.Clamp(current_HP, 0, max_HP); //[cite: 3]
        UpdateHP(); //[cite: 3]
    }

    private void PlaySE(AudioClip clip)
    {
        if (seAudioSource != null && clip != null)
        {
            seAudioSource.PlayOneShot(clip);
        }
    }

    private void UpdateHP()
    {
        HP_bar.value = (float)current_HP / max_HP; //[cite: 3]
        HP_text.text = $"Player: HP {current_HP}"; //[cite: 3]
    }

    private void GameOver()
    {
        Debug.Log("GAME OVER..."); //[cite: 3]
        if (gameOverText != null) //[cite: 3]
        {
            gameOverText.gameObject.SetActive(true); //[cite: 3]
        }
        if (gameOverBGM != null) //[cite: 3]
        {
            gameOverBGM.Play(); //[cite: 3]
        }
    }
}