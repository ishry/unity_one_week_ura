using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.InputSystem; 
using TMPro; // ▼▼▼ 追加：TextMeshProを扱うために必要 ▼▼▼

public class TitleManager : MonoBehaviour
{
    [Header("フェード設定")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private string nextSceneName = "GameScene";

    [Header("点滅テキスト設定")]
    [SerializeField] private TMP_Text startText; // 「CLICK TO START」のテキスト
    [SerializeField] private float blinkDuration = 1.0f; // 点滅の周期（フェードアウトに1秒、フェードインに1秒）

    private bool isTransitioning = false;
    private Tweener blinkTweener; // ▼▼▼ 追加：点滅アニメーションを管理する変数 ▼▼▼

    void Start()
    {
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.raycastTarget = false;

        // ▼▼▼ 追加：点滅アニメーションを開始 ▼▼▼
        if (startText != null)
        {
            // DOTweenでアルファ値を0（透明）にする
            // .SetLoops(-1, LoopType.Yoyo) で無限に（-1）ヨーヨー（行って戻って）させる
            blinkTweener = startText.DOFade(0.0f, blinkDuration)
                                    .SetLoops(-1, LoopType.Yoyo)
                                    .SetEase(Ease.InOutSine); // スムーズな加減速
        }
    }

    void Update()
    {
        if (!isTransitioning)
        {
            bool isClicked = false;

            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                isClicked = true;
            }
            else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                isClicked = true;
            }

            if (isClicked)
            {
                StartTransition();
            }
        }
    }

    private void StartTransition()
    {
        isTransitioning = true;
        
        // ▼▼▼ 追加：クリックされたら、点滅を停止する ▼▼▼
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
}