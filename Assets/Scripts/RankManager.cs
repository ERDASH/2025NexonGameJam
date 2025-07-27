using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class RankingManager : MonoBehaviour
{
    public int maxRankCount = 10;
    private string rankKeyPrefix = "Rank_";

    [Header("UI Panel")]
    public GameObject inputRankPanel; // 랭킹 입력 패널
    public GameObject rankViewPanel; // 랭킹 보기 패널

    [Header("Input UI")]
    public TMP_InputField inputName; // 이름 입력칸
    public TMP_Text scoreText; // 점수
    public Button saveButton; // 저장 버튼
    public Button exitButton; // 나가기 버튼

    [Header("View UI")] 
    public Transform contentParent; // 스크롤 뷰 content
    public GameObject rankItemPrefab; // RankItem 프리팹
    public Button exitViewButton; // 나가기 버튼

    public List<RankingData> rankingList = new();

    void Start()
    {
        if (exitButton != null) exitButton.onClick.AddListener(() => CloseInputPanel());
        if (saveButton != null) saveButton.onClick.AddListener(() => InputRanking());
        if (exitViewButton != null) exitViewButton.onClick.AddListener(() => CloseViewPanel());
    }

    public void SaveScore(string playerName, int score)
    {
        List<RankingData> rankingList = LoadScores();

        // 새 점수 추가
        rankingList.Add(new RankingData(playerName, score));

        // 내림차순 정렬
        rankingList.Sort((a, b) => b.score.CompareTo(a.score));

        // 상위 랭크 수만 저장
        for (int i = 0; i < Mathf.Min(rankingList.Count, maxRankCount); i++)
        {
            PlayerPrefs.SetString(rankKeyPrefix + i + "_Name", rankingList[i].name);
            PlayerPrefs.SetInt(rankKeyPrefix + i + "_Score", rankingList[i].score);
        }
        Debug.Log("랭크 등록 완료: " + playerName + " 점수: " + score);
        PlayerPrefs.Save();
    }


    public List<RankingData> LoadScores()
    {
        List<RankingData> result = new();

        for (int i = 0; i < maxRankCount; i++)
        {
            string nameKey = rankKeyPrefix + i + "_Name";
            string scoreKey = rankKeyPrefix + i + "_Score";

            if (PlayerPrefs.HasKey(nameKey) && PlayerPrefs.HasKey(scoreKey))
            {
                string name = PlayerPrefs.GetString(nameKey);
                int score = PlayerPrefs.GetInt(scoreKey);
                result.Add(new RankingData(name, score));
            }
        }

        return result;
    }

    public void ResetRanking()
    {
        for (int i = 0; i < maxRankCount; i++)
        {
            PlayerPrefs.DeleteKey(rankKeyPrefix + i);
            PlayerPrefs.DeleteKey(rankKeyPrefix + i + "_Name");
            PlayerPrefs.DeleteKey(rankKeyPrefix + i + "_Score");
        }

        PlayerPrefs.Save();
        rankingList.Clear();

        // UI도 초기화
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        Debug.Log("랭킹이 초기화되었습니다.");
        PlayerPrefs.Save();
    }

    // 최고 점수 업데이트
    public void UpdateScore()
    {
        scoreText.text = GameManager.Instance.GetScore() + "점";
    }

    // 랭킹 표시함.
    public void RenderRanking(string newPlayerName = null, int newScore = -1)
    {
        // 1. 기존 랭킹 불러오기
        List<RankingData> latest = LoadScores();

        // 2. 새로운 점수가 있을 경우 추가
        if (!string.IsNullOrEmpty(newPlayerName) && newScore >= 0)
        {
            // 기존 동일 이름 제거
            latest.RemoveAll(data => data.name == newPlayerName);
            latest.Add(new RankingData(newPlayerName, newScore));
        }

        // 3. 내림차순 정렬 및 상위만 유지
        latest.Sort((a, b) => b.score.CompareTo(a.score));
        if (latest.Count > maxRankCount)
            latest = latest.GetRange(0, maxRankCount);

        // 4. 리스트를 클래스 필드에도 반영
        rankingList = latest;

        // 5. PlayerPrefs에 최신화 저장
        for (int i = 0; i < rankingList.Count; i++)
        {
            PlayerPrefs.SetString(rankKeyPrefix + i + "_Name", rankingList[i].name);
            PlayerPrefs.SetInt(rankKeyPrefix + i + "_Score", rankingList[i].score);
        }
        PlayerPrefs.Save();

        // 6. 기존 UI 제거
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // 7. 리스트를 UI에 표시
        for (int i = 0; i < rankingList.Count;i++)
        {
            GameObject item = Instantiate(rankItemPrefab, contentParent);
            RankItem ri = item.GetComponent<RankItem>();
            ri.SetData(rankingList[i].name, rankingList[i].score);
            ri.SetRank((i + 1) + "위");
        }
    }


    public void OpenInputPanel()
    {
        inputRankPanel.SetActive(true);
        UpdateScore();
    }

    public void CloseInputPanel()
    {
        inputRankPanel.SetActive(false);
        OpenViewPanel();
    }

    public void OpenViewPanel()
    {
        rankViewPanel.SetActive(true);
        RenderRanking();
    }

    public void CloseViewPanel()
    {
        rankViewPanel.SetActive(false);
    }

    // 랭킹 입력함.
    public void InputRanking()
    {
        SaveScore(inputName.text, GameManager.Instance.score);
        CloseInputPanel();
        OpenViewPanel();
    }
}
