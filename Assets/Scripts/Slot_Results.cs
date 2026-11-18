using UnityEngine;

public class Slot_Results : MonoBehaviour
{
    public Slot_Stop[] slot_reels; // 三つのスロットを参照
    public int attack_count; // attackの目の数
    public int block_count;  // blockの目の数
    public int heal_count;   // healの目の数
    public int curse_count;  // デメリット絵柄（呪い）の目の数
    [SerializeField] private AudioSource spinBGM; // スロット回転中のBGM

    public bool AllStopped()
    {
        for (int i = 0; i < slot_reels.Length; i++)
        {
            if (!slot_reels[i].isStopped)
            {
                return false;
            }
        }
        return true;
    }

    public void CountSlot(Slot_Stop[] slot_reels)
    {
        attack_count = 0;
        block_count = 0;
        heal_count = 0;
        curse_count = 0;

        for (int i = 0; i < slot_reels.Length; i++)
        {
            int symbolType = slot_reels[i].GetStoppedSymbolType();

            switch (symbolType)
            {
                case 0:
                    attack_count++;
                    break;
                case 1:
                    block_count++;
                    break;
                case 2:
                    heal_count++;
                    break;
                case 3:
                    // オールマイティ：全効果を同時にカウント
                    attack_count++;
                    block_count++;
                    heal_count++;
                    break;
                case 4:
                    // デメリット絵柄（呪い）：ペナルティカウント
                    curse_count++;
                    break;
            }
        }
        Debug.Log($"attack={attack_count}, block={block_count}, heal={heal_count}, curse={curse_count}");
    }

    public void StopSpinBGM()
    {
        if (spinBGM != null)
        {
            spinBGM.Stop();
        }
    }
}