using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
public class Level_up : MonoBehaviour

{
    [SerializeField]private GameObject LevelUPButton_attack;
    [SerializeField]private GameObject LevelUPButton_block;
    [SerializeField]private GameObject LevelUPButton_heal;
    [SerializeField]private GameObject Button_next;
    public static List<int> LevelUP_Ability = new List<int>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Enemy_Status.islevelup == true)
        {
            LevelUPButton_attack.SetActive(true);
            LevelUPButton_block.SetActive(true);
            LevelUPButton_heal.SetActive(true);
            Button_next.SetActive(false);
            Enemy_Status.islevelup = false;
        }
        
    }
    
        public void OnButtonClick(int abilityIndex)
    {
        Debug.Log($"Button {abilityIndex} clicked!");
        LevelUP_Ability.Add(abilityIndex);
        LevelUPButton_attack.SetActive(false);
        LevelUPButton_block.SetActive(false);
        LevelUPButton_heal.SetActive(false);
        Button_next.SetActive(true);
    }
    

}
