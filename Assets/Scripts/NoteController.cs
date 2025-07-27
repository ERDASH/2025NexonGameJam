using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

[System.Serializable]
public class CarSpriteSet
{
    public string carName;        // 예: "Police", "Taxi"
    public Sprite carSprite;      // 1개의 스프라이트만 사용
}

public class NoteController : MonoBehaviour
{
    [Header("노트 애니메이션 설정")]
    public float openDuration = 0.5f;
    public float closeDuration = 0.5f;
    public float closeOffsetX = 500f;

    public Vector3 openScale = Vector3.one;
    public Vector3 closedScale = Vector3.one;

    private bool isOpen = true;
    private Tween currentTween;
    private Vector3 openPosition;

    [Header("차량 UI 이미지 칸 (3개)")]
    public Image[] carImageSlots = new Image[3];

    [Header("차량 스프라이트 세트")]
    public List<CarSpriteSet> carSpriteSets = new();

    private Dictionary<string, Sprite> carSpriteDict = new();

    void Start()
    {
        openPosition = transform.position;

        // 차량 이름 → 스프라이트 등록
        foreach (var set in carSpriteSets)
        {
            if (!carSpriteDict.ContainsKey(set.carName))
                carSpriteDict.Add(set.carName, set.carSprite);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleNote();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetCarSprite("Police");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetCarSprite("Taxi");
        }
    }

    public void ToggleNote()
    {
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }

        if (!isOpen)
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOMove(openPosition, openDuration).SetEase(Ease.OutExpo));
            seq.Join(transform.DOScale(openScale, openDuration).SetEase(Ease.OutBack));
            currentTween = seq;
        }
        else
        {
            Vector3 targetPos = openPosition + Vector3.left * closeOffsetX;
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOMove(targetPos, closeDuration).SetEase(Ease.InExpo));
            seq.Join(transform.DOScale(closedScale, closeDuration).SetEase(Ease.InBack));
            currentTween = seq;
        }

        isOpen = !isOpen;
    }

    public void SetCarSprite(string carName)
    {
        if (!carSpriteDict.ContainsKey(carName))
        {
            Debug.LogWarning($"[NoteController] 등록되지 않은 차량 이름: {carName}");
            return;
        }

        Sprite sprite = carSpriteDict[carName];

        for (int i = 0; i < carImageSlots.Length; i++)
        {
            if (carImageSlots[i] != null)
                carImageSlots[i].sprite = sprite;
        }

        Debug.Log($"[{carName}] 스프라이트 일괄 적용 완료");
    }
}