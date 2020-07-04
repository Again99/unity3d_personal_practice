using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 存档数据
[Serializable]
public class Save
{
    // 金币得分
    public int coin;

    #region 玩家位置
    public float playerPosX;
    public float playerPosY;
    public float playerPosZ;
    #endregion

    // 金币存档数据的列表
    public List<CoinState> coins = new List<CoinState>();
}

// 金币状态的存档数据
[Serializable]
public struct CoinState
{
    public float x;
    public float y;
    public float z;

    public bool isDead;
}