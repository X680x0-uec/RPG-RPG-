using UnityEngine;
using UnityEngine.UI;
public class Slot_Speed : MonoBehaviour
{
    public static float slot_speed = 1.0f; //変数の宣言,staticにすることで他のスクリプトからもアクセス可能...だったはず

     public void OnClickButton(int button_speed) //ボタンが押された時の処理
    {
        switch(button_speed)
        {
            case 1:
                slot_speed = 0.5f; //遅め
                Player_Status.speed_bonus = 0.5f; // ★追加: スピードボーナスを遅めに設定
                Slot_Spin.spin_speed = 1.0f * 300 * slot_speed; //スロットの回転速度を遅くする
                break;
            case 2:
                slot_speed = 1.0f; //通常
                Player_Status.speed_bonus = 1.0f; // ★追加: スピードボーナスを通常に設定
                Slot_Spin.spin_speed = 1.0f * 300 * slot_speed; //スロットの回転速度を通常にする
                break;
            case 3:
                slot_speed = 1.5f; //速め
                Player_Status.speed_bonus = 1.5f; // ★追加: スピードボーナスを速めに設定
                Slot_Spin.spin_speed = 1.0f * 300 * slot_speed; //スロットの回転速度を速くする
                break;
        }
    }
}
