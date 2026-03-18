using UnityEngine;

public class Belt : MonoBehaviour
{
    private Animator animator;
    private bool isReversed = false;

    [SerializeField] private GearFlipper gearFlipper_L;
    [SerializeField] private GearFlipper gearFlipper_R;
    [SerializeField] private float scrollSpeed = 2.0f; 

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnClicked()
    {
        if (gearFlipper_L.IsFlipping || gearFlipper_R.IsFlipping)
        {
            return;
        }
        
        Debug.Log(gameObject.name + " is clicked!"); 
        isReversed = !isReversed;
        animator.SetFloat("Speed", isReversed ? -1f : 1f); 
        gearFlipper_L.FlipGear();
        gearFlipper_R.FlipGear();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // 現在の向きに合わせて速度を計算 (反転時はマイナス)
            float direction = isReversed ? -1f : 1f;
            float targetVelocity = scrollSpeed * direction;

            rb.linearVelocity = new Vector2(targetVelocity, rb.linearVelocity.y);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        
        if (rb != null)
        {
            float direction = isReversed ? -0.1f : 0.1f;
            float targetVelocity = scrollSpeed * direction;

            rb.linearVelocity = new Vector2(targetVelocity, rb.linearVelocity.y);
        }
        
    }
}