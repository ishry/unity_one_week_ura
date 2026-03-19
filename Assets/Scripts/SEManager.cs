using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SEManager : MonoBehaviour
{
    public static SEManager Instance { get; private set; }
    
    private AudioSource audioSource;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    // SEを鳴らしたい時はこれを呼ぶ
    public void PlaySE(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}