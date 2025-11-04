using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("오디오 소스")]
    public AudioSource bgmSource; // 배경음악
    public AudioSource sfxSource; // 효과음

    [Header("배경음 리스트")]
    public List<SoundData> bgmList = new();

    [Header("효과음 리스트")]
    public List<SoundData> soundList = new();

    private Dictionary<string, AudioClip> bgmDict = new();
    private Dictionary<string, AudioClip> soundDict = new();
    

    void Awake()
    {
        // 싱글톤 설정
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 리스트 → 딕셔너리로 변환
        foreach (var sound in soundList)
        {
            if (!soundDict.ContainsKey(sound.name))
                soundDict.Add(sound.name, sound.clip);
        }

        foreach (var sound in bgmList)
        {
            if (!bgmDict.ContainsKey(sound.name))
                bgmDict.Add(sound.name, sound.clip);
        }
    }

    // 디버깅 삭제
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            PlaySFX("Bike");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            PlaySFX("DownArrow");
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlaySFX("Button");
        }
    }

    /* 배경음악 재생 **/
    /*
    public void PlayBGM(string name, bool loop = true)
    {
        if (bgmSource == null) return;

        // 이미 플레이 중인 거 끄기
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }

        bgmSource.clip = bgmDict[name];
        bgmSource.loop = loop;
        bgmSource.Play();
    }
    */
    public void PlayBGM(string name, bool loop = true)
    {
        if (bgmSource == null || !bgmDict.ContainsKey(name))
            return;

        // 🎵 이미 같은 곡이 재생 중이면 다시 틀지 않음
        if (bgmSource.isPlaying && bgmSource.clip == bgmDict[name])
            return;

        // 🔇 다른 곡이 재생 중이면 중단
        bgmSource.Stop();

        // ▶️ 새 곡 재생
        bgmSource.clip = bgmDict[name];
        bgmSource.loop = loop;
        bgmSource.Play();
    }



    /* 배경음악 재생 **/
    public void StopBGM(string name, bool loop = true)
    {
        if (bgmSource == null) return;

        bgmSource.clip = bgmDict[name];
        bgmSource.loop = loop;
        bgmSource.Stop();
    }

    /* 효과음 재생 **/
    public void PlaySFX(string name)
    {
        if (sfxSource == null || !soundDict.ContainsKey(name)) return;

        sfxSource.PlayOneShot(soundDict[name]);
    }
}

[System.Serializable]
public class SoundData
{
    public string name;
    public AudioClip clip;
}
