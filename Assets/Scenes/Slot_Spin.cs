using UnityEngine;
using UnityEngine.UI;
public class Slot_Spin : MonoBehaviour
{
    public static float spin_speed = 1.0f * 300;//Slot_Speed.slot_speed;//変数の宣言
    public Vector2 initial_position ;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initial_position = new Vector2(transform.position.x, transform.position.y);
    }

    // Update is called once per frame
    void Update() //記憶が正しければこっちの関数は毎フレーム呼ばれる関数だったはず
    {
        float object_location = transform.position.y; //オブジェクトの位置を取得
        object_location -= spin_speed*Time.deltaTime; //オブジェクトの位置を変数に代入
        //Debug.Log(object_location); //デバッグ用にオブジェクトの位置を表示
        transform.position = new Vector2(transform.position.x, object_location); //オブジェクトの位置を変更
        if ((initial_position.y - transform.position.y) > 500f) //オブジェクトの位置が画面外に出たら
        {
            transform.position = initial_position; //オブジェクトの位置を初期位置に戻す
        }
    }
}
