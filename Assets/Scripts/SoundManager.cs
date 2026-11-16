using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("オーディオソース（同じオブジェクトにアタッチ）")]
    [SerializeField] private AudioSource seSource;

    [Header("スロット操作の効果音")]
    public AudioClip seSlotStart;     // 再回転（スピン）ボタン
    public AudioClip seSlotStop;      // リール停止時
    public AudioClip seSpeedChange;   // スピード変更ボタン

    [Header("絵柄の揃い判定の効果音")]
    public AudioClip seAttackMid;     // アタック2つ揃い（中攻撃音）
    public AudioClip seAttackHigh;    // アタック3つ揃い（大攻撃音）
    public AudioClip seBlockMid;      // 防御2つ揃い（中防御音）
    public AudioClip seBlockHigh;     // 防御3つ揃い（大防御音）
    public AudioClip seHealMid;       // 回復2つ揃い（中回復音）
    public AudioClip seHealHigh;      // 回復3つ揃い（大回復音）
    public AudioClip seMatchAllOne;   // 全部1つずつ揃い専用音

    [Header("バトル結果・UIの効果音")]
    public AudioClip seEnemyDefeat;   // 敵撃破
    public AudioClip seGameClear;     // 全5体クリア
    public AudioClip seLevelUpSelect; // 能力選択ボタン
    public AudioClip seNextButton;    // NEXTボタン

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (seSource == null)
        {
            seSource = GetComponent<AudioSource>();
        }
    }

    // 効果音を1回鳴らす共通メソッド（clipが未設定なら何もしない）
    public void PlaySE(AudioClip clip)
    {
        if (seSource != null && clip != null)
        {
            seSource.PlayOneShot(clip);
        }
    }
}