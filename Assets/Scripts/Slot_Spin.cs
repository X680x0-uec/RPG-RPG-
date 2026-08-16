using UnityEngine;
using UnityEngine.UI;
public class Slot_Spin : MonoBehaviour
{
    public static float spin_speed = 1.0f * 300;//Slot_Speed.slot_speed;//変数の宣言
    public Vector2 restart_position ;
    public float loop_hight = 250.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        restart_position = new Vector2(transform.position.x, 250f); //オブジェクトのループ位置を変数に代入
    }

    // Update is called once per frame
    void Update() //記憶が正しければこっちの関数は毎フレーム呼ばれる関数だったはず
    {
        float object_location = transform.position.y; //オブジェクトの位置を取得
        object_location -= spin_speed*Time.deltaTime; //オブジェクトの位置を変数に代入
        //Debug.Log(object_location); //デバッグ用にオブジェクトの位置を表示
        transform.position = new Vector2(transform.position.x, object_location); //オブジェクトの位置を変更
        if (transform.position.y < 60.0f) //オブジェクトの位置が画面外に出たら
        {
            //transform.position = restart_position; //オブジェクトの位置を上に戻す
            transform.position = new Vector2(transform.position.x, transform.position.y + loop_hight); //Updateで動かしてるから一行上のだとずれる
        }
    }
}
