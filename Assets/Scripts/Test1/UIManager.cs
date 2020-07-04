using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text coinText;

    public Button pauseButton;

    public GameObject player;

    public GameObject[] coins;

    // Update is called once per frame
    void Update()
    {
        // 设置 金币得分
        coinText.text = GameManager.instance.coin.ToString();
    }

    // 游戏暂停按钮触发
    public void Pause()
    {
        GameManager.instance.PauseGame();

        if (GameManager.instance.pauseState)
        {
            pauseButton.GetComponentInChildren<Text>().text = "继续";
        }
        else
        {
            pauseButton.GetComponentInChildren<Text>().text = "暂停";
        }
    }

    // 游戏保存按钮触发
    public void Save()
    {
        // Save Data
        //SavePlayerPrefs();
        //SaveSerializes();
        SaveJsonData();
    }

    // 游戏加载按钮触发
    public void Load()
    {
        // Load Data
        //LoadPlayerPrefs();
        //LoadSerializes();
        LoadJsonData();
    }

    #region 以PlayerPrefs的方式存储数据
    private void SavePlayerPrefs()
    {
        GameObject player = GameObject.FindWithTag("Player");

        PlayerPrefs.SetInt("coin", GameManager.instance.coin);

        PlayerPrefs.SetFloat("posX", player.GetComponent<PlayerController>().playerPos.x);
        PlayerPrefs.SetFloat("posZ", player.GetComponent<PlayerController>().playerPos.z);
    }

    private void LoadPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("coin"))
            GameManager.instance.coin = PlayerPrefs.GetInt("coin");

        if (PlayerPrefs.HasKey("posX") && PlayerPrefs.HasKey("posZ"))
        {
            float x = PlayerPrefs.GetFloat("posX");
            float z = PlayerPrefs.GetFloat("posZ");

            GameObject player = GameObject.FindWithTag("Player");

            player.transform.position = new Vector3(x, 0.5f, z);
        }
    }
    #endregion

    #region 以Serialization的方式存储数据

    // 创建 Save 数据对象， 保存数据
    private Save CreateSaveObject()
    {
        Save save = new Save();

        save.coin = GameManager.instance.coin;

        save.playerPosX = player.transform.position.x;
        save.playerPosY = player.transform.position.y;
        save.playerPosZ = player.transform.position.z;

        foreach (var coin in coins)
        {
            CoinState coinState = new CoinState();

            coinState.x = coin.transform.position.x;
            coinState.y = coin.transform.position.y;
            coinState.z = coin.transform.position.z;

            coinState.isDead = coin.GetComponent<Coin>().isDead;

            save.coins.Add(coinState);
        }

        return save;
    }

    private void LoadSaveObject(Save save)
    {
        #region 加载存档数据
        GameManager.instance.coin = save.coin;
        player.transform.position = new Vector3(save.playerPosX, save.playerPosY, save.playerPosZ);

        for (int i = 0; i < coins.Length; i++)
        {
            coins[i].GetComponent<Coin>().isDead = save.coins[i].isDead;

            if (!save.coins[i].isDead)
                coins[i].SetActive(true);

            Vector3 coin_pos = new Vector3(save.coins[i].x, save.coins[i].y, save.coins[i].z);

            coins[i].transform.position = coin_pos;
        }

        Debug.Log("加载存档成功");
        #endregion
    }

    // 序列化 save 对象，保存在SaveData目录
    private void SaveSerializes()
    {
        Save save = CreateSaveObject();

        FileStream fs;

        if (File.Exists(Application.dataPath + "/SaveData/save.data"))
        {
            fs = File.Open(Application.dataPath + "/SaveData/save.data", FileMode.Open);
            Debug.Log("存档成功");
        }
        else
        {
            fs = File.Create(Application.dataPath + "/SaveData/save.data");
            Debug.Log("新建数据文件，并存档成功");
        }

        BinaryFormatter bf = new BinaryFormatter();

        bf.Serialize(fs, save);

        fs.Close();
    }

    // 反序列化， 把在 save.data 文件中的序列化对象中加载数据
    private void LoadSerializes()
    {
        // 判断是否存在存档文件再进行操作
        if (File.Exists(Application.dataPath + "/SaveData/save.data"))
        {
            // 打开存档数据
            FileStream fs = File.OpenRead(Application.dataPath + "/SaveData/save.data");

            BinaryFormatter bf = new BinaryFormatter();

            Save save = bf.Deserialize(fs) as Save;

            LoadSaveObject(save);

            fs.Close();

            Debug.Log("加载存档成功");
        }
        else
        {
            Debug.Log("无任何存档数据");
        }
    }
    #endregion

    #region 以 JSON 的保存数据
    private void SaveJsonData()
    {
        Save save = CreateSaveObject();

        StreamWriter sw = new StreamWriter(Application.dataPath + "/SaveData/save.json");

        string jsonString = JsonUtility.ToJson(save);
        sw.Write(jsonString);

        Debug.Log("存档成功JSON");

        sw.Close();
    }

    private void LoadJsonData()
    {
        if(File.Exists(Application.dataPath + "/SaveData/save.json"))
        {
            StreamReader sr = new StreamReader(Application.dataPath + "/SaveData/save.json");

            Save save = JsonUtility.FromJson<Save>(sr.ReadToEnd());

            LoadSaveObject(save);

            sr.Close();
        }
        else
        {
            Debug.Log("无存档数据");
        }
    }
    #endregion
}
