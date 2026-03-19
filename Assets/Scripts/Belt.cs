using UnityEngine;
using UniRx;

public class Belt : MonoBehaviour
{
    private Animator animator;
    private bool isReversed = false;

    [SerializeField] private GearFlipper gearFlipper_L;
    [SerializeField] private GearFlipper gearFlipper_R;
    [SerializeField] private float scrollSpeed = 1.0f; 
    [SerializeField] private float incrementSpeed = 0.1f;

    public static ReactiveProperty<float> SharedSpeed;
    //private float totalSpeed;

    [HideInInspector] public bool canClick;

    [SerializeField] private ConveyorArrow conveyorArrow;

    [SerializeField] private AudioClip clickSE;

    void Awake()
    {
        // 一番最初に誕生したベルトが初期化を行う
        if (SharedSpeed == null)
        {
            SharedSpeed = new ReactiveProperty<float>(scrollSpeed);
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        canClick = false; // 最初はoffにしておいて，game managaerが遅延してOnにする．

        // SharedSpeed の値が変化した時，自動的に UpdateAnimationSpeed を呼ぶ
        SharedSpeed.Subscribe(speed => 
            {
                UpdateAnimationSpeed(speed);
            }).AddTo(this);
    }

    public void OnClicked()
    {
        // ゲーム開始直後はクリックできない
        if (!canClick) return;

        // フリップアニメーション中はクリックできない
        if (gearFlipper_L.IsFlipping || gearFlipper_R.IsFlipping) return;

        //SE
        SEManager.Instance.PlaySE(clickSE);

        // 1. 【自分自身】の反転処理を行う
        isReversed = !isReversed;
        gearFlipper_L.FlipGear();
        gearFlipper_R.FlipGear();

        // （矢印の見た目を反転させる。速度は次のスピードを予測して渡す）
        conveyorArrow.ReverseArrow(SharedSpeed.Value + incrementSpeed);

        // 2. 共有スピードをアップ
        // 💡 ここで数値を足した瞬間、全ベルトの Subscribe が一斉に発火し、アニメーションが加速します！
        SharedSpeed.Value += incrementSpeed;
    }

    private void UpdateAnimationSpeed(float speed)
    {
        // 自分の回転の向き（isReversed）を考慮してアニメーション速度をセット
        animator.SetFloat("Speed", isReversed ? -speed : speed); 

        // ギアの速度をセット
        gearFlipper_L.speedMagnification = speed;
        gearFlipper_R.speedMagnification = speed;

        // 矢印の速度をセット
        if (conveyorArrow != null)
        {
            conveyorArrow.UpdateSpeed(speed);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // ▼ 変更：totalSpeed を SharedSpeed.Value に置き換え
            float direction = isReversed ? -SharedSpeed.Value : SharedSpeed.Value;
            float targetVelocity = SharedSpeed.Value * direction;

            rb.linearVelocity = new Vector2(targetVelocity, rb.linearVelocity.y);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        
        if (rb != null)
        {
            float direction = isReversed ? -0.1f : 0.1f;
            float targetVelocity = direction;

            rb.linearVelocity = new Vector2(targetVelocity, rb.linearVelocity.y);
        }
        
    }
}