using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Game Manager 单例
    public static GameManager instance;

    // 游戏暂停状态
    public bool pauseState = false;

    // 金币得分
    public int coin = 0;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            if(instance != this)
            {
                Destroy(gameObject);
            }
        }

        DontDestroyOnLoad(gameObject);
    }

    public void PauseGame()
    {
        pauseState = !pauseState;

        if (pauseState)
        {
            Time.timeScale = 0;
            Debug.Log("游戏暂停");
        }
        else
        {
            Time.timeScale = 1;
            Debug.Log("游戏开始");
        }
    }
}
