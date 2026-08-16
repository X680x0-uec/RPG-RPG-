using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Slot_Stop : MonoBehaviour
{
    [SerializeField] private Slot_Results slot_results;//Slot_Resultsスクリプトを参照
    [SerializeField] private Player_Status player;
    public RectTransform[] image_location; //画像の位置を取得するための変数
    public Slot_Spin[] slot_spin; //スロットの回転を止めるための変数
    public bool isStopped = false;//false→スロットが回転中、true→スロットが止まっている
    private bool isResulted = false;//false→count未集計、true→count集計済み
    public int slot_error = 0; //スロットのずれを格納する変数
    int slot_error_min = 1000; //スロットのずれの最小値を格納する変数
    string slot_flag = "false"; //スロットの回転を止めるためのフラグ
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (slot_flag == "true") //スロットの回転を止めるためのフラグが立っている場合
        {
            if (image_location[slot_error].anchoredPosition.y < -185.0f) //スロットのずれが真ん中より大きくなったら
            {
                StopSlot(slot_spin.Length); //スロットの回転を止める
                slot_flag = "false"; //スロットの回転を止めるためのフラグを下ろす
                isStopped=true;//停止したスロットのisStoppedをtrueにする
                TriggerResult();//countの集計
            }
        }
    }
    public void OnClickButton() //ボタンが押された時の処理
    {
        slot_error_min = 1000;
        int image_count = image_location.Length;
        List<float> image_center = new List<float>(); //画像の中心位置を格納するリスト
        for (int i = 0; i < image_count; i++)
        {
            image_center.Add(image_location[i].anchoredPosition.y + 185.0f); //画像の中心位置をリストに格納
            Debug.Log(image_location[i].anchoredPosition.y + 185.0f); //画像の中心位置をデバッグログに出力
        }
        //image_center.Sort(); //画像の中心位置を昇順に並べ替え
        //image_center.Reverse(); //画像の中心位置を降順に並べ替え
        image_count = image_center.Count; //画像の数を取得
        for (int i = 0; i < image_count; i++)
        {
            if (Mathf.Abs(image_center[i]) < slot_error_min && image_center[i] > 0) //画像の中心位置と真ん中の位置の差が最小値より小さい場合,Mathf.Absは絶対値を返す関数らしい
            {
                slot_error_min = (int) Mathf.Abs(image_center[i]); //最小値を更新
                slot_error = i; //どのスロットが一番真ん中に近いかを格納
            }
        }
        Debug.Log(slot_error); //どのスロットが一番真ん中に近いか
        Debug.Log(image_location[slot_error].anchoredPosition.y); //どのスロットが一番真ん中に近いかの位置
        Slot_Speed.slot_speed = 0.03f; //スロットの回転速度を遅くする
        slot_flag = "true"; //スロットの回転を止めるためのフラグを立てる
    }
    private void TriggerResult()
    {
        // すべてのスロットが止まっていて、count未集計の時に実行
        if (slot_results != null && slot_results.AllStopped() && !isResulted)
        {
            isResulted = true; //count集計済みになるのでisResultedをtrueにする
            slot_results.CountSlot(slot_results.slot_reels); //countSlot関数の呼び出し
            player.OnAllSlotsStopped();//
        }
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
        isStopped = false;//スロットの回転が再開したのでisStoppedをfalseに戻す
        isResulted=false;//isResultedも同様にfalseに戻す
        for (int i = 0; i < n; i++)
        {
            slot_spin[i].enabled = true; //その列のスロットの回転を再開する
        }
    }
}
