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
    int slot_error_min = 1000; // スロットのずれの最小値を格納する変数
    string slot_flag = "false"; // スロットの回転を止めるためのフラグ

    [SerializeField] public GameObject ButtonParent;
    [SerializeField] private float defaultSpeed = 1.0f;

    void Start()
    {
    }

    void Update()
    {
        if (slot_flag == "true") // スロットの回転を止めるためのフラグが立っている場合
        {
            if (image_location[slot_error].anchoredPosition.y < -275.0f) // スロットのずれが真ん中より大きくなったら
            {
                StopSlot(slot_spin.Length); // スロットの回転を止める
                SoundManager.Instance?.PlaySE(SoundManager.Instance.seSlotStop); // リール停止音
                slot_flag = "false"; // スロットの回転を止めるためのフラグを下ろす
                isStopped = true; // 停止したスロットのisStoppedをtrueにする
                TriggerResult(); // countの集計
            }
        }
    }

    public void OnClickButton() // ボタンが押された時の処理
    {
        // すでに停止済み、または停止処理中なら無視する
        if (isStopped || slot_flag == "true") return;

        float minDistance = float.MaxValue;
        int bestIndex = 0;
        bool foundAbove = false;

        for (int i = 0; i < image_location.Length; i++)
        {
            float centerOffset = image_location[i].anchoredPosition.y + 275.0f;
            if (centerOffset > 0f && centerOffset < minDistance)
            {
                minDistance = centerOffset;
                bestIndex = i;
                foundAbove = true;
            }
        }

        // すべて判定ラインより下だった場合は一番近い図柄を選ぶ（すり抜け防止）
        if (!foundAbove)
        {
            for (int i = 0; i < image_location.Length; i++)
            {
                float dist = Mathf.Abs(image_location[i].anchoredPosition.y + 275.0f);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    bestIndex = i;
                }
            }
        }

        slot_error = bestIndex;
        Debug.Log(slot_error);
        Debug.Log(image_location[slot_error].anchoredPosition.y);

        slot_flag = "true"; // スロットの回転を止めるためのフラグを立てる
    }

    private void TriggerResult()
    {
        // すべてのスロットが止まっていて、count未集計の時に実行
        if (slot_results != null && slot_results.AllStopped() && !isResulted)
        {
            isResulted = true; // count集計済みになるのでisResultedをtrueにする
            slot_results.CountSlot(slot_results.slot_reels); // countSlot関数の呼び出し
            slot_results.StopSpinBGM();
            player.OnAllSlotsStopped(ButtonParent);
        }
    }

    public void StopSlot(int n) // スロットの回転を止めるための関数
    {
        for (int i = 0; i < n; i++)
        {
            slot_spin[i].enabled = false; // その列のスロットの回転を止める
        }
    }

    public void RestartSlot(int n) // スロットの回転を再開するための関数
    {
        isStopped = false; // スロットの回転が再開したのでisStoppedをfalseに戻す
        isResulted = false; // isResultedも同様にfalseに戻す
        slot_flag = "false"; // 停止用フラグも初期化する

        // プレイヤーが選択中の速度（Slot_Speed.slot_speed）を維持して再開する
        Slot_Spin.spin_speed = 1.0f * 300f * Slot_Speed.slot_speed;

        for (int i = 0; i < n; i++)
        {
            slot_spin[i].enabled = true; // その列のスロットの回転を再開する
        }
    }
}