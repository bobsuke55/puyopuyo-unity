using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Configs
{   
    // 縦移動にかかる時間 
    public static float scale = 2.0f;

    public static float freeyspeed = 3.0f; //加速 key入力を入れると　yspeed倍の速さでぷよが落ちる。

    public static float yspeed = 15.0f; //加速 key入力を入れると　yspeed倍の速さでぷよが落ちる。
    public static float ymove = 1f;
    public static float ytime = 0.5f ; // 放置しているとytime秒に一回tmove移動する。

    public static float xtime = 0.05f ; //横移動の入力遅延時間。
    public static float xmove = 1f;
    public static float xmove_count = 2f; //

    public static float smove_count = 2f; //
    public static float fixtime = 0.1f;

    public static float xspeed = 10.0f;
    public static float rspeed = 10*90f;

    //上下両端
    public static int height = 13;
    public static int width = 6;

    public static int board_height = 2*(height+2);
    public static int board_width =  2*(width+2);

    public static float fall_time = 0.03f;

    public static Vector4 one = new Vector4(255,0,0,0);
    public static Vector4 two = new Vector4(0,255,0,0);
    public static Vector4 three = new Vector4(0,0,255,0);
    public static Vector4 four = new Vector4(255,255,0,0);

    public static int random_seed = 0;

    public static Vector3 init_pos     = new Vector3(6,24,0);
    public static Vector3 next_pos     = new Vector3(18,26,0);
    public static Vector3 nextnext_pos = new Vector3(18,22,0);

    //繋がる
    public static int erase_num = 4;

}