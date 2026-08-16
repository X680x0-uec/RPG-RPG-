using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Enemy_Status : MonoBehaviour
{
    [SerializeField] private int max_HP;//Enemyの最大HP
    [SerializeField] private int current_HP;//Enemyの現在のHP
    [SerializeField] private Slider HP_bar;//HPバーの参照
    [SerializeField] TextMeshProUGUI HP_text;//HPテキストの参照
    [SerializeField] public int damage;//敵の攻撃力

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        current_HP=max_HP;//EnemyのHPを最大HPに設定
        UpdateHP();//HPを更新
    }

    // Update is called once per frame
    void Update()
    {
        
    }
     public void TakeDamege(int enemy_damage)//敵のダメージ判定
    {
        current_HP -= enemy_damage;//敵のHPからPlayerの攻撃力とattack_countの積を引く
        current_HP = Mathf.Clamp(current_HP, 0, max_HP);//HPが0～100の範囲を超えないようにする
        UpdateHP();//HPを更新
    }
    private void UpdateHP()
    {
        HP_bar.value = (float)current_HP / max_HP;//HPバーを現在のHPに更新
        HP_text.text = $"Enemy: HP {current_HP}";//HPtextを現在のHPに更新
    }
}
