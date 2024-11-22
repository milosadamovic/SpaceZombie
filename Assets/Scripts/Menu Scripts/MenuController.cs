using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    [SerializeField]
    private Text score, high_Score;

    private void Start()
    {
        score.text = PlayerPrefs.GetString("score");
        high_Score.text = PlayerPrefs.GetString("highScore");
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("LunarLandscape3D");
    }

    public void ExitGame()
    {
        Application.Quit();
    }












} // class
