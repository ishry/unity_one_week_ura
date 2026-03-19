using UnityEngine;
using UnityEngine.SceneManagement; 

[RequireComponent(typeof(AudioSource))]
public class BGMManager : MonoBehaviour
{
    private static BGMManager instance;
    private AudioSource audioSource; 

    [Header("BGMリスト")]
    [SerializeField] private AudioClip titleBGM;
    [SerializeField] private AudioClip gameBGM;   

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    // オブジェクトが有効になった時に「シーン読み込みイベント」に登録する
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // オブジェクトが無効になった時はイベント登録を解除する（エラー防止の鉄則）
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // シーンがロードされるたびに自動で呼ばれるメソッド
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 読み込まれたシーンの名前によってBGMを出し分ける
        if (scene.name == "TitleScene") 
        {
            PlayBGM(titleBGM);
        }
        else if (scene.name == "GameScene")
        {
            PlayBGM(gameBGM);
        }
    }

    private void PlayBGM(AudioClip nextBGM)
    {
        //同じシーンならBGM変更なし
        if (audioSource.clip == nextBGM) return;

        // 新しい曲をセットして再生
        audioSource.clip = nextBGM;
        audioSource.Play();
    }
}