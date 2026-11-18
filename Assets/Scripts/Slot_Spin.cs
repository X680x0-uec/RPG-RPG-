using UnityEngine;
using UnityEngine.UI;

public class Slot_Spin : MonoBehaviour
{
    public static float spin_speed = 1.0f * 300 * Slot_Speed.slot_speed;
    public Vector2 restart_position;
    public float loop_hight = 600.0f;
    private RectTransform rectTransform;

    [HideInInspector] public int currentSymbolType = 0; // 0:攻撃, 1:防御, 2:回復, 3:オールマイティ, 4:デメリット
    private Slot_Stop parentReel;
    private Image symbolImage;
    private Color originalColor = Color.white;

    // 中央(-275f)を基準に上下対称（-130f ～ -420f）で1周290fとする
    private const float BottomLimit = -420.0f;
    private const float LoopSpan = 290.0f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        restart_position = new Vector2(rectTransform.anchoredPosition.x, 500f);
    }

    public void InitializeSymbol(Slot_Stop reel, int initialType, Sprite initialSprite)
    {
        parentReel = reel;
        currentSymbolType = initialType;
        symbolImage = GetComponent<Image>();
        if (symbolImage != null)
        {
            originalColor = symbolImage.color;
            if (initialSprite != null) symbolImage.sprite = initialSprite;
        }
    }

    public void ApplySymbolVisual(int symbolType, Sprite newSprite, Color? tintColor)
    {
        currentSymbolType = symbolType;
        if (symbolImage == null) symbolImage = GetComponent<Image>();
        if (symbolImage != null)
        {
            if (newSprite != null) symbolImage.sprite = newSprite;
            symbolImage.color = tintColor.HasValue ? tintColor.Value : originalColor;
        }
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

            if (parentReel != null)
            {
                parentReel.AssignNextSymbol(this);
            }
        }
    }
}