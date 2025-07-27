using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;

    public Button rankClearBtn;
    public Button stageClearBtn;

    void Start()
    {

        if (rankClearBtn != null) rankClearBtn.onClick.AddListener(() => ClearRank());
        if (stageClearBtn != null) stageClearBtn.onClick.AddListener(() => ClearStage());

        float savedBGM = PlayerPrefs.GetFloat("BGMVolume", 1f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // 초기 슬라이더 값 세팅 (현재 볼륨 기준)
        if (SoundManager.Instance != null)
        {
            bgmSlider.value = SoundManager.Instance.bgmSource.volume;
            sfxSlider.value = SoundManager.Instance.sfxSource.volume;
        }

        SetBGMVolume(savedBGM);
        SetSFXVolume(savedSFX);

        // 슬라이더 변경 시 볼륨 반영
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetBGMVolume(float value)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.bgmSource.volume = value;
            PlayerPrefs.SetFloat("BGMVolume", value);
        }
    }

    public void SetSFXVolume(float value)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.sfxSource.volume = value;
            PlayerPrefs.SetFloat("SFXVolume", value);
        }
    }

    public void ClearRank()
    {
         string rankKeyPrefix = "Rank_";
        for (int i = 0; i < 10; i++)
        {
            PlayerPrefs.DeleteKey(rankKeyPrefix + i);
            PlayerPrefs.DeleteKey(rankKeyPrefix + i + "_Name");
            PlayerPrefs.DeleteKey(rankKeyPrefix + i + "_Score");
        }
    }

    public void ClearStage()
    {
        global.stage = 1;
        global.stageNow = 0;
    }
}
