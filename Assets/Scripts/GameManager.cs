using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // 
    [Header("Game Data")]
    public int score = 0;
    public int life = 3;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int value)
    {
        score += value;
    }

    public void AddLife(int value)
    {
        life += value;
    }

    public int GetScore() => score;
    public int GetLife() => life;
}
