using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puyo : MonoBehaviour
{
    private GameObject firstpuyo;
    private GameObject secondpuyo;

    private float speed = 0.2f;

    //puyoの色(id)
    private int puyo_id;

    void Start(){

    }

    void Update()
    {

        float hKey = Input.GetAxis("Horizontal");
        float vKey = Input.GetAxis("Vertical");

        //右入力で右向きに動く
        if(hKey > 0)
        {
            this.transform.position += new Vector3(speed, 0, 0);
        }
        //左入力で左向きに動く
        else if(hKey < 0)
        {
            this.transform.position += new Vector3(-speed, 0, 0);
        }

        if(vKey > 0)
        {
            this.transform.position += new Vector3(0, speed, 0);
        }
        //左入力で左向きに動く
        else if(vKey < 0)
        {
            this.transform.position += new Vector3(0, -speed, 0);
        }

    }


    //整数の足し算を行うメソッド
    public int Add(int a,int b){
        return a + b;
    }
 
    //整数の引き算を行うメソッド
    public int Sub(int a,int b){
        return a - b;
    }
 
    //整数のかけ算を行うメソッド
    public int Mul(int a,int b){
        return a * b;
    }
 
    //整数の割り算を行うメソッド
    public int Div(int a,int b){
        return a / b;
    }
}