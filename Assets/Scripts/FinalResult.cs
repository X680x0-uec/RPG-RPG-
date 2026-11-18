using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FinalResult : MonoBehaviour
{
    public GameObject gameClearText;  // ゲームクリア時に表示するテキスト
    public GameObject gameOverText;   // ゲームオーバー時に表示するテキスト
    public GameObject continueButton; // コンティニューボタンの参照
    public AudioSource gameOverBGM;   // ゲームオーバー時に再生するBGM
    public AudioSource gameClearBGM;  // ゲームクリア時に再生するBGM

    [Header("コントローラー操作用（未設定でも自動検出します）")]
    [SerializeField] private GameObject titleOrRetryButton; // 「タイトルへ戻る」「最初から遊ぶ」等のボタン

    private List<Selectable> activeButtons = new List<Selectable>();

    void Start()
    {
        if (Enemy_Status.isGameClear)
        {
            if (gameClearText != null) gameClearText.SetActive(true);
            if (gameOverText != null) gameOverText.SetActive(false);
            if (continueButton != null) continueButton.SetActive(false); // クリア時は非表示
            if (gameClearBGM != null) gameClearBGM.Play();
        }
        else
        {
            if (gameClearText != null) gameClearText.SetActive(false);
            if (gameOverText != null) gameOverText.SetActive(true);
            if (continueButton != null) continueButton.SetActive(true);  // ゲームオーバー時は表示
            if (gameOverBGM != null) gameOverBGM.Play();
        }

        // EventSystemの初期化完了を1フレーム待ってからカーソルを設定する
        StartCoroutine(SetupControllerFocus());
    }

    private IEnumerator SetupControllerFocus()
    {
        yield return null;

        BuildActiveButtonList();
        SetupNavigation();

        if (activeButtons.Count > 0)
        {
            SelectUI(activeButtons[0].gameObject);
        }
    }

    // 現在画面に表示されているボタンを優先順（コンティニュー → タイトル/リトライ → その他）で収集
    private void BuildActiveButtonList()
    {
        activeButtons.Clear();

        AddIfActive(continueButton);
        AddIfActive(titleOrRetryButton);

        // 未登録のボタンがシーン内にある場合も自動で拾う
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach (Button btn in allButtons)
        {
            if (btn != null && btn.gameObject.activeInHierarchy && btn.interactable)
            {
                if (!activeButtons.Contains(btn))
                {
                    activeButtons.Add(btn);
                }
            }
        }
    }

    private void AddIfActive(GameObject obj)
    {
        if (obj == null || !obj.activeInHierarchy) return;

        Selectable sel = obj.GetComponent<Selectable>();
        if (sel != null && sel.interactable && !activeButtons.Contains(sel))
        {
            activeButtons.Add(sel);
        }
    }

    // 表示中のボタン同士だけでカーソルがループするようにナビゲーションを固定
    private void SetupNavigation()
    {
        int count = activeButtons.Count;
        if (count == 0) return;

        if (count == 1)
        {
            Navigation nav = activeButtons[0].navigation;
            nav.mode = Navigation.Mode.None;
            activeButtons[0].navigation = nav;
            return;
        }

        for (int i = 0; i < count; i++)
        {
            Selectable prev = activeButtons[(i - 1 + count) % count];
            Selectable next = activeButtons[(i + 1) % count];

            Navigation nav = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnUp    = prev,
                selectOnLeft  = prev,
                selectOnDown  = next,
                selectOnRight = next
            };
            activeButtons[i].navigation = nav;
        }
    }

    void Update()
    {
        if (EventSystem.current == null || activeButtons.Count == 0) return;

        // マウスクリック等でフォーカスが外れた場合、有効なボタンへカーソルを戻す
        GameObject current = EventSystem.current.currentSelectedGameObject;
        if (current == null || !current.activeInHierarchy)
        {
            SelectUI(activeButtons[0].gameObject);
        }
    }

    private void SelectUI(GameObject target)
    {
        if (EventSystem.current == null || target == null) return;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(target);
    }
}