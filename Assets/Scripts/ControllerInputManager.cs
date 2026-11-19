using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerInputManager : MonoBehaviour
{
    [Header("スロット関連の参照")]
    [SerializeField] private Slot_Stop[] reels;          // 左・中・右の順に3つセット
    [SerializeField] private Slot_Restart slotRestart;   // Slot_Restartをセット
    [SerializeField] private Slot_Speed slotSpeed;       // Slot_Speedをセット
    [SerializeField] private GameObject buttonParent;    // 操作ボタンの親オブジェクト

    [Header("コンティニュー画面の参照（未設定でも自動検出します）")]
    [SerializeField] private Player_Status playerStatus;
    private int inputBlockFrames = 0;

    void Start()
    {
        if (playerStatus == null)
        {
            playerStatus = FindAnyObjectByType<Player_Status>();
        }
    }

    void Update()
    {
        // コンティニュー画面表示中、および閉じた直後の2フレームはスロット入力をブロック
        if (playerStatus != null && playerStatus.continuePanel != null && playerStatus.continuePanel.activeInHierarchy)
        {
            inputBlockFrames = 2;
            return;
        }
        if (inputBlockFrames > 0)
        {
            inputBlockFrames--;
            return;
        }

        var gamepad = Gamepad.current;
        var keyboard = Keyboard.current;

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

    // リール速度変更（左で1段階遅く / 右で1段階速く）
    private void HandleSlotSpeed(Gamepad gamepad, Keyboard keyboard)
    {
        if (slotSpeed == null) return;

        bool left = (gamepad != null && (gamepad.dpad.left.wasPressedThisFrame || gamepad.leftStick.left.wasPressedThisFrame || gamepad.buttonWest.wasPressedThisFrame))
                 || (keyboard != null && (keyboard.leftArrowKey.wasPressedThisFrame || keyboard.aKey.wasPressedThisFrame));

        bool right = (gamepad != null && (gamepad.dpad.right.wasPressedThisFrame || gamepad.leftStick.right.wasPressedThisFrame || gamepad.buttonEast.wasPressedThisFrame))
                  || (keyboard != null && (keyboard.rightArrowKey.wasPressedThisFrame || keyboard.dKey.wasPressedThisFrame));

        int currentSpeedIndex = GetCurrentSpeedIndex();

        if (left && currentSpeedIndex > 1)
        {
            slotSpeed.OnClickButton(currentSpeedIndex - 1); // 1段階遅くする（高速→通常→低速）
        }
        else if (right && currentSpeedIndex < 3)
        {
            slotSpeed.OnClickButton(currentSpeedIndex + 1); // 1段階速くする（低速→通常→高速）
        }
    }

    // 現在のスロット速度からインデックス（1:低速, 2:通常, 3:高速）を取得
    private int GetCurrentSpeedIndex()
    {
        if (Mathf.Approximately(Slot_Speed.slot_speed, 0.5f)) return 1;
        if (Mathf.Approximately(Slot_Speed.slot_speed, 1.5f)) return 3;
        return 2; // 1.0f（通常）
    }

    // スロット開始：決定ボタン（XboxのAボタン / Space / Enter）
    private void HandleSlotStart(Gamepad gamepad, Keyboard keyboard)
    {
        bool padStart = gamepad != null && gamepad.buttonSouth.wasPressedThisFrame;
        bool keyStart = keyboard != null && (keyboard.spaceKey.wasPressedThisFrame || keyboard.enterKey.wasPressedThisFrame);

        if (padStart || keyStart)
        {
            if (slotRestart != null)
            {
                slotRestart.RestartAllSlots();
            }
        }
    }

    // リール停止（左：十字左 or X / 中：十字下 or A / 右：十字右 or B）
    private void HandleSlotStop(Gamepad gamepad, Keyboard keyboard)
    {
        if (reels == null || reels.Length < 3) return;

        bool left  = (gamepad != null && (gamepad.dpad.left.wasPressedThisFrame  || gamepad.buttonWest.wasPressedThisFrame))  || (keyboard != null && (keyboard.leftArrowKey.wasPressedThisFrame  || keyboard.aKey.wasPressedThisFrame));
        bool down  = (gamepad != null && (gamepad.dpad.down.wasPressedThisFrame  || gamepad.buttonSouth.wasPressedThisFrame)) || (keyboard != null && (keyboard.downArrowKey.wasPressedThisFrame  || keyboard.sKey.wasPressedThisFrame));
        bool right = (gamepad != null && (gamepad.dpad.right.wasPressedThisFrame || gamepad.buttonEast.wasPressedThisFrame))  || (keyboard != null && (keyboard.rightArrowKey.wasPressedThisFrame || keyboard.dKey.wasPressedThisFrame));

        if (left)
        {
            StopReelIfMoving(0); // 左リール停止
        }
        else if (down)
        {
            StopReelIfMoving(1); // 中リール停止
        }
        else if (right)
        {
            StopReelIfMoving(2); // 右リール停止
        }
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