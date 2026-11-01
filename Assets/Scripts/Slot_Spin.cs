using UnityEngine;
using UnityEngine.UI;
public class Slot_Spin : MonoBehaviour
{
    public static float spin_speed = 1.0f * 300;//Slot_Speed.slot_speed;//変数の宣言
    public Vector2 restart_position ;
    public float loop_hight = 600.0f;
    private RectTransform rectTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        restart_position = new Vector2(rectTransform.anchoredPosition.x, 500f); //オブジェクトのループ位置を変数に代入
    }

    // Update is called once per frame
    void Update() //記憶が正しければこっちの関数は毎フレーム呼ばれる関数だったはず
    {
        Vector2 current_position = rectTransform.anchoredPosition; //オブジェクトの現在位置を取得
        current_position.y -= spin_speed * Time.deltaTime; //オブジェクトの位置
        rectTransform.anchoredPosition = current_position; //オブジェクトの位置を更新

        if (rectTransform.anchoredPosition.y < -450.0f) //オブジェクトの位置が画面外に出たら
        {
            //rectTransform.anchoredPosition = restart_position; //オブジェクトの位置を上に戻す
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, /*rectTransform.anchoredPosition.y + loop_hight*/ -160f); //Updateで動かしてるから一行上のだとずれる
        }
    }
}
