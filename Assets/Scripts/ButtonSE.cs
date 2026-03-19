using UnityEngine;

public class ButtonSE : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;

    public void PlaySE()
    {
        SEManager.Instance.PlaySE(audioClip);
    }
}
