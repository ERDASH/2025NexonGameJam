using System.Collections;
using UnityEngine;

public class GameSpeechController : MonoBehaviour
{
    public GameObject targetObject; // �ν����Ϳ��� �巡�� �� ����� ������Ʈ (�⺻ ��Ȱ��ȭ ���¿��� ��)

    void Start()
    {
        StartCoroutine(EnableThenDisable());
    }

    IEnumerator EnableThenDisable()
    {
        yield return new WaitForSeconds(1f); // 1�� ��ٸ�
        targetObject.SetActive(true);        // ������Ʈ Ȱ��ȭ

        yield return new WaitForSeconds(5f); // 2�� �� ��ٸ�
        targetObject.SetActive(false);       // ������Ʈ ��Ȱ��ȭ
    }
}