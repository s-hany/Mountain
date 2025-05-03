using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioSource bgmAudioSource;
    [SerializeField] AudioSource seAudioSource;

    public static SoundManager instance;
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

    void Start() {
        DontDestroyOnLoad(gameObject);//シーン遷移しても破棄されない
    }

    /// <summary>
    /// BGMを再生
    /// </summary>
    /// <param name="clip"></param>
    public void PlayBgm(AudioClip clip) {
        bgmAudioSource.clip = clip;
        bgmAudioSource.Play();
    }

    /// <summary>
    /// BGMを停止
    /// </summary>
    public void StopBgm() {
        bgmAudioSource.Stop();
    }

    /// <summary>
    /// 効果音を再生
    /// </summary>
    /// <param name="clip"></param>
    public void PlaySe(AudioClip clip) {
        seAudioSource.PlayOneShot(clip);
    }

    /// <summary>
    /// 引数のAudioClipの中からランダムに再生
    /// </summary>
    /// <param name="clips"></param>

    public void RandomizeSfx(params AudioClip[] clips) {
        var randomIndex = UnityEngine.Random.Range(0, clips.Length);
        seAudioSource.PlayOneShot(clips[randomIndex]);
    }
}