using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Configs
{   

    // 縦移動にかかる時間 
    public static float freeyspeed = 3.0f; //加速 key入力を入れると　yspeed倍の速さでぷよが落ちる。

    public static float yspeed = 10.0f; //加速 key入力を入れると　yspeed倍の速さでぷよが落ちる。
    public static float ymove = 0.5f;
    public static float ytime = 0.25f; // 放置しているとytime秒に一回tmove移動する。

    public static float xtime = 0.1f; //横移動の入力遅延時間。
    public static float xmove = 0.5f;
    public static float xmove_count = 2f; //

    public static float smove_count = 2f; //


    public static float xspeed = 10.0f;
    public static float rspeed = 10*90f;

    //上下両端
    public static int top = 11;
    public static int bottom = 0;
    public static int right = 5;
    public static int left = 0;
}
