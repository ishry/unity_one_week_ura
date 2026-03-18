using UnityEngine;
using DG.Tweening;

public class GearFlipper : MonoBehaviour
{
    [Header("アニメーション設定")]
    public float duration = 0.5f;     // 裏返る時間
    public float jumpPower = 1.0f;    // 跳ねる高さ
    public float rotationSpeed = 90f; // 1秒間に回転する角度（Z軸）

    [Header("画像の設定")]
    public SpriteRenderer gearSpriteRenderer;
    public Sprite frontSprite;
    public Sprite backSprite;

    private bool isFlipped = false;
    private float currentYRotation = 0f;
    private float currentZRotation = 0f;
    public bool IsFlipping { get; private set; } = false;

    [HideInInspector] public float speedMagnification = 1.0f; //速度倍率．Beltから与えられる．

    void Update()
    {
        // 1. Z軸の継続的な回転（マイナスにすることで表から見て右回転）
        currentZRotation -= rotationSpeed * speedMagnification * Time.deltaTime;
        currentZRotation %= 360f; // 値が無限に大きくならないようループさせる

        // 2. Y軸（裏返し）とZ軸（継続回転）の角度を合成して適用
        transform.localRotation = Quaternion.Euler(0f, currentYRotation, currentZRotation);
    }

    public void FlipGear()
    {
        if (IsFlipping) return; // アニメーション中の連続クリックを防ぐ
        IsFlipping = true;

        transform.DOKill(false); // 実行中のDOJumpがあればリセット
        Sequence sequence = DOTween.Sequence();

        // 位置のジャンプ（Positionの変更なので回転処理とは競合しません）
        sequence.Join(transform.DOJump(transform.position, jumpPower, 1, duration));

        // Y軸の回転は、Transformではなく「currentYRotation」という変数の数値をTweenさせる
        float targetYRotation = currentYRotation + 180f;
        sequence.Join(
            DOTween.To(
                () => currentYRotation,         // 現在の値
                x => currentYRotation = x,      // 更新処理
                targetYRotation,                // 目標の値
                duration                        // 時間
            ).SetEase(Ease.OutBack)             // ここで弾力のある動きを適用
        );

        // アニメーションの半分（90度横を向いた瞬間）で画像を切り替え
        Sprite nextSprite = isFlipped ? frontSprite : backSprite;
        sequence.InsertCallback(duration / 2f, () =>
        {
            gearSpriteRenderer.sprite = nextSprite;
        });

        // アニメーション完了時にフラグを戻す
        sequence.OnComplete(() =>
        {
            IsFlipping = false;
        });

        isFlipped = !isFlipped;
    }
}