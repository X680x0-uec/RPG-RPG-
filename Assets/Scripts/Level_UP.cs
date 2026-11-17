using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // ★追加
using System.Collections.Generic;
using TMPro;

public class Level_up : MonoBehaviour
{
    [SerializeField] private GameObject LevelUPButton_attack;
    [SerializeField] private GameObject LevelUPButton_block;
    [SerializeField] private GameObject LevelUPButton_heal;
    [SerializeField] private GameObject Button_next;
    [SerializeField] public GameObject ButtonParent;
    public static List<int> LevelUP_Ability = new List<int>();

    void Update()
    {
        if (Enemy_Status.islevelup == true)
        {
            ButtonParent.SetActive(false);
            LevelUPButton_attack.SetActive(true);
            LevelUPButton_block.SetActive(true);
            LevelUPButton_heal.SetActive(true);
            Button_next.SetActive(false);
            Enemy_Status.islevelup = false;

            // ★追加: 最初のボタン（攻撃アップ）に選択カーソルを合わせる
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(LevelUPButton_attack);
            }
        }
    }

    public void OnButtonClick(int abilityIndex)
    {
        SoundManager.Instance?.PlaySE(SoundManager.Instance.seLevelUpSelect);
        Debug.Log($"Button {abilityIndex} clicked!");
        LevelUP_Ability.Add(abilityIndex);
        LevelUPButton_attack.SetActive(false);
        LevelUPButton_block.SetActive(false);
        LevelUPButton_heal.SetActive(false);
        Button_next.SetActive(true);

        // ★追加: NEXTボタンに選択カーソルを合わせる
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(Button_next);
        }
    }
}