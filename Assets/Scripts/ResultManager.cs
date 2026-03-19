using UnityEngine;
using DG.Tweening;
using TMPro; // TMPを扱うために必要
using UnityEngine.SceneManagement; // シーンのリロードに必要
using UnityEngine.InputSystem; // クリック判定に必要

public class ResultManager : MonoBehaviour
{
    [Header("リザルト演出設定")]
    [SerializeField] private GameObject resultTilemapObj;
    [SerializeField] private float dropDuration = 1.5f;
    [SerializeField] private float startY = 15.0f;
    [SerializeField] private float targetY = 0.0f;

    [Header("テキストUI設定")]
    [SerializeField] private TMP_Text resultScoreText;   // スコア表示用TMP
    [SerializeField] private TMP_Text clickToStartText;  // CLICK TO START用TMP
    [SerializeField] private float blinkDuration = 1.0f; // 点滅の周期

    private bool canRestart = false; // クリック入力を受け付けるかどうかのフラグ

    void Start()
    {
        if (resultTilemapObj != null) resultTilemapObj.SetActive(false);

        // 最初はテキストを透明(alpha = 0)にして見えないようにしておく
        if (resultScoreText != null) resultScoreText.alpha = 0f;
        if (clickToStartText != null) clickToStartText.alpha = 0f;
    }

    void Update()
    {
        // 演出がすべて終わるまではクリックを受け付けない
        if (!canRestart) return;

        bool isClicked = false;
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) isClicked = true;
        else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame) isClicked = true;

        if (isClicked)
        {
            // 現在のシーン名を取得して、同じシーンを再読み込み（リロード）する
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // GameManagerから呼ばれる、リザルト演出の開始メソッド（スコアを受け取る）
    public void ShowResult(int finalScore)
    {
        if (resultTilemapObj == null) return;

        // あらかじめテキストにスコアの文字列をセットしておく（まだ透明）
        if (resultScoreText != null)
        {
            resultScoreText.text = "Score: " + finalScore.ToString("00000");
        }

        // リザルト画面を上空にセットして表示
        resultTilemapObj.SetActive(true);
        Vector3 startPos = resultTilemapObj.transform.position;
        startPos.y = startY;
        resultTilemapObj.transform.position = startPos;

        resultTilemapObj.transform.DOMoveY(targetY, dropDuration)
            .SetEase(Ease.OutBounce)
            .OnComplete(() => 
            {
                // バウンドが完全に終わったら、Sequenceで次の演出を順番に実行する
                Sequence seq = DOTween.Sequence();

                // 1. 【追加】バウンド終了後、まずは1秒待つ
                seq.AppendInterval(1.0f);

                // 2. 1秒経ったらスコアをパッと表示
                if (resultScoreText != null)
                {
                    seq.Append(resultScoreText.DOFade(1.0f, 0.0f));
                }

                // 3. 【追加】スコアが表示されてから、さらに1秒待つ
                seq.AppendInterval(0.5f);

                // 4. その後、CLICK TO STARTを点滅させ始め、クリックを許可する
                seq.AppendCallback(() =>
                {
                    if (clickToStartText != null)
                    {
                        clickToStartText.DOFade(1.0f, blinkDuration)
                                        .SetLoops(-1, LoopType.Yoyo)
                                        .SetEase(Ease.InOutSine);
                    }
                    canRestart = true; // ここで初めてクリック可能になる
                });
            });
    }
}