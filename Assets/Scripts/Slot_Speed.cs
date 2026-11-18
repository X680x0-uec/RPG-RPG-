using UnityEngine;
using UnityEngine.UI;

public class Slot_Speed : MonoBehaviour
{
    public static float slot_speed = 1.0f;

    [Header("速度ボタンの登録")]
    [SerializeField] private Image slowButtonImage;   // 低速ボタンをセット
    [SerializeField] private Image normalButtonImage; // 通常ボタンをセット
    [SerializeField] private Image fastButtonImage;   // 高速ボタンをセット

    [Header("ボタンの色設定")]
    [SerializeField] private Color selectedColor = Color.yellow; // 選択中の色
    [SerializeField] private Color defaultColor = Color.white;   // 未選択時の色

    void Start()
    {
        slot_speed = 1.0f;
        Slot_Spin.spin_speed = GetCurrentSpinSpeed();

        UpdateButtonColors(2);
    }

    // 基本速度 × 選択速度 × スキル倍率 を一括計算する
    public static float GetCurrentSpinSpeed()
    {
        return 300f * slot_speed * Level_up.GetReelSpeedMultiplier();
    }

    public void OnClickButton(int button_speed)
    {
        SoundManager.Instance?.PlaySE(SoundManager.Instance.seSpeedChange);
        switch (button_speed)
        {
            case 1:
                slot_speed = 0.5f; // 遅め
                Player_Status.speed_bonus = 0.5f;
                break;
            case 2:
                slot_speed = 1.0f; // 通常
                Player_Status.speed_bonus = 1.0f;
                break;
            case 3:
                slot_speed = 1.5f; // 速め
                Player_Status.speed_bonus = 1.5f;
                break;
        }

        Slot_Spin.spin_speed = GetCurrentSpinSpeed();
        UpdateButtonColors(button_speed);
    }

    private void UpdateButtonColors(int speedIndex)
    {
        SetButtonColor(slowButtonImage,   (speedIndex == 1) ? selectedColor : defaultColor);
        SetButtonColor(normalButtonImage, (speedIndex == 2) ? selectedColor : defaultColor);
        SetButtonColor(fastButtonImage,   (speedIndex == 3) ? selectedColor : defaultColor);
    }

    private void SetButtonColor(Image targetImage, Color color)
    {
        if (targetImage == null) return;
        targetImage.color = color;

        Button btn = targetImage.GetComponent<Button>();
        if (btn != null)
        {
            ColorBlock cb = btn.colors;
            cb.normalColor = color;
            cb.selectedColor = color;
            btn.colors = cb;
        }
    }
}