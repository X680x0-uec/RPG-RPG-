using UnityEngine;

public class Slot_Restart : MonoBehaviour
{
    // インスペクターから全リールの Slot_Stop をセットします
    [SerializeField] private Slot_Stop[] allReels;
    [SerializeField] private GameObject ButtonParent;
    /// <summary>
    /// 全てのリールを一括で再回転させるメソッド
    /// （UIボタンの OnClick に登録します）
    /// </summary>
    public void RestartAllSlots()
    {
        SoundManager.Instance?.PlaySE(SoundManager.Instance.seSlotStart); // ★追加: スロット再回転音
        ButtonParent.SetActive(false);
        if (allReels == null || allReels.Length == 0)
        {
            Debug.LogWarning("リールがアタッチされていません！");
            return;
        }

        // 登録されているすべての Slot_Stop に対して RestartSlot を実行
        foreach (Slot_Stop reel in allReels)
        {
            if (reel != null)
            {
                // 各リールが持っている slot_spin の数だけ回転を再開させる
                reel.RestartSlot(reel.slot_spin.Length);
            }
        }

        Debug.Log("すべてのスロットを再回転させました。");
    }

    public void RecreateButtons()
    {
        Debug.Log("RecreateButtons called");
        ButtonParent.SetActive(true);
    }
}