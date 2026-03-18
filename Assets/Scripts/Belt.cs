using UnityEngine;

public class Belt : MonoBehaviour
{
    private Animator animator;
    private bool isReversed = false;

    [SerializeField] private GearFlipper gearFlipper_L;
    [SerializeField] private GearFlipper gearFlipper_R;
    [SerializeField] private float scrollSpeed = 1.0f; 
    [SerializeField] private float incrementSpeed = 0.1f;

    private float totalSpeed;

    void Start()
    {
        animator = GetComponent<Animator>();
        totalSpeed = scrollSpeed;
    }

    public void OnClicked()
    {
        if (gearFlipper_L.IsFlipping || gearFlipper_R.IsFlipping)
        {
            return;
        }

        Debug.Log(gameObject.name + " is clicked!"); 
        totalSpeed += incrementSpeed;
        isReversed = !isReversed;
        animator.SetFloat("Speed", isReversed ? -totalSpeed : totalSpeed); 
        gearFlipper_L.speedMagnification = totalSpeed;
        gearFlipper_R.speedMagnification = totalSpeed;
        gearFlipper_L.FlipGear();
        gearFlipper_R.FlipGear();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // 現在の向きに合わせて速度を計算 (反転時はマイナス)
            float direction = isReversed ? -totalSpeed : totalSpeed;
            float targetVelocity = totalSpeed * direction;

            rb.linearVelocity = new Vector2(targetVelocity, rb.linearVelocity.y);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        
        if (rb != null)
        {
            float direction = isReversed ? -0.1f : 0.1f;
            float targetVelocity = totalSpeed * direction;

            rb.linearVelocity = new Vector2(targetVelocity, rb.linearVelocity.y);
        }
        
    }
}