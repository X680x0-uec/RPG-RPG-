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

    private const float CenterY = -275.0f;
    private const float SymbolSpacing = 290.0f / 3f; // 3枚で1周290fになる均等間隔（約96.67f）

    void Start()
    {
        for (int i = 0; i < slot_spin.Length; i++)
        {
            if (slot_spin[i] != null)
            {
                slot_spin[i].SetParentReel(this);
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
                // 0番目:上(-178.33f), 1番目:中央(-275.0f), 2番目:下(-371.67f)
                pos.y = CenterY + (1 - i) * SymbolSpacing;
                image_location[i].anchoredPosition = pos;
            }
        }
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

        Slot_Spin.spin_speed = 1.0f * 300f * Slot_Speed.slot_speed;

        for (int i = 0; i < n; i++)
        {
            slot_spin[i].enabled = true;
        }
    }
}