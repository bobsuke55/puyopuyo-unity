using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Configs
{

    public static int next_num = 2;
    public static Vector3[] next_pos = new Vector3[4] {new Vector3(17,27,0),new Vector3(17,23,0),new Vector3(17,19,0),new Vector3(17,15,0)};
    public static Vector3 init_pos = new Vector3(6,24,0);
    public static float next_time = 10/60f;

    //global座標におけるpuyo_imageのサイズ.
    public static int puyo_width  = 2;
    public static int puyo_height = 2;

    //ぷよ移動用配列。
    public static int grid_num = 2;
    public static int height = 14;
    public static int width = 6;
    public static int height_out = 2;
    public static int width_out = 2;
    public static int board_height = grid_num*(height+height_out);
    public static int board_width =  grid_num*(width+width_out);

    public static float create_time = 6/60f;

    // 縦移動にかかる時間 
    public static float yspeed = 20.0f; //加速 key入力を入れると　yspeed倍の速さでぷよが落ちる。
    public static float ytime = 2/6f;  

    public static float ymove_count = 2;
    public static float ymove_amount = puyo_height/ymove_count;

    //xmove_count回xmove分移動すれば1マス移動したことになる。
    public static float xmove_count = 2;
    public static float xmove_amount = puyo_width/xmove_count;
    public static float xmove_inputdelay = 0.05f ; //横移動の入力遅延時間。

    //width 
    public static float smove_count = 1f; 
    public static float fixtime  = 6/6f;
    public static float fixspeed = 10f;// (2/60)

    public static float rotate_speed = 10*90f;
    public static float fall_time = 0.05f;

    public static Vector4 one = new Vector4(255,0,0,0);
    public static Vector4 two = new Vector4(0,255,0,0);
    public static Vector4 three = new Vector4(0,0,255,0);
    public static Vector4 four = new Vector4(255,255,0,0);

    public static int random_seed = 0;

    //繋がる
    public static int color_num = 4;
    public static int erase_num = 4;

    public static float erase_time = 1.0f;

    //chain
    public static int[] chain_bonus = new int[19] {
        0,8,16,32,64,96,128,160,192,224,256,288,320,352,384,416,448,480,512};

    //link_num
    public static int[] link_bonus = new int[8]{0,2,3,4,5,6,7,10};

    //color_num
    public static int[] color_bonus = new int[5]{0,3,6,12,24};

    //
    //public static float[] time_func   = new float[12] { 100,19f/60f,24f/60f,28f/60f,31f/60f,34f/60f,37f/60f,40f/60f,42f/60f,44f/60f,46f/60f,48f/60f };
    //public static float[] time_func   = new float[12] { 100,13f/60f,18f/60f,22f/60f,15f/60f,28f/60f,31f/60f,34f/60f,46f/60f,38f/60f,40f/60f,42f/60f };
    //public static float[] time_func   = new float[12] { 100,9f/60f,14f/60f,18f/60f,21f/60f,24f/60f,27f/60f,30f/60f,32f/60f,34f/60f,36f/60f,38f/60f };
    //public static float[] time_func   = new float[12] { 100,7f/60f,12f/60f,16f/60f,19f/60f,22f/60f,25f/60f,28f/60f,30f/60f,32f/60f,34f/60f,36f/60f };
    public static float[] time_func   = new float[12] { 100,4f/60f,9f/60f,13f/60f,16f/60f,19f/60f,22f/60f,25f/60f,27f/60f,29f/60f,31f/60f,33f/60f };

    //public static float[] time_func   = new float[12] { 100,4f/60f,9f/60f,13f/60f,16f/60f,19f/60f,22f/60f,25f/60f,27f/60f,29f/60f,31f/60f,33f/60f };

}