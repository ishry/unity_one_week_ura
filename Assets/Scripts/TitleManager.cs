using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.InputSystem; 
using TMPro;
using UnityEngine.EventSystems;

public class TitleManager : MonoBehaviour
{
    [Header("フェード設定")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private string nextSceneName = "GameScene";

    [Header("点滅テキスト設定")]
    [SerializeField] private TMP_Text startText; // 「CLICK TO START」のテキスト
    [SerializeField] private float blinkDuration = 1.0f; // 点滅の周期（フェードアウトに1秒，フェードインに1秒）

    [Header("音量オプション")]
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private Button optionButton;

    [Header("遊び方オプション")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private TMP_Text tutorialButtonText;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color disabledColor = Color.gray;

    [Header("SE")]
    [SerializeField] private AudioClip gameStartSE;

    private bool isSceneTransitioning = false;
    private bool isOptionOpen = false;
    private bool isTutorialOpen = false;

    private Tweener blinkTweener;   

    void Start()
    {
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.raycastTarget = false;

        // 点滅アニメーションを開始
        if (startText != null)
        {
            blinkTweener = startText.DOFade(0.0f, blinkDuration)
                                    .SetLoops(-1, LoopType.Yoyo)
                                    .SetEase(Ease.InOutSine);
        }        
        if (optionPanel != null) optionPanel.SetActive(false);
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
        SetButtonsInteractable(true);
        
    }

    void Update()
    {
        // シーン遷移中 or オーディオオプションを開いている時はクリック無効
        if (isSceneTransitioning || isOptionOpen || isTutorialOpen) return;

        // マウスが接続されていて，左クリックが押されたフレームのみ判定
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // UI(音量オプションボタン)じゃない部分をクリックした場合はシーン遷移開始
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                StartTransition();
            }
        }
    }

    private void StartTransition()
    {
        isSceneTransitioning = true;
        SetButtonsInteractable(false);
        SEManager.Instance.PlaySE(gameStartSE);
        
        // クリックされたら点滅停止
        if (blinkTweener != null)
        {
            blinkTweener.Kill(true); 
            startText.color = new Color(startText.color.r, startText.color.g, startText.color.b, 1.0f);
        }

        fadeImage.raycastTarget = true;

        fadeImage.DOFade(1.0f, fadeDuration)
                 .OnComplete(() =>
                 {
                     SceneManager.LoadScene(nextSceneName);
                 });
    }

    private void SetButtonsInteractable(bool state)
    {
        if (optionButton != null) optionButton.interactable = state;
        if (tutorialButton != null) tutorialButton.interactable = state;
        if (tutorialButtonText != null) 
        {
            tutorialButtonText.color = state ? normalColor : disabledColor;
        }
    }

    // 以下ボタンが参照する関数

    public void OpenOptionPanel()
    {
        if (isSceneTransitioning) return;
        isOptionOpen = true;
        if (optionPanel != null) optionPanel.SetActive(true);
        SetButtonsInteractable(false); // パネルを開いたらボタンを両方無効化
    }

    public void CloseOptionPanel()
    {
        isOptionOpen = false;
        if (optionPanel != null) optionPanel.SetActive(false);
        SetButtonsInteractable(true); // パネルを閉じたらボタンを両方復活
    }

    public void OpenTutorialPanel()
    {
        if (isSceneTransitioning) return;
        isTutorialOpen = true;
        if (tutorialPanel != null) tutorialPanel.SetActive(true);
        SetButtonsInteractable(false); // パネルを開いたらボタンを両方無効化
    }

    public void CloseTutorialPanel()
    {
        isTutorialOpen = false;
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
        SetButtonsInteractable(true); // パネルを閉じたらボタンを両方復活
    }
}