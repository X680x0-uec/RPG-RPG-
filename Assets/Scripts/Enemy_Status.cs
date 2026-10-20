using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Enemy_Status : MonoBehaviour
{
    [SerializeField] private int max_HP;//Enemyの最大HP
    [SerializeField] private int current_HP;//Enemyの現在のHP
    [SerializeField] private Slider HP_bar;//HPバーの参照
    [SerializeField] TextMeshProUGUI HP_text;//HPテキストの参照
    [SerializeField] public int damage;//敵の攻撃力
    public static List<int> Enemy_HP = new List<int>();
    private int currentEnemyIndex = 0; // 現在の敵のインデックスを追跡する変数
    public static bool islevelup = false;

    public Result resultManager; // Result スクリプトの参照

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        for (int i = 0; i < 5; i++ )
        {
            Enemy_HP.Add(max_HP  + i * 20);//Enemy_HPリストに現在のHPを追加
        }
        current_HP = Enemy_HP[currentEnemyIndex];//EnemyのHPを最大HPに設定
        UpdateHP();//HPを更新
    }

    // Update is called once per frame
    void Update()
    {
        if (current_HP <= 0)
        {
            islevelup = true; // 敵が倒されたらislevelupをtrueにする
            currentEnemyIndex++; // 次の敵に進む
            max_HP = Enemy_HP[currentEnemyIndex];//EnemyのHPを最大HPに設定
            current_HP = Enemy_HP[currentEnemyIndex];//EnemyのHPを最大HPに設定
            UpdateHP();//HPを更新
        }
    }
     public void TakeDamege(int enemy_damage)//敵のダメージ判定
    {
        current_HP -= enemy_damage;//敵のHPからPlayerの攻撃力とattack_countの積を引く
        current_HP = Mathf.Clamp(current_HP, 0, max_HP);//HPが0～max_HPの範囲を超えないようにする
        UpdateHP();//HPを更新

        if(current_HP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if(resultManager != null)
        {
            resultManager.ShowResult();
        }
    }
    private void UpdateHP()
    {
        HP_bar.value = (float)current_HP / max_HP;//HPバーを現在のHPに更新
        HP_text.text = $"Enemy: HP {current_HP}";//HPtextを現在のHPに更新
    }
}
