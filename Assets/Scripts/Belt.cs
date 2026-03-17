using UnityEngine;

public class Belt : MonoBehaviour
{
    private Animator animator;
    private bool isReversed = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnClicked()
    {
        Debug.Log(gameObject.name + " is clicked!"); 
        isReversed = !isReversed;
        animator.SetFloat("Speed", isReversed ? -1f : 1f); 
    }
}