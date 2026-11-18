using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneChanger2 : MonoBehaviour
{
    [SerializeField] private SceneManager SceneManager;

    void Update()
    {
        bool isMouseClicked = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;

        bool isGamepadPressed = Gamepad.current != null && 
            (Gamepad.current.buttonSouth.wasPressedThisFrame || Gamepad.current.startButton.wasPressedThisFrame);

        bool isKeyboardPressed = Keyboard.current != null &&
            (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame);

        if (isMouseClicked || isGamepadPressed || isKeyboardPressed)
        {
            SceneManager.LoadScene("BattleScene");
        }
    }
    
}