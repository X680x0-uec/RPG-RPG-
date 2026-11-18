using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class Level_up : MonoBehaviour
{
    [SerializeField] private GameObject LevelUPButton_attack;
    [SerializeField] private GameObject LevelUPButton_block;
    [SerializeField] private GameObject LevelUPButton_heal;
    [SerializeField] private GameObject Button_next;
    [SerializeField] public GameObject ButtonParent;

    // 互換性維持用リスト
    public static List<int> LevelUP_Ability = new List<int>();

    // 累計ステータス加算値
    public static int attackBonusTotal = 0;
    public static int blockBonusTotal = 0;
    public static int healBonusTotal = 0;

    // 獲得スキル（0:なし, 1:高速＆効果UP, 2:絵柄追加, 3:低速化＆全ステ小UP）
    public static int acquiredSkill = 0;

    void Start()
    {
        // リトライ時に前のプレイの強化状態をすべてリセット
        LevelUP_Ability.Clear();
        attackBonusTotal = 0;
        blockBonusTotal = 0;
        healBonusTotal = 0;
        acquiredSkill = 0;
    }

    void Update()
    {
        if (Enemy_Status.islevelup == true)
        {
            UpdateButtonLabels(Enemy_Status.currentEnemyIndex);

            ButtonParent.SetActive(false);
            LevelUPButton_attack.SetActive(true);
            LevelUPButton_block.SetActive(true);
            LevelUPButton_heal.SetActive(true);
            Button_next.SetActive(false);
            Enemy_Status.islevelup = false;

            // 最初のボタンに選択カーソルを合わせる
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(LevelUPButton_attack);
            }
        }
    }

    // 倒した敵の番号（0:1体目, 1:2体目, 2:3体目, 3:4体目）に応じてボタン表示を変更
    private void UpdateButtonLabels(int defeatedEnemyIndex)
    {
        switch (defeatedEnemyIndex)
        {
            case 0: // 1体目撃破：ステータスアップ小
                SetTexts("攻撃アップ（小）", "防御アップ（小）", "回復アップ（小）");
                break;
            case 1: // 2体目撃破：ステータスアップ中
                SetTexts("攻撃アップ（中）", "防御アップ（中）", "回復アップ（中）");
                break;
            case 2: // 3体目撃破：スキル獲得
                SetTexts("高速回転＆効果UP", "絵柄追加（万能＆呪い）", "低速化＆全ステ小UP");
                break;
            default: // 4体目撃破：ステータスアップ大
                SetTexts("攻撃アップ（大）", "防御アップ（大）", "回復アップ（大）");
                break;
        }
    }

    private void SetTexts(string label0, string label1, string label2)
    {
        ApplyButtonText(LevelUPButton_attack, label0);
        ApplyButtonText(LevelUPButton_block, label1);
        ApplyButtonText(LevelUPButton_heal, label2);
    }

    // TextMeshPro と 従来の Text のどちらでも文字を書き換え、ボタン枠内に自動フィットさせる
    private void ApplyButtonText(GameObject buttonObj, string label)
    {
        if (buttonObj == null) return;

        TextMeshProUGUI tmp = buttonObj.GetComponentInChildren<TextMeshProUGUI>(true);
        if (tmp != null)
        {
            tmp.text = label;
            tmp.enableAutoSizing = true;
            tmp.fontSizeMin = 10f;
            tmp.fontSizeMax = 30f;
            return;
        }

        Text legacyText = buttonObj.GetComponentInChildren<Text>(true);
        if (legacyText != null)
        {
            legacyText.text = label;
            legacyText.resizeTextForBestFit = true;
        }
    }

    public void OnButtonClick(int abilityIndex)
    {
        SoundManager.Instance?.PlaySE(SoundManager.Instance.seLevelUpSelect);
        LevelUP_Ability.Add(abilityIndex);

        int defeatedIndex = Enemy_Status.currentEnemyIndex;

        if (defeatedIndex == 2)
        {
            // 3体目撃破：スキル獲得（1:高速＆効果UP, 2:絵柄追加, 3:低速化＆全ステ小UP）
            acquiredSkill = abilityIndex + 1;

            // スキル③の場合はステータスアップ小（全項目）も同時に付与
            if (acquiredSkill == 3)
            {
                attackBonusTotal += 10;
                blockBonusTotal  += 5;
                healBonusTotal   += 10;
            }

            Slot_Spin.spin_speed = Slot_Speed.GetCurrentSpinSpeed();
            Debug.Log($"スキル獲得: {acquiredSkill} / ATK+{attackBonusTotal}, BLK+{blockBonusTotal}, HEAL+{healBonusTotal}");
        }
        else
        {
            // 1, 2, 4体目撃破：ステータスアップ（小・中・大）
            int atkAdd = 10, blkAdd = 5, healAdd = 10;
            if (defeatedIndex == 1)      { atkAdd = 25; blkAdd = 12; healAdd = 25; } // 中
            else if (defeatedIndex >= 3) { atkAdd = 50; blkAdd = 25; healAdd = 50; } // 大

            switch (abilityIndex)
            {
                case 0: attackBonusTotal += atkAdd; break;
                case 1: blockBonusTotal  += blkAdd; break;
                case 2: healBonusTotal   += healAdd; break;
            }
            Debug.Log($"ステータス強化: ATK+{attackBonusTotal}, BLK+{blockBonusTotal}, HEAL+{healBonusTotal}");
        }

        LevelUPButton_attack.SetActive(false);
        LevelUPButton_block.SetActive(false);
        LevelUPButton_heal.SetActive(false);
        Button_next.SetActive(true);

        // NEXTボタンに選択カーソルを合わせる
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(Button_next);
        }
    }

    // スキルによるリール回転速度の倍率
    public static float GetReelSpeedMultiplier()
    {
        if (acquiredSkill == 1) return 1.4f;  // スキル①：高速化
        if (acquiredSkill == 3) return 0.65f; // スキル③：低速化
        return 1.0f;
    }

    // スキルによる揃い効果の倍率
    public static float GetEffectMultiplier()
    {
        if (acquiredSkill == 1) return 1.3f;  // スキル①：効果アップ
        return 1.0f;
    }

    // スキル②（絵柄追加）を所持しているか
    public static bool HasExtraSymbolSkill()
    {
        return acquiredSkill == 2;
    }
}