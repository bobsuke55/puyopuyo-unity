using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Configs
{   
    // 縦移動にかかる時間 
    public static float scale = 2.0f;

    public static float freeyspeed = 3.0f; //加速 key入力を入れると　yspeed倍の速さでぷよが落ちる。

    public static float yspeed = 10.0f; //加速 key入力を入れると　yspeed倍の速さでぷよが落ちる。
    public static float ymove = 1f;
    public static float ytime = 0.5f ; // 放置しているとytime秒に一回tmove移動する。

    public static float xtime = 0.05f ; //横移動の入力遅延時間。
    public static float xmove = 1f;
    public static float xmove_count = 2f; //

    public static float smove_count = 2f; //
    public static float fixtime = 0.5f;

    public static float xspeed = 10.0f;
    public static float rspeed = 10*90f;

    //上下両端
    public static int height = 13;
    public static int width = 6;

    public static int board_height = 2*(height+2);
    public static int board_width =  2*(width+2);

    public static float fall_time = 0.03f;

}