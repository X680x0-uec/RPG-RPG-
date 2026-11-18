using UnityEngine;
using UnityEngine.UI;

public class Slot_Speed : MonoBehaviour
{
    public static float slot_speed = 1.0f;

    [Header("速度ボタンの登録")]
    [SerializeField] private Image slowButtonImage;
    [SerializeField] private Image normalButtonImage;
    [SerializeField] private Image fastButtonImage;

    [Header("ボタンの色設定")]
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color defaultColor = Color.white;

    void Start()
    {
        slot_speed = 1.0f;
        Slot_Spin.spin_speed = GetCurrentSpinSpeed();

        UpdateButtonColors(2);
        ScoreManager.Instance?.UpdateAllUI();
    }

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
                slot_speed = 0.5f;
                Player_Status.speed_bonus = 0.5f;
                break;
            case 2:
                slot_speed = 1.0f;
                Player_Status.speed_bonus = 1.0f;
                break;
            case 3:
                slot_speed = 1.5f;
                Player_Status.speed_bonus = 1.5f;
                break;
        }

        Slot_Spin.spin_speed = GetCurrentSpinSpeed();
        UpdateButtonColors(button_speed);
        ScoreManager.Instance?.UpdateAllUI();
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