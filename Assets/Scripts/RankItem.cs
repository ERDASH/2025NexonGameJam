using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro 사용 시

public class RankItem : MonoBehaviour
{
    public TMP_Text rankText;
    public TMP_Text nameText;
    public TMP_Text scoreText;

    public void SetData(string name, int score)
    {
        nameText.text = name;
        scoreText.text = score.ToString();
    }

    public void SetRank(string rank)
    {
        rankText.text = rank;
    }
}