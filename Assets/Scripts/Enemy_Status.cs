using UnityEngine;
using UnityEngine.UI;

public class Enemy_Status : MonoBehaviour
{
    [SerializeField] private int max_HP;//Enemyの最大HP
    [SerializeField] private int current_HP;//Enemyの現在のHP
    [SerializeField] private Slider HP_bar;//HPバーの参照

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        current_HP=max_HP;//EnemyのHPを最大HPに設定
        HP_bar.value = (float)current_HP / max_HP;//HPバーに現在のHPを反映
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
