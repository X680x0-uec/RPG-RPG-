using UnityEngine;
using UnityEngine.UI;
public class Slot_Speed : MonoBehaviour
{
    public static float slot_speed; //変数の宣言,staticにすることで他のスクリプトからもアクセス可能...だったはず

     public void OnClickButton(int button_speed) //ボタンが押された時の処理
    {
        switch(button_speed)
        {
            case 1:
                slot_speed = 0.5f; //遅め
                break;
            case 2:
                slot_speed = 1.0f; //通常
                break;
            case 3:
                slot_speed = 1.2f; //速め
                break;
        }
    }
}
