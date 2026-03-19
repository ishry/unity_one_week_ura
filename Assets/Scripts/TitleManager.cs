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

    [Header("SE")]
    [SerializeField] private AudioClip gameStartSE;

    private bool isSceneTransitioning = false;
    private bool isOptionOpen = false;

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
        optionPanel.SetActive(false); // 最初はオプションパネルをオフにする
        optionButton.interactable = true;  // 最初はボタンを押せる
        
    }

    void Update()
    {
        // シーン遷移中 or オーディオオプションを開いている時はクリック無効
        if (isSceneTransitioning || isOptionOpen) return;

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
        SEManager.Instance.PlaySE(gameStartSE);
        
        // クリックされたら点滅停止
        if (blinkTweener != null)
        {
            // trueを渡すと、現在のアルファ値で即座に終了する
            blinkTweener.Kill(true); 
            // 念のため、テキストは表示状態にしておく（画面が暗くなる時に一緒に消えるようにするため）
            startText.color = new Color(startText.color.r, startText.color.g, startText.color.b, 1.0f);
        }

        fadeImage.raycastTarget = true;

        fadeImage.DOFade(1.0f, fadeDuration)
                 .OnComplete(() =>
                 {
                     SceneManager.LoadScene(nextSceneName);
                 });
    }

    // ボタンが参照
    public void OpenOptionPanel()
    {
        if (isSceneTransitioning) return;
        isOptionOpen = true;
        if (optionPanel != null) optionPanel.SetActive(true);
        if (optionButton != null) optionButton.interactable = false;
    }

    //　ボタンが参照
    public void CloseOptionPanel()
    {
        isOptionOpen = false;
        if (optionPanel != null) optionPanel.SetActive(false);
        if (optionButton != null) optionButton.interactable = true;
    }
}