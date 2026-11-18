using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneChanger2 : MonoBehaviour
{
    public AudioSource openingBGM; // オープニングBGMの参照
    void Update()
    {
        bool isMouseClicked = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;

        bool isGamepadPressed = Gamepad.current != null &&
            (Gamepad.current.buttonSouth.wasPressedThisFrame || Gamepad.current.startButton.wasPressedThisFrame);

        bool isKeyboardPressed = Keyboard.current != null &&
            (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame);

        if (isMouseClicked || isGamepadPressed || isKeyboardPressed)
        {
            ScoreManager.ResetForNewGame();
            if (openingBGM != null)
            {
                openingBGM.Stop(); // BGMを停止
            }
            SceneManager.LoadScene("BattleScene");
        }
    }
}