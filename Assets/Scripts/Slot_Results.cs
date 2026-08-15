using UnityEngine;

public class Slot_Results : MonoBehaviour
{
    public Slot_Stop[] slot_reels;//三つのスロットを参照
    public int attack_count;//attackの目の数
    public int block_count;//blockの目の数
    public int heal_count;//healの目の数
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool AllStopped()//スロットがすべて止まっているかどうかを判定する関数
    {
        for (int i = 0; i < slot_reels.Length; i++)
        {
            if (!slot_reels[i].isStopped)//動いているスロットがある場合はfalseを返す
            {
                return false; 
            }
        }
        return true;//すべてのスロットが止まった時trueを返す
    }
    public void CountSlot(Slot_Stop[] slot_reels)
    {
        // 各count数を初期化
        attack_count = 0;
        block_count = 0;
        heal_count = 0;
        for (int i = 0; i < slot_reels.Length; i++)
        {
            int slot_error = slot_reels[i].slot_error % 3;//slot_errorを0,1,2のどれかに変換

            switch (slot_error)
            {
                case 0:
                    attack_count++; //0の場合はattackのカウントをプラス1
                    break;
                case 1:
                    block_count++; //1の場合はblockのカウントをプラス1
                    break;
                case 2:
                    heal_count++; //2の場合はhealのカウントをプラス1
                    break;
            }
        }
        Debug.Log("attack=" + attack_count + ", block=" + block_count + ", heal=" + heal_count);
    }
}
