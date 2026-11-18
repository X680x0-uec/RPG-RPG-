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
        // リトライ時に前のプレイの速度設定を引き継がないよう通常（1.0f）に初期化
        slot_speed = 1.0f;
        Slot_Spin.spin_speed = 1.0f * 300f * slot_speed;

        UpdateButtonColors(2);
    }

    public void OnClickButton(int button_speed)
    {
        SoundManager.Instance?.PlaySE(SoundManager.Instance.seSpeedChange);
        switch (button_speed)
        {
            case 1:
                slot_speed = 0.5f; // 遅め
                Player_Status.speed_bonus = 0.5f;
                Slot_Spin.spin_speed = 1.0f * 300 * slot_speed;
                break;
            case 2:
                slot_speed = 1.0f; // 通常
                Player_Status.speed_bonus = 1.0f;
                Slot_Spin.spin_speed = 1.0f * 300 * slot_speed;
                break;
            case 3:
                slot_speed = 1.5f; // 速め
                Player_Status.speed_bonus = 1.5f;
                Slot_Spin.spin_speed = 1.0f * 300 * slot_speed;
                break;
        }

        // 選んだ速度のボタンの色を変更する
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

        // Buttonコンポーネント側の設定で色が白に戻されるのを防ぐ
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