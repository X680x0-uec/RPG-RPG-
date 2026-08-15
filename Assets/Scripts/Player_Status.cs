using UnityEngine;
using UnityEngine.UI;

public class Player_Status : MonoBehaviour
{
    [SerializeField] private int max_HP;//Playerの最大HP。[SerializeField]をつけることでInspectorから値を変えられるようになる
    [SerializeField] private int current_HP;//Playerの現在のHP
    [SerializeField] private Slider HP_bar;//HPバーの参照
    Slot_Results slot_results;
    int attack_count;
    int block_count;
    int heal_count;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        current_HP=max_HP;//PlyerのHPを最大HPに設定
        UpdateHPBar();//HPバーを更新
        slot_results = GetComponent<Slot_Results>();//Slot_Resultsスクリプトから各マス目の数を持ってくる
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnAllSlotsStopped()
    {
        // 1. 最新の出目（カウント）を取り直す
        GetSlotResults();

        // 2. 回復処理（healの目が出ている場合）
        if (heal_count > 0)
        {
            Heal(heal_count);
        }

        // 3. 敵からのダメージ処理を実行
        TakeDamege(block_count, 10); // 例として基本ダメージ10

        // 4. 攻撃処理（必要に応じて敵の処理などを呼び出す）
        Attack(attack_count);
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
    public void Attack(int attack_count)//敵への攻撃
    {
        int attack=10*attack_count;
    }
    
    public void TakeDamege(int block_count, int damage=10)//敵からの攻撃
    {
        current_HP-=(int)(damage - damage * block_count*0.2f);//blockの出た目の数に応じて受けるダメージを減らす
        current_HP = Mathf.Clamp(current_HP, 0, max_HP);//HPが0～100の範囲を超えないようにする
        UpdateHPBar();//HPバーを更新
    }
    private void Heal(int heal_count)//回復
    {
        current_HP+=heal_count * 10;//healの出た目の数に応じてHPを回復
        current_HP = Mathf.Clamp(current_HP, 0, max_HP);//HPが0～100の範囲を超えないようにする
        UpdateHPBar();//HPバーを更新
    }
    private void UpdateHPBar()
    {
        HP_bar.value = (float)current_HP / max_HP;//HPバーを現在のHPに更新
    }
}
