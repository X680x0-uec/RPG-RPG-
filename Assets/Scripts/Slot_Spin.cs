using UnityEngine;

public class Slot_Spin : MonoBehaviour
{
    public static float spin_speed = 1.0f * 300 * Slot_Speed.slot_speed;
    public Vector2 restart_position;
    public float loop_hight = 600.0f;
    private RectTransform rectTransform;
    private Slot_Stop parentReel;

    // 中央(-275f)を基準に上下対称（-130f ～ -420f）で1周290fとする
    private const float BottomLimit = -420.0f;
    private const float LoopSpan = 290.0f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        restart_position = new Vector2(rectTransform.anchoredPosition.x, 500f);
    }

    public void SetParentReel(Slot_Stop reel)
    {
        parentReel = reel;
    }

    void Update()
    {
        // 停止確定した同じフレーム内で残りの絵柄が1フレーム分動かないようにする
        if (parentReel != null && parentReel.isStopped) return;

        Vector2 current_position = rectTransform.anchoredPosition;
        current_position.y -= spin_speed * Time.deltaTime;
        rectTransform.anchoredPosition = current_position;

        if (rectTransform.anchoredPosition.y < BottomLimit)
        {
            rectTransform.anchoredPosition = new Vector2(
                rectTransform.anchoredPosition.x,
                rectTransform.anchoredPosition.y + LoopSpan
            );
        }
    }
}