using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
public class Level_up : MonoBehaviour

{
    [SerializeField] private GameObject ButtonPrefab;
    [SerializeField]private GameObject LevelUPButton;
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
            CreateNewButton();
            Enemy_Status.islevelup = false;
        }
        
    }
    public void CreateNewButton()
    {
        if (LevelUPButton == null) return;
        LevelUPButton.SetActive(true);
        
        foreach (Transform child in LevelUPButton.transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < 3; i++)
        {
            GameObject newButton = Instantiate(ButtonPrefab, LevelUPButton.transform);

            TMP_Text buttonText = LevelUPButton.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                if (i == 0)
                {
                    buttonText.text = $"Attack + 10";
                }
                else if (i == 1)
                {
                    buttonText.text = $"Block + 5";
                }
                else
                {
                    buttonText.text = $"Heal + 10";
                }
            }
            Button buttonComponent = newButton.GetComponent<Button>();
            if (buttonComponent != null)
            {
                int abilityIndex = i; 
                LevelUP_Ability.Add(abilityIndex  + 1);
                int capturedIndex = abilityIndex; // キャプチャされた変数を使用
                buttonComponent.onClick.AddListener(() => OnButtonClick(capturedIndex));
                
            }
            
        }
    }
        private void OnButtonClick(int abilityIndex)
    {
        Debug.Log($"Button {abilityIndex} clicked!");
        LevelUPButton.SetActive(false);
    }
    

}
