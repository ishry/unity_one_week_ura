using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("UI Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;

    // パラメーター名を定数で定義（ミスを防ぐため）
    private const string MasterVolumeKey = "MasterVolume";
    private const string BGMVolumeKey = "BGMVolume";
    private const string SEVolumeKey = "SEVolume";

    void Start()
    {
        // 1. 保存されている音量を読み込む（なければデフォルトの 1.0f にする）
        float masterVol = PlayerPrefs.GetFloat(MasterVolumeKey, 1.0f);
        float bgmVol = PlayerPrefs.GetFloat(BGMVolumeKey, 1.0f);
        float seVol = PlayerPrefs.GetFloat(SEVolumeKey, 1.0f);

        // 2. スライダーの位置を読み込んだ値に合わせる
        if (masterSlider != null) masterSlider.value = masterVol;
        if (bgmSlider != null) bgmSlider.value = bgmVol;
        if (seSlider != null) seSlider.value = seVol;

        // 3. 実際のミキサーに音量を反映させる
        SetMasterVolume(masterVol);
        SetBGMVolume(bgmVol);
        SetSEVolume(seVol);

        // 4. スライダーを動かした時にメソッドが呼ばれるように設定
        if (masterSlider != null)
        {
            masterSlider.onValueChanged.AddListener(SetMasterVolume);
        }
        if (bgmSlider != null)
        {
            bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        }
        if (seSlider != null)
        {
            seSlider.onValueChanged.AddListener(SetSEVolume);
        }
    }

    // ▼ マスター音量を変更するメソッド
    public void SetMasterVolume(float value)
    {
        // 魔法の計算式： Mathf.Log10(value) * 20 でデシベルに変換
        audioMixer.SetFloat(MasterVolumeKey, Mathf.Log10(value) * 20);
        // 設定を保存
        PlayerPrefs.SetFloat(MasterVolumeKey, value);
        PlayerPrefs.Save();
    }

    // ▼ BGM音量を変更するメソッド
    public void SetBGMVolume(float value)
    {
        audioMixer.SetFloat(BGMVolumeKey, Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(BGMVolumeKey, value);
        PlayerPrefs.Save();
    }

    // ▼ SE音量を変更するメソッド
    public void SetSEVolume(float value)
    {
        audioMixer.SetFloat(SEVolumeKey, Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(SEVolumeKey, value);
        PlayerPrefs.Save();
    }
}