using System.Collections;
using UnityEngine;

public class GameSpeechController : MonoBehaviour
{
    public GameObject targetObject; // 인스펙터에서 드래그 앤 드롭할 오브젝트 (기본 비활성화 상태여야 함)

    void Start()
    {
        StartCoroutine(EnableThenDisable());
    }

    IEnumerator EnableThenDisable()
    {
        yield return new WaitForSeconds(1f); // 1초 기다림
        targetObject.SetActive(true);        // 오브젝트 활성화

        yield return new WaitForSeconds(5f); // 2초 더 기다림
        targetObject.SetActive(false);       // 오브젝트 비활성화
    }
}