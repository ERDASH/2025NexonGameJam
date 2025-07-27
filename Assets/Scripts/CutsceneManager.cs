using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CutsceneActionType
{
    Entry,
    Exit
}

[System.Serializable]
public class CutsceneGroup
{
    [Tooltip("이 그룹의 CutsceneElement들은 동시에 실행됩니다. Entry: 등장, Exit: 퇴장")]
    public CutsceneActionType actionType = CutsceneActionType.Entry;
    public List<CutsceneElement> elements;
}

public class CutsceneManager : MonoBehaviour
{
    [Header("컷씬 그룹들 (내부는 동시에, 외부는 순차적으로 실행)")]
    public List<CutsceneGroup> cutsceneGroups;

    [Header("컷씬 완료 후 이 오브젝트 비활성화")]
    public bool deactivateOnEnd = true;

    public GameObject fullToon;

    void Start()
    {
        SoundManager.Instance.PlayBGM("Intro");
        StartCoroutine(PlayCutsceneSequence());
    }

    IEnumerator PlayCutsceneSequence()
    {
        int count = 0;
        foreach (var group in cutsceneGroups)
        {
            int completedCount = 0;

            foreach (var element in group.elements)
            {
                count += 1;
                element.Init();
                element.onComplete = () => completedCount++;
                if (group.actionType == CutsceneActionType.Entry)
                    element.PlayEntryAnimation();
                else
                    element.PlayExitAnimation();
            }
            Debug.Log("컷씬 진행 중: "+ count);

            yield return new WaitUntil(() => completedCount >= group.elements.Count);
        }

        Debug.Log("컷씬 전체 완료");
        if (fullToon != null)
            fullToon.SetActive(true);

        if (deactivateOnEnd)
            gameObject.SetActive(false);
    }
}
