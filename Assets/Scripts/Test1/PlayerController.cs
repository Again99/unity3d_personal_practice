using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Transform playerTs;

    [Tooltip("移动速度")]
    public float speed = 2f;

    [Tooltip("Player坐标")]
    public Vector3 playerPos;

    // Start is called before the first frame update
    void Start()
    {
        playerTs = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float z = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        playerTs.transform.Translate(new Vector3(x, 0, z));

        playerPos = playerTs.position;
    }
}
