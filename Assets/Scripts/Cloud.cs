using UnityEngine;
using DG.Tweening;

public class Cloud : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveDistance = 20f;
    public float moveDuration = 5f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        StartLoop();
    }

    void StartLoop()
    {
        MoveLeft();
    }

    void MoveLeft()
    {
        transform.DOMoveX(startPosition.x - moveDistance, moveDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                // 왼쪽 끝 도달 → 초기 위치로 순간 이동
                transform.position = startPosition;

                // 다시 시작
                MoveLeft();
            });
    }
}
