
[System.Serializable]
public class RankingData
{
    public string name;
    public int score;

    public RankingData(string name, int score)
    {
        this.name = name;
        this.score = score;
    }
}