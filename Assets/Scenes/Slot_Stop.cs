using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Slot_Stop : MonoBehaviour
{
    public RectTransform[] image_location; //画像の位置を取得するための変数
    public Slot_Spin[] slot_spin; //スロットの回転を止めるための変数
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClickButton() //ボタンが押された時の処理
    {
        StopSlot(slot_spin.Length); //スロットを停止
        Debug.Log("stop");
        int image_count = image_location.Length;
        List<float> image_center = new List<float>(); //画像の中心位置を格納するリスト
        for (int i = 0; i < image_count; i++)
        {
            Debug.Log(image_location[i].anchoredPosition.y); //終了地点の確認
            image_center.Add(image_location[i].anchoredPosition.y + 185.0f); //画像の中心位置をリストに格納
            Debug.Log(image_center[i]); //画像の中心位置の確認
        }
        image_center.Sort(); //画像の中心位置を昇順に並べ替え
        image_center.Reverse(); //画像の中心位置を降順に並べ替え
        image_count = image_center.Count; //画像の数を取得
        float slot_error = image_center[0]; //スロットのずれを格納する変数
        for (int i = 0; i < image_count; i++)
        {
            if (image_center[i] > 0.0f && image_center[i] < slot_error) 
            {
                slot_error = image_center[i]; //スロットのずれを更新する。ただし、0より大きく、かつ現在のずれより小さい場合のみ更新する
            }
        }
        Debug.Log(slot_error); //スロットのずれの確認
        float error_time = slot_error / Slot_Spin.spin_speed; //スロットのずれを修正するための時間を計算する 
        RestartSlot(slot_spin.Length); //スロットを再開
        float restart_time = 0.0f; //スロットの再開時間を格納する変数
        while (restart_time < error_time) //スロットの再開時間がスロットのずれを修正するための時間より小さい間、ループする
        {
            restart_time += Time.deltaTime; //スロットの再開時間を更新する
        }
        StopSlot(slot_spin.Length); //スロットを停止
        Debug.Log("stop");
    }
    public void StopSlot(int n) //スロットの回転を止めるための関数
    {
        for (int i = 0; i < n; i++)
        {
            slot_spin[i].enabled = false; //その列のスロットの回転を止める
        }
    }
    public void RestartSlot(int n) //スロットの回転を再開するための関数
    {
        for (int i = 0; i < n; i++)
        {
            slot_spin[i].enabled = true; //その列のスロットの回転を再開する
        }
    }
}
