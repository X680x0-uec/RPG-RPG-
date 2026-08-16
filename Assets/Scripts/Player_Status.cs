using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player_Status : MonoBehaviour
{
    [SerializeField] int max_HP;//Playerの最大HP。[SerializeField]をつけることでInspectorから値を変えられるようになる
    [SerializeField] int current_HP;//Playerの現在のHP
    [SerializeField] Slider HP_bar;//HPバーの参照
    [SerializeField] TextMeshProUGUI HP_text;//HPテキストの参照
    [SerializeField] int attack_power;//Playerの攻撃力
    [SerializeField] int attack_bonus;//attackが三つそろった時のボーナス値
    [SerializeField] float heal_bonus;//healが三つそろった時のボーナス値
    [SerializeField] Enemy_Status enemy;//Enemy_Statusスクリプトの参照
    [SerializeField] Slot_Results slot_results;//Slot_Resultsスクリプトの参照
    int attack_count;//attackが出た数
    int block_count;//blockが出た数
    int heal_count;//healが出た数

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        current_HP=max_HP;//PlyerのHPを最大HPに設定
        UpdateHP();//HPを更新
        slot_results = FindAnyObjectByType<Slot_Results>();//Slot_Resultsスクリプトから各マス目の数を持ってくる
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnAllSlotsStopped()
    {
        GetSlotResults();//各カウント数の取得
        if (heal_count > 0)//healの目が出た場合、回復
        {
            Heal(heal_count);
        }
        TakeDamege(block_count, 10);//ダメージ処理
        Attack(attack_count);//敵への攻撃処理
    }
    private void GetSlotResults()//各count数を取得する関数
    {
        if (slot_results != null)
        {
            attack_count = slot_results.attack_count;
            block_count = slot_results.block_count;
            heal_count = slot_results.heal_count;
        }
    }
    public void Attack(int attack_count)//敵への攻撃
    {
        if(attack_count == 3)
        {
            attack_count *= attack_bonus; //attackが三つそろった時、攻撃力UP
            Debug.Log("attackが三つそろった! 相手に大ダメージ!");
        }
        //attackcountが0の時にも相手に攻撃するようにattackcountを+1
        int enemy_damage = attack_power * (attack_count+=1); 
        enemy.TakeDamege(enemy_damage);//敵のダメージ処理関数を呼び出す
    }
    
    public void TakeDamege(int block_count, int damage)//敵からの攻撃
    {
        if(block_count != 3)
        {
            current_HP -= (int)(damage - damage * block_count*0.2f);//blockの出た目の数に応じて受けるダメージを減らす
        }
        current_HP = Mathf.Clamp(current_HP, 0, max_HP);//HPが0～100の範囲を超えないようにする
        UpdateHP();//HPを更新
    }
    private void Heal(int heal_count)//回復
    {
        float total_heal = heal_count * 10f; //intからfloatに変換
        if(heal_count == 3)
        {
            total_heal *= heal_bonus;//healが三つそろったとき回復力UP
            Debug.Log("healが三つそろった!");
        }
        current_HP += (int)total_heal;//healの出た目の数に応じてHPを回復
        current_HP = Mathf.Clamp(current_HP, 0, max_HP);//HPが0～100の範囲を超えないようにする
        UpdateHP();//HPを更新
    }
    private void UpdateHP()
    {
        HP_bar.value = (float)current_HP / max_HP;//HPバーを現在のHPに更新
        HP_text.text = $"Player: HP {current_HP}"; //HPテキストを現在のHPに更新
    }
}
