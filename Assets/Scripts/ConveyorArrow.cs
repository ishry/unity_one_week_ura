using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(RawImage))]
public class ConveyorArrow : MonoBehaviour
{
    private RawImage rawImage;
    private Tweener scrollTweener;

    [Header("スクロール設定")]
    [SerializeField] private float scrollSpeed = 0.5f; // 1秒間に移動するUV座標量

    [Header("反転時の演出設定")]
    // 初期方向のフラグ
    // false: 初期状態, true: 反転状態
    [SerializeField] private bool directionBool = false; 
    // 反転した時の色
    [SerializeField] private Color reverseColor = Color.red; 

    private float currentUvX = 0f;
    private Color initialColor;    // 初期の色を保存する変数
    private Vector3 initialScale; // 初期のScaleを保存する変数

    void Awake()
    {
        rawImage = GetComponent<RawImage>();
    }

    void Start()
    {
        // 1. 初期の演出設定（色とScale）を保存しておく
        initialColor = rawImage.color;
        initialScale = transform.localScale;

        // 2. 現在の directionBool の状態に合わせて，演出を初期化する
        UpdateVisuals();

        // 3. UVスクロールのアニメーションを開始（初期速度1.0f）
        // この演出では，スクロール自体の向きは常に一定
        UpdateAnimation(1.0f);
    }

    // Beltから呼ばれる反転命令
    public void ReverseArrow(float currentBeltSpeed)
    {
        // 1. 方向のフラグを反転
        directionBool = !directionBool;
        
        // 2. 新しいフラグに合わせて，見た目を更新
        UpdateVisuals();

        // 3. 新しいベルト速度に合わせて，UVスクロールの速度（間隔）を更新
        UpdateAnimation(currentBeltSpeed);
    }

    public void UpdateSpeed(float currentBeltSpeed)
    {
        // 向きは変えずに，アニメーションの速度だけ更新する
        UpdateAnimation(currentBeltSpeed); 
    }

    // 見た目（ScaleとColor）を directionBool に合わせて更新する内部用メソッド
    private void UpdateVisuals()
    {
        if (directionBool)
        {
            // Scale.x をマイナスにして，画像を左右反転させる
            Vector3 scale = initialScale;
            scale.x = -initialScale.x;
            transform.localScale = scale;

            // 色を反転時の色（reverseColor）にする
            rawImage.color = reverseColor;
        }
        else
        {
            // Scale と 色 を元に戻す
            transform.localScale = initialScale;
            rawImage.color = initialColor;
        }
    }

    // UVスクロールアニメーションの実際の更新処理
    private void UpdateAnimation(float speedMultiplier)
    {
        // すでに動いているアニメーションがあれば一度止める
        if (scrollTweener != null) scrollTweener.Kill();

        // 画像自体をScaleで反転させるので，UVスクロール自体の向きは常に一定（正の向き：1f）にする
        float direction = 1f;
        
        // ベルトのスピード倍率（speedMultiplier）を反映して，アニメーションの期間を計算
        float duration = 1.0f / (scrollSpeed * speedMultiplier);
        
        float startX = rawImage.uvRect.x;
        float targetX = startX + direction;

        // currentUvX変数をTweenして，毎フレームRawImageのuvRect.xに反映する
        scrollTweener = DOTween.To(
            () => currentUvX,
            x => {
                currentUvX = x;
                Rect rect = rawImage.uvRect;
                rect.x = x;
                rawImage.uvRect = rect;
            },
            targetX,
            duration
        )
        .SetEase(Ease.Linear) // 等速移動
        .SetLoops(-1, LoopType.Restart); // 無限ループ
    }
}