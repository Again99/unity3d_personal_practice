using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Coin : MonoBehaviour
{
    [Tooltip("旋转速度")]
    public float speed = 5f;

    public bool isDead = false;

    private void Start()
    {
        // 改变物体颜色
        GetComponent<Renderer>().material.color = Color.yellow;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.pauseState)
            return;

        if (isDead)
        {
            gameObject.SetActive(false);
        }

        // 旋转
        transform.Rotate(0, speed, 0, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 使玩家触发后，进行得分和销毁操作
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("分数增加");
            GameManager.instance.coin += 1;
            isDead = true;
            //Destroy(gameObject);
        }
    }
}
