using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player_Status : MonoBehaviour
{
    [SerializeField] int max_HP; // Playerの最大HP
    [SerializeField] int current_HP; // Playerの現在のHP
    [SerializeField] Slider HP_bar; // HPバーの参照
    [SerializeField] TextMeshProUGUI HP_text; // HPテキストの参照
    [SerializeField] TextMeshProUGUI gameOverText; // ★追加: GAMEOVERテキストの参照

    [SerializeField] int attack_power; // Playerの攻撃力
    [SerializeField] int attack_bonus; // attackが三つそろった時のボーナス値
    [SerializeField] float heal_bonus; // healが三つそろった時のボーナス値
    [SerializeField] Enemy_Status enemy; // Enemy_Statusスクリプトの参照
    [SerializeField] Slot_Results slot_results; // Slot_Resultsスクリプトの参照
    
    int attack_count; // attackが出た数
    int block_count; // blockが出た数
    int heal_count; // healが出た数

    void Start()
    {
        current_HP = max_HP; // PlyerのHPを最大HPに設定
        
        // ★追加: ゲーム開始時はGAMEOVERテキストを非表示にする
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }

        UpdateHP(); // HPを更新
        slot_results = FindAnyObjectByType<Slot_Results>();
    }

    void Update()
    {
        
    }

    public void OnAllSlotsStopped()
    {
        GetSlotResults(); // 各カウント数の取得
        if (heal_count > 0) // healの目が出た場合、回復
        {
            Heal(heal_count);
        }
        TakeDamege(block_count, 10); // ダメージ処理
        
        // ★修正: 生きている時だけ敵へ攻撃するように設定
        if (current_HP > 0)
        {
            Attack(attack_count);
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
        int enemy_damage = attack_power * (attack_count + 1); 
        enemy.TakeDamege(enemy_damage);
    }
    
    public void TakeDamege(int block_count, int damage)
    {
        if (block_count != 3)
        {
            current_HP -= (int)(damage - damage * block_count * 0.2f);
        }
        current_HP = Mathf.Clamp(current_HP, 0, max_HP);
        UpdateHP(); // HPを更新

        // ★追加: HPが0以下になったらゲームオーバー処理を行う
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
            total_heal *= heal_bonus;
            Debug.Log("healが三つそろった!");
        }
        current_HP += (int)total_heal;
        current_HP = Mathf.Clamp(current_HP, 0, max_HP);
        UpdateHP();
    }

    private void UpdateHP()
    {
        HP_bar.value = (float)current_HP / max_HP;
        HP_text.text = $"Player: HP {current_HP}";
    }

    // ★追加: GAMEOVER表示処理
    private void GameOver()
    {
        Debug.Log("GAME OVER...");
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true); // テキストを表示する
        }
    }
}