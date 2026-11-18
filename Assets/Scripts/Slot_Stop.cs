using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot_Stop : MonoBehaviour
{
    [SerializeField] private Slot_Results slot_results; // Slot_Resultsスクリプトを参照
    [SerializeField] private Player_Status player;
    public RectTransform[] image_location; // 画像の位置を取得するための変数
    public Slot_Spin[] slot_spin; // スロットの回転を止めるための変数
    public bool isStopped = false; // false→スロットが回転中、true→スロットが止まっている
    private bool isResulted = false; // false→count未集計、true→count集計済み
    public int slot_error = 0; // スロットのずれを格納する変数
    string slot_flag = "false"; // スロットの回転を止めるためのフラグ

    [SerializeField] public GameObject ButtonParent;

    [Header("スキル②「絵柄追加」用の画像（任意・未設定なら色分けで表示）")]
    [SerializeField] private Sprite extraSymbolSprite; // オールマイティ用画像（未設定なら金色）
    [SerializeField] private Sprite curseSymbolSprite; // デメリット用画像（未設定なら紫色）

    private const float CenterY = -275.0f;
    private const float SymbolSpacing = 290.0f / 3f; // 3枚で1周290fになる均等間隔

    private Sprite[] defaultSprites = new Sprite[3];
    private int nextSpawnSymbolIndex = 0;

    void Start()
    {
        nextSpawnSymbolIndex = 0;

        // 初期の3枚の絵柄スプライト（0:攻撃, 1:防御, 2:回復）を記憶する
        for (int i = 0; i < slot_spin.Length && i < 3; i++)
        {
            if (slot_spin[i] != null)
            {
                Image img = slot_spin[i].GetComponent<Image>();
                if (img != null)
                {
                    defaultSprites[i] = img.sprite;
                }
                slot_spin[i].InitializeSymbol(this, i % 3, defaultSprites[i]);
            }
        }

        // ゲーム開始時に3枚の絵柄を中央(-275f)基準の均等な位置へ自動整列させる
        AlignInitialPositions();
    }

    private void AlignInitialPositions()
    {
        if (image_location == null) return;

        for (int i = 0; i < image_location.Length; i++)
        {
            if (image_location[i] != null)
            {
                Vector2 pos = image_location[i].anchoredPosition;
                pos.y = CenterY + (1 - i) * SymbolSpacing;
                image_location[i].anchoredPosition = pos;
            }
        }
    }

    // 画面外から上にループする図柄の絵柄IDと見た目を更新する（スキル②獲得時用）
    public void AssignNextSymbol(Slot_Spin targetSpin)
    {
        if (!Level_up.HasExtraSymbolSkill()) return;

        int assignedType = nextSpawnSymbolIndex;
        // 0:攻撃, 1:防御, 2:回復, 3:オールマイティ, 4:デメリット の5種類でローテーション
        nextSpawnSymbolIndex = (nextSpawnSymbolIndex + 1) % 5;

        Sprite spriteToUse = null;
        Color? tintColor = null;

        if (assignedType < 3)
        {
            spriteToUse = defaultSprites[assignedType];
        }
        else if (assignedType == 3)
        {
            spriteToUse = extraSymbolSprite;
            if (extraSymbolSprite == null) tintColor = Color.yellow;
        }
        else if (assignedType == 4)
        {
            spriteToUse = curseSymbolSprite;
            if (curseSymbolSprite == null) tintColor = new Color(0.75f, 0.25f, 1.0f);
        }

        targetSpin.ApplySymbolVisual(assignedType, spriteToUse, tintColor);
    }

    // 現在中央に止まっている絵柄の種類を返す
    public int GetStoppedSymbolType()
    {
        if (slot_spin != null && slot_error >= 0 && slot_error < slot_spin.Length && slot_spin[slot_error] != null)
        {
            return slot_spin[slot_error].currentSymbolType;
        }
        return slot_error % 3;
    }

    void Update()
    {
        if (slot_flag == "true")
        {
            if (image_location[slot_error].anchoredPosition.y <= CenterY)
            {
                // 通り過ぎたズレ分を計算し、リール内の全絵柄をぴったり中央（-275.0f）に補正する
                float snapOffset = CenterY - image_location[slot_error].anchoredPosition.y;
                for (int i = 0; i < image_location.Length; i++)
                {
                    if (image_location[i] != null)
                    {
                        Vector2 pos = image_location[i].anchoredPosition;
                        pos.y += snapOffset;
                        image_location[i].anchoredPosition = pos;
                    }
                }

                StopSlot(slot_spin.Length);
                SoundManager.Instance?.PlaySE(SoundManager.Instance.seSlotStop);
                slot_flag = "false";
                isStopped = true;
                TriggerResult();
            }
        }
    }

    public void OnClickButton()
    {
        if (isStopped || slot_flag == "true") return;

        float minDistance = float.MaxValue;
        int bestIndex = 0;
        bool foundAbove = false;

        for (int i = 0; i < image_location.Length; i++)
        {
            float centerOffset = image_location[i].anchoredPosition.y - CenterY;
            if (centerOffset > 0f && centerOffset < minDistance)
            {
                minDistance = centerOffset;
                bestIndex = i;
                foundAbove = true;
            }
        }

        if (!foundAbove)
        {
            for (int i = 0; i < image_location.Length; i++)
            {
                float dist = Mathf.Abs(image_location[i].anchoredPosition.y - CenterY);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    bestIndex = i;
                }
            }
        }

        slot_error = bestIndex;
        slot_flag = "true";
    }

    private void TriggerResult()
    {
        if (slot_results != null && slot_results.AllStopped() && !isResulted)
        {
            isResulted = true;
            slot_results.CountSlot(slot_results.slot_reels);
            slot_results.StopSpinBGM();
            player.OnAllSlotsStopped(ButtonParent);
        }
    }

    public void StopSlot(int n)
    {
        for (int i = 0; i < n; i++)
        {
            slot_spin[i].enabled = false;
        }
    }

    public void RestartSlot(int n)
    {
        isStopped = false;
        isResulted = false;
        slot_flag = "false";

        Slot_Spin.spin_speed = Slot_Speed.GetCurrentSpinSpeed();

        for (int i = 0; i < n; i++)
        {
            slot_spin[i].enabled = true;
        }
    }
}