using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public float volume;

    public List<Sound> sounds = new();
    public List<AudioClip> clips = new();

    public AudioSource prefab;

    [SerializeField]
    AudioSource musicSource;
    float startingmusicvolume;
    Slider slider;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);

            return;
        }
        Instance = this;
        startingmusicvolume = musicSource.volume;
        DontDestroyOnLoad(gameObject);
        slider = FindFirstObjectByType<Slider>();
        SceneManager.sceneLoaded += (s, l) =>
        {
            slider = FindFirstObjectByType<Slider>();
            if (slider != null)
                slider.value = volume;
        };

    }
    private void Update()
    {
        musicSource.volume = startingmusicvolume * volume;
        if (slider != null)
        {
            volume = slider.value;
        }
    }

    

    public enum Sound
    {
        Thud,
        Cast,
        Hurt,
        Open,
        Close
    }

    public void PlaySound(float volume, Vector3 point, Sound sound)
    {
        AudioSource s = Instantiate(prefab, point, Quaternion.identity);
        s.volume = this.volume * volume;
        s.clip = clips[sounds.IndexOf(sound)];
        s.pitch *= Random.Range(.9f, 1.1f);
        s.Play();
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(5f);
        sequence.AppendCallback(() => Destroy(s.gameObject));
    }
}
