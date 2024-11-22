using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillCountManager : MonoBehaviour
{

    public static KillCountManager instance;

    public static int score, high_Score;

    [SerializeField]
    Text kill_Count;

    private void Awake()
    {
       MakeInstance();
       score = 0;
    }

    void Start()
    {
        
    }

  
    void Update()
    {
        
    }


    void MakeInstance()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void AddScore()
    {
        score++;
        kill_Count.text = score + "";
        UpdateHighScore();
    }


    public void UpdateHighScore()
    {
        if (score > high_Score) high_Score = score;
        print(high_Score);
    }


    public int GetScore()
    {
        return score;
    }

    public int GetHighScore()
    {
        return high_Score;
    }



}
