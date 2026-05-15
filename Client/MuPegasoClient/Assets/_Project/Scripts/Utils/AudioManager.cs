using UnityEngine;

namespace MuPegaso.Client
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Sources")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("BGM")]
        [SerializeField] private AudioClip loginBGM;
        [SerializeField] private AudioClip worldBGM;

        [Header("SFX")]
        [SerializeField] private AudioClip btnClick;
        [SerializeField] private AudioClip loginSuccess;

        [Header("Config")]
        [SerializeField] [Range(0,1)] private float bgmVolume = 0.6f;
        [SerializeField] [Range(0,1)] private float sfxVolume = 1.0f;

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            bgmSource.volume = bgmVolume;
            sfxSource.volume = sfxVolume;
            bgmSource.loop   = true;
        }

        public void PlayBGM(AudioClip clip)
        {
            if (clip == null || bgmSource.clip == clip) return;
            bgmSource.clip = clip;
            bgmSource.Play();
        }

        public void PlayLoginBGM() => PlayBGM(loginBGM);
        public void PlayWorldBGM() => PlayBGM(worldBGM);

        public void PlaySFX(AudioClip clip)
        {
            if (clip == null) return;
            sfxSource.PlayOneShot(clip, sfxVolume);
        }

        public void PlayClick()    => PlaySFX(btnClick);
        public void PlayLoginOK()  => PlaySFX(loginSuccess);

        public void StopBGM()      => bgmSource.Stop();
        public void SetBGMVolume(float v)  => bgmSource.volume = v;
        public void SetSFXVolume(float v)  => sfxSource.volume = v;
    }
}
