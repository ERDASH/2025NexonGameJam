using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class scr_combo : MonoBehaviour
{
    public TextMeshProUGUI comboText;      // 드래그앤드롭할 텍스트

    float depth = 10f;          // 시작 depth
    int comboCountDisplay;             // 생성 시 콤보값 고정

    void Start()
    {
        Debug.Log("Combo Created");
        // 생성 시 콤보값 고정
        comboCountDisplay = global.comboCount;

        if (comboText != null)
            comboText.text = comboCountDisplay.ToString()+"Combo";

        // 시작 depth 설정
        Vector3 p = transform.position;
        p.z = depth;
        transform.position = p;

        // 등장할 때 한 번 뽈록
        transform.localScale = Vector3.one * 1.3f;
    }

    void Update()
    {
        // depth 조금씩 감소 → 나중에 생긴 애가 더 앞
        depth -= 0.00001f;
        Vector3 p = transform.position;
        p.z = depth;
        p.y += 0.5f * Time.deltaTime;   // 천천히 위로 올라가기
        transform.position = p;

        // 점점 투명해지기
        if (comboText != null)
        {
            Color c = comboText.color;
            c.a -= 0.7f * Time.deltaTime;   // 속도는 취향껏 조절
            comboText.color = c;

            // 다 투명해지면 삭제
            if (c.a <= 0f)
                Destroy(gameObject);
        }

        // 뽈록 → 서서히 원래 크기(1)로 복귀
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            Vector3.one,
            Time.deltaTime * 10f      // 숫자 키우면 더 빨리 원래 크기로
        );
    }
}
