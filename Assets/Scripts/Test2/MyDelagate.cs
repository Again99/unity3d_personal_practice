using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyDelagate : MonoBehaviour
{
    private Renderer rd;

    // 定义委托类型
    delegate void MyDelagateClass();
    MyDelagateClass myDelagate; // 定义委托字段

    // Start is called before the first frame update
    void Start()
    {
        rd = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            // 直接调用
            //ChangeColor();
            //Log();

            // 委托调用（间接调用）
            //myDelagate = new MyDelagateClass(ChangeColor);
            //myDelagate += new MyDelagateClass(Log);
            //myDelagate.Invoke();

            // 委托调用简写
            myDelagate = ChangeColor;
            myDelagate += Log; // 多播委托
            myDelagate();
        }
    }

    void ChangeColor()
    {
        rd.material.color = new Color(Random.value, Random.value, Random.value);
    }

    void Log()
    {
        Debug.Log("Hey, This's Log");
    }
}
