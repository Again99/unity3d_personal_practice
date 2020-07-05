using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
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
        // -- 使用何种方式存储数据 --

        //SavePlayerPrefs();
        //SaveSerializes();
        //SaveJsonData();
        SaveXmlData();
    }

    // 游戏加载按钮触发
    public void Load()
    {
        // -- 使用何种方式加载数据 --

        //LoadPlayerPrefs();
        //LoadSerializes();
        //LoadJsonData();
        LoadXmlData();
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
        if (File.Exists(Application.dataPath + "/SaveData/save.json"))
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

    #region 以 Xml 的方式存储数据
    private void SaveXmlData()
    {
        // 创建 save 对象和 xml 文档对象
        Save save = CreateSaveObject();
        XmlDocument xmlDocument = new XmlDocument();

        #region Xml 存档数据生成
        XmlElement root = xmlDocument.CreateElement("Save");

        // 金币得分
        XmlElement CoinNum = xmlDocument.CreateElement("CoinNum");
        CoinNum.InnerText = save.coin.ToString();
        root.AppendChild(CoinNum);

        // 玩家位置
        XmlElement PlayerPosX = xmlDocument.CreateElement("PlayerPosX");
        XmlElement PlayerPosY = xmlDocument.CreateElement("PlayerPosY");
        XmlElement PlayerPosZ = xmlDocument.CreateElement("PlayerPosZ");

        PlayerPosX.InnerText = save.playerPosX.ToString();
        PlayerPosY.InnerText = save.playerPosY.ToString();
        PlayerPosZ.InnerText = save.playerPosZ.ToString();

        root.AppendChild(PlayerPosX);
        root.AppendChild(PlayerPosY);
        root.AppendChild(PlayerPosZ);

        // 各个金币的状态
        XmlElement Coin, isDead, CoinPosX, CoinPosY, CoinPosZ;

        for (int i = 0; i < coins.Length; i++)
        {
            Coin = xmlDocument.CreateElement("Coin");
            isDead = xmlDocument.CreateElement("isDead");
            CoinPosX = xmlDocument.CreateElement("CoinPosX");
            CoinPosY = xmlDocument.CreateElement("CoinPosY");
            CoinPosZ = xmlDocument.CreateElement("CoinPosZ");

            isDead.InnerText = save.coins[i].isDead.ToString();
            CoinPosX.InnerText = save.coins[i].x.ToString();
            CoinPosY.InnerText = save.coins[i].y.ToString();
            CoinPosZ.InnerText = save.coins[i].z.ToString();

            Coin.AppendChild(isDead);
            Coin.AppendChild(CoinPosX);
            Coin.AppendChild(CoinPosY);
            Coin.AppendChild(CoinPosZ);

            root.AppendChild(Coin);
        }

        // 添加 root 节点到 xml 文档中
        xmlDocument.AppendChild(root);

        #endregion

        // 存储 xml 文档到 save.xml 文件中
        xmlDocument.Save(Application.dataPath + "/SaveData/save.xml");

        if (File.Exists(Application.dataPath + "/SaveData/save.xml"))
        {
            Debug.Log("存档成功");
        }
    }

    private void LoadXmlData()
    {
        if (File.Exists(Application.dataPath + "/SaveData/save.xml"))
        {
            // 新建 save 对象
            Save save = new Save();

            // 读取 save.xml 文件的数据
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(Application.dataPath + "/SaveData/save.xml");

            #region 加载存档数据

            // 加载金币得分
            save.coin = int.Parse(xmlDocument.GetElementsByTagName("CoinNum")[0].InnerText);

            // 加载玩家位置
            save.playerPosX = float.Parse(xmlDocument.GetElementsByTagName("PlayerPosX")[0].InnerText);
            save.playerPosY = float.Parse(xmlDocument.GetElementsByTagName("PlayerPosY")[0].InnerText);
            save.playerPosZ = float.Parse(xmlDocument.GetElementsByTagName("PlayerPosZ")[0].InnerText);

            XmlNodeList coins = xmlDocument.GetElementsByTagName("Coin");

            if (coins.Count != 0)
            {
                for (int i = 0; i < coins.Count; i++)
                {
                    CoinState coinState = new CoinState();

                    // 获取各个 Coin 的状态数据
                    XmlNodeList CoinsPosX = xmlDocument.GetElementsByTagName("CoinPosX");
                    XmlNodeList CoinsPosY = xmlDocument.GetElementsByTagName("CoinPosY");
                    XmlNodeList CoinsPosZ = xmlDocument.GetElementsByTagName("CoinPosZ");
                    XmlNodeList CoinsDead = xmlDocument.GetElementsByTagName("isDead");

                    // 转换坐标位置
                    coinState.x = float.Parse(CoinsPosX[i].InnerText);
                    coinState.y = float.Parse(CoinsPosY[i].InnerText);
                    coinState.z = float.Parse(CoinsPosZ[i].InnerText);

                    // 金币的死亡状态
                    coinState.isDead = bool.Parse(CoinsDead[i].InnerText);

                    // 添加进 save 的 coins 列表
                    save.coins.Add(coinState);
                }
            }
            #endregion

            // 加载 save 数据到场景中
            LoadSaveObject(save);

        }
        else
        {
            Debug.Log("无存档数据");
        }
    }
    #endregion
}
