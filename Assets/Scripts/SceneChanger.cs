using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // 新Input Systemを使うために追加

public class SceneChanger : MonoBehaviour
{
    void Update()
    {
        // 新Input Systemでマウスクリック（または画面タップ）を判定
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            SceneManager.LoadScene("BattleScene");
        }
    }
}