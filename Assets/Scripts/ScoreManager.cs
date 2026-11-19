using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    // シーンを跨いで保持するスコア・コンティニュー情報
    public static int currentScore = 0;
    public static int continueCount = 0;
    public static int currentEnemyTurn = 0;
    public static int lastNewRank = 0; // 今回ランクインした順位（1〜5、圏外は0）
    public static bool isContinueing = false; // コンティニュー中かどうかのフラグ

    [Header("スコア加算設定")]
    [SerializeField] private int twoMatchBaseScore = 100;   // 2つ揃い基本点
    [SerializeField] private int threeMatchBaseScore = 300; // 3つ揃い基本点
    [SerializeField] private int turnBonusUnit = 100;       // 早期撃破ボーナス単価
    [SerializeField] private int targetMaxTurns = 10;       // ボーナス対象の上限ターン数
    [SerializeField] private int continueBonusUnit = 3000;   // 低コンティニューボーナス単価

    [Header("呪い揃い時のスコア減少設定")]
    [SerializeField] private int curseTwoMatchPenalty = 100;          // 呪い2つ揃い減少点
    [SerializeField] private int curseThreeMatchPenalty = 300;        // 呪い3つ揃い減少点
    [SerializeField] private bool applySpeedMultiplierToCurse = true; // 減少点にも速度倍率を掛けるか

    [Header("バトル画面用UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI turnAndContinueText;
    [SerializeField] private TextMeshProUGUI scorePopupText; // 未設定ならscoreTextの下に自動生成

    [Header("リザルト・ランキング画面用UI")]
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI rankingText;

    private bool isScoreFinalized = false;
    private Vector2 popupBasePosition;
    private Coroutine popupCoroutine;

    private const float BaseSpinSpeed = 300f;
    private const int MaxRankCount = 5;
    private const string RankKeyPrefix = "SlotRPG_HighScore_";

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
    }

    private void Start()
    {
        currentEnemyTurn = 0;
        isScoreFinalized = false;
        SetupPopupText();
        UpdateAllUI();
    }

    private void SetupPopupText()
    {
        if (scorePopupText == null && scoreText != null)
        {
            scorePopupText = Instantiate(scoreText, scoreText.transform.parent);
            scorePopupText.name = "ScorePopupText";
            RectTransform rt = scorePopupText.rectTransform;
            rt.anchoredPosition = scoreText.rectTransform.anchoredPosition + new Vector2(0f, -45f);
        }

        if (scorePopupText != null)
        {
            popupBasePosition = scorePopupText.rectTransform.anchoredPosition;
            scorePopupText.gameObject.SetActive(false);
        }
    }

    // タイトル画面から「最初から」遊ぶ時に呼ぶリセット処理
    public static void ResetForNewGame()
    {
        currentScore = 0;
        continueCount = 0;
        currentEnemyTurn = 0;
        lastNewRank = 0;
    }

    // コンティニュー時に呼ぶ処理（コンティニュー回数を+1）
    public static void RegisterContinue()
    {
        continueCount++;
        currentEnemyTurn = 0;
        isContinueing = true;
        lastNewRank = 0;
    }

    public void ResetTurnForNextEnemy()
    {
        currentEnemyTurn = 0;
        UpdateAllUI();
    }

    public void OnTurnPassed()
    {
        currentEnemyTurn++;
        UpdateAllUI();
    }

    // 現在の実際のリール速度（選択速度 × レベルアップスキル速度倍率）からスコア倍率を算出
    public float GetCurrentSpeedScoreMultiplier()
    {
        float currentSpeed = Slot_Speed.GetCurrentSpinSpeed();
        if (currentSpeed > 0f)
        {
            return currentSpeed / BaseSpinSpeed;
        }
        return Slot_Speed.slot_speed * Level_up.GetReelSpeedMultiplier();
    }

    private void ChangeScore(int delta)
    {
        if (delta == 0) return;

        currentScore = Mathf.Max(0, currentScore + delta);
        UpdateAllUI();
        ShowScorePopup(delta);
    }

    private void ShowScorePopup(int delta)
    {
        if (scorePopupText == null) return;

        if (popupCoroutine != null)
        {
            StopCoroutine(popupCoroutine);
        }
        popupCoroutine = StartCoroutine(AnimateScorePopup(delta));
    }

    private IEnumerator AnimateScorePopup(int delta)
    {
        scorePopupText.gameObject.SetActive(true);
        scorePopupText.rectTransform.anchoredPosition = popupBasePosition;

        if (delta > 0)
        {
            scorePopupText.text = $"+{delta}";
            scorePopupText.color = Color.yellow;
        }
        else
        {
            scorePopupText.text = $"{delta}";
            scorePopupText.color = new Color(1f, 0.3f, 0.3f);
        }

        float duration = 1.2f;
        float elapsed = 0f;
        Color startColor = scorePopupText.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            scorePopupText.rectTransform.anchoredPosition = popupBasePosition + new Vector2(0f, 30f * t);

            if (t > 0.6f)
            {
                float alpha = 1f - ((t - 0.6f) / 0.4f);
                scorePopupText.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Clamp01(alpha));
            }

            yield return null;
        }

        scorePopupText.gameObject.SetActive(false);
        popupCoroutine = null;
    }

    // 絵柄揃い時のスコア増減
    public void AddMatchScore(int attackCount, int blockCount, int healCount, int curseCount = 0)
    {
        float speedMultiplier = GetCurrentSpeedScoreMultiplier();

        int plusBasePoints = 0;
        plusBasePoints += GetSingleSymbolMatchScore(attackCount);
        plusBasePoints += GetSingleSymbolMatchScore(blockCount);
        plusBasePoints += GetSingleSymbolMatchScore(healCount);
        int plusScore = Mathf.RoundToInt(plusBasePoints * speedMultiplier);

        int penaltyBasePoints = GetCurseMatchPenalty(curseCount);
        int minusScore = applySpeedMultiplierToCurse
            ? Mathf.RoundToInt(penaltyBasePoints * speedMultiplier)
            : penaltyBasePoints;

        int delta = plusScore - minusScore;
        if (delta == 0) return;

        ChangeScore(delta);
    }

    private int GetSingleSymbolMatchScore(int count)
    {
        if (count >= 3) return threeMatchBaseScore;
        if (count == 2) return twoMatchBaseScore;
        return 0;
    }

    private int GetCurseMatchPenalty(int count)
    {
        if (count >= 3) return curseThreeMatchPenalty;
        if (count == 2) return curseTwoMatchPenalty;
        return 0;
    }

    // 敵撃破時の早期撃破ボーナス加算
    public void AddDefeatTurnBonus(int enemyIndex)
    {
        int remainingTurns = Mathf.Max(0, (targetMaxTurns + 1) - currentEnemyTurn);
        int turnBonus = remainingTurns * turnBonusUnit;

        if (turnBonus > 0)
        {
            ChangeScore(turnBonus);
        }
    }

    // ゲーム終了時にコンティニューボーナスを確定しランキング保存
    public void FinalizeGameScore(bool isGameClear)
    {
        if (isScoreFinalized) return;
        isScoreFinalized = true;

        int contBonus = 0;
        if (isGameClear)
        {
            contBonus = Mathf.Max(0, 3 - continueCount) * continueBonusUnit;
            if (contBonus > 0)
            {
                ChangeScore(contBonus);
            }
        }

        SaveScoreToRanking(currentScore);
        UpdateAllUI();
    }

    private void SaveScoreToRanking(int newScore)
    {
        List<int> scores = GetRankingList();
        lastNewRank = 0;

        if (newScore > 0)
        {
            scores.Add(newScore);
            scores.Sort((a, b) => b.CompareTo(a));

            if (scores.Count > MaxRankCount)
            {
                scores.RemoveRange(MaxRankCount, scores.Count - MaxRankCount);
            }

            for (int i = 0; i < scores.Count; i++)
            {
                if (scores[i] == newScore)
                {
                    lastNewRank = i + 1;
                    break;
                }
            }

            for (int i = 0; i < scores.Count; i++)
            {
                PlayerPrefs.SetInt(RankKeyPrefix + i, scores[i]);
            }
            PlayerPrefs.Save();
        }
    }

    public List<int> GetRankingList()
    {
        List<int> scores = new List<int>();
        for (int i = 0; i < MaxRankCount; i++)
        {
            scores.Add(PlayerPrefs.GetInt(RankKeyPrefix + i, 0));
        }
        scores.Sort((a, b) => b.CompareTo(a));
        return scores;
    }

    public void ClearRankingData()
    {
        for (int i = 0; i < MaxRankCount; i++)
        {
            PlayerPrefs.DeleteKey(RankKeyPrefix + i);
        }
        PlayerPrefs.Save();
        UpdateAllUI();
    }

    public void UpdateAllUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"スコア: {currentScore}";
        }

        if (turnAndContinueText != null)
        {
            float mult = GetCurrentSpeedScoreMultiplier();
            turnAndContinueText.text = $"ターン: {currentEnemyTurn}   コンティニュー回数: {continueCount}";
        }

        if (finalScoreText != null)
        {
            string rankMsg = (lastNewRank > 0) ? $"  【NEW! {lastNewRank}位】" : "";
            finalScoreText.text = $"FINAL SCORE: {currentScore}{rankMsg}";
        }

        if (rankingText != null)
        {
            List<int> list = GetRankingList();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("=== HIGH SCORE RANKING ===");
            for (int i = 0; i < list.Count; i++)
            {
                string mark = (lastNewRank == i + 1) ? " ◀ NEW!" : "";
                sb.AppendLine($"{i + 1}位 : {list[i]} pt{mark}");
            }
            rankingText.text = sb.ToString();
        }
    }
}