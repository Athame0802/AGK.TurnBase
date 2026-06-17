using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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

    private void GameStart()
    {
        SceneManager.LoadScene(Scenes.inGame); // 비동기 로드로 변경 필요
    }

    private void GameEnd()
    {
        D.Log("우왕 맞춤");
    }

    public void OnAllCorrectAnswer()
    {
        GameEnd();
    }
}
