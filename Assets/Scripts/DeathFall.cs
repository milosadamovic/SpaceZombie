using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathFall : MonoBehaviour
{


    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine("LoadMenuScene");
    }

    IEnumerator LoadMenuScene()
    {
        audioSource.Play();
        yield return new WaitForSeconds(6f);
        PlayerPrefs.SetString("score", KillCountManager.instance.GetScore().ToString());
        PlayerPrefs.SetString("highScore", KillCountManager.instance.GetHighScore().ToString());
        SceneManager.LoadScene("SampleScene");
    }

}
