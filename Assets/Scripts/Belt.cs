using UnityEngine;

public class Belt : MonoBehaviour
{
    private Animator animator;
    private Vector3 initialScale;
    
    // 親から与えられる現在の状態
    private bool isReversed = false;
    private float currentSpeed = 0f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        initialScale = transform.localScale;
    }

    // 親(BeltConveyor)から呼ばれる：速度の更新
    public void SetSpeed(float speed)
    {
        currentSpeed = speed;
        animator.SetFloat("BeltSpeed", speed); 
    }

    // 親(BeltConveyor)から呼ばれる：向きの更新
    public void SetDirection(bool reversed)
    {
        isReversed = reversed;

        // Scaleを反転させてアニメーションの向きを逆にする
        if (animator != null)
        {
            Vector3 scale = initialScale;
            scale.x = isReversed ? -initialScale.x : initialScale.x;
            transform.localScale = scale;
        }
    }

    // アイテム運搬
    private void OnCollisionStay2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float direction = isReversed ? -currentSpeed : currentSpeed;
            float targetVelocity = currentSpeed * direction;

            rb.linearVelocity = new Vector2(targetVelocity, rb.linearVelocity.y);
        }
    }

    // アイテム落下
    private void OnCollisionExit2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float direction = isReversed ? -0.1f : 0.1f;
            rb.linearVelocity = new Vector2(direction, rb.linearVelocity.y);
        }
    }
}