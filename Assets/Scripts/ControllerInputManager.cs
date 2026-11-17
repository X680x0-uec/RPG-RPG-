using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerInputManager : MonoBehaviour
{
    [Header("スロット関連の参照")]
    [SerializeField] private Slot_Stop[] reels;          // 左・中・右の順に3つセット
    [SerializeField] private Slot_Restart slotRestart;   // Slot_Restartをセット
    [SerializeField] private Slot_Speed slotSpeed;       // Slot_Speedをセット
    [SerializeField] private GameObject buttonParent;    // 操作ボタンの親オブジェクト

    void Update()
    {
        var gamepad = Gamepad.current;
        var keyboard = Keyboard.current;

        // コントローラーもキーボードも未接続なら何もしない
        if (gamepad == null && keyboard == null) return;

        // 1. 回転前（速度選択・スタートボタンが表示されている時）
        if (buttonParent != null && buttonParent.activeInHierarchy)
        {
            HandleSlotSpeed(gamepad, keyboard);
            HandleSlotStart(gamepad, keyboard);
        }
        // 2. リール回転中（1つでも動いている時）
        else if (!AreAllReelsStopped())
        {
            HandleSlotStop(gamepad, keyboard);
        }
    }

    // リール速度変更（左：低速 / 下：通常 / 右：高速）
    private void HandleSlotSpeed(Gamepad gamepad, Keyboard keyboard)
    {
        if (slotSpeed == null) return;

        if (IsLeftPressed(gamepad, keyboard))
        {
            slotSpeed.OnClickButton(1); // 1: 低速 (0.5f)
        }
        else if (IsDownPressed(gamepad, keyboard))
        {
            slotSpeed.OnClickButton(2); // 2: 通常 (1.0f)
        }
        else if (IsRightPressed(gamepad, keyboard))
        {
            slotSpeed.OnClickButton(3); // 3: 高速 (1.5f)
        }
    }

    // スロット開始：Y / Start または Space / Enterキー
    private void HandleSlotStart(Gamepad gamepad, Keyboard keyboard)
    {
        bool padStart = gamepad != null && (gamepad.buttonNorth.wasPressedThisFrame || gamepad.startButton.wasPressedThisFrame);
        bool keyStart = keyboard != null && (keyboard.spaceKey.wasPressedThisFrame || keyboard.enterKey.wasPressedThisFrame);

        if (padStart || keyStart)
        {
            if (slotRestart != null)
            {
                slotRestart.RestartAllSlots();
            }
        }
    }

    // リール停止（左・中・右）
    private void HandleSlotStop(Gamepad gamepad, Keyboard keyboard)
    {
        if (reels == null || reels.Length < 3) return;

        if (IsLeftPressed(gamepad, keyboard))
        {
            StopReelIfMoving(0); // 左リール停止
        }
        else if (IsDownPressed(gamepad, keyboard))
        {
            StopReelIfMoving(1); // 中リール停止
        }
        else if (IsRightPressed(gamepad, keyboard))
        {
            StopReelIfMoving(2); // 右リール停止
        }
    }

    // --- 共通の入力判定（コントローラー ＋ キーボード） ---
    private bool IsLeftPressed(Gamepad g, Keyboard k)
    {
        bool pad = g != null && (g.dpad.left.wasPressedThisFrame || g.buttonWest.wasPressedThisFrame);
        bool key = k != null && (k.leftArrowKey.wasPressedThisFrame || k.aKey.wasPressedThisFrame);
        return pad || key;
    }

    private bool IsDownPressed(Gamepad g, Keyboard k)
    {
        bool pad = g != null && (g.dpad.down.wasPressedThisFrame || g.buttonSouth.wasPressedThisFrame);
        bool key = k != null && (k.downArrowKey.wasPressedThisFrame || k.sKey.wasPressedThisFrame);
        return pad || key;
    }

    private bool IsRightPressed(Gamepad g, Keyboard k)
    {
        bool pad = g != null && (g.dpad.right.wasPressedThisFrame || g.buttonEast.wasPressedThisFrame);
        bool key = k != null && (k.rightArrowKey.wasPressedThisFrame || k.dKey.wasPressedThisFrame);
        return pad || key;
    }

    private void StopReelIfMoving(int index)
    {
        if (reels[index] != null && !reels[index].isStopped)
        {
            reels[index].OnClickButton();
        }
    }

    private bool AreAllReelsStopped()
    {
        if (reels == null) return true;
        foreach (var reel in reels)
        {
            if (reel != null && !reel.isStopped) return false;
        }
        return true;
    }
}