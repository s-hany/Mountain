using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioVolume : MonoBehaviour {
    public AudioMixer audioMixer; // オーディオミキサーを登録
    public Slider bGMSlider; // BGMのスライダーを登録
    public Slider sESlider; // SEのスライダーを登録

    public static AudioVolume instance;

    void Awake() {
        CheckInstance();
    }

    void CheckInstance() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        // BGMとSEのスライダーの位置をロード。データがなければ最大値を入れる。
        float bgmSliderPosition = PlayerPrefs.GetFloat("BGM_SLIDER", 5.0f);
        float seSliderPosition = PlayerPrefs.GetFloat("SE_SLIDER", 5.0f);
        // BGMとSEのセットメソッドを呼び出す
        SetBGM(bgmSliderPosition);
        SetSE(seSliderPosition);
    }

    public void SetBGM(float bgmSliderPosition) {
        // スライダーの位置から相対量をdBに変換してvolumeに入れる
        var volume = Mathf.Clamp(Mathf.Log10(bgmSliderPosition / 5) * 20f, -80f, 0f);
        // スライダーの位置のデータとビジュアルを合わせる
        bGMSlider.value = bgmSliderPosition;
        // オーディオミキサーにvolumeの値をセットする。
        audioMixer.SetFloat("BGM", volume);
        // スライダーの位置をセーブする。
        PlayerPrefs.SetFloat("BGM_SLIDER", bGMSlider.value);
        PlayerPrefs.Save();
    }

    public void SetSE(float seSliderPosition) {
        var volume = Mathf.Clamp(Mathf.Log10(seSliderPosition / 5) * 20f, -80f, 0f);
        sESlider.value = seSliderPosition;
        audioMixer.SetFloat("SE", volume);
        PlayerPrefs.SetFloat("SE_SLIDER", sESlider.value);
        PlayerPrefs.Save();
    }
}