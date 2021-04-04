using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Puyo : MonoBehaviour
{
    public GameObject puyo;
    public Vector3Int pos;
    public int fall_starty;
    public int fall_deltay;
    public float fall_time;
    public float fall_time_elapsed;

    public int puyo_color;
    public bool is_falling = true;
    public int side_flag = 0b0000;

    private Renderer puyo_renderer;


    public void reset_sideflag(Vector2Int d){
        this.side_flag = 0b0000;
    }

//繋がるぷよをside_flagに保持する。
    public void set_sideflag(Vector2Int d){
        if ( d.y == 1 ){ //上
            this.side_flag = this.side_flag | 0b1000;
        }
        else if ( d.y == -1){ //下
            this.side_flag = this.side_flag | 0b0100;
        }
        else if ( d.x == -1){ //左
            this.side_flag = this.side_flag | 0b0010;
        }
        else if ( d.x == 1 ){ //右
            this.side_flag = this.side_flag | 0b0001;
        }
    }

    public void set_image(){
        this.puyo_renderer.material.SetFloat("_Image_num",this.side_flag);
    }

    public void set_blinking(){
        this.puyo_renderer.material.SetFloat("_Blinking_gate",1);
    }

    public void set_fall_deltay(int deltay){
        this.fall_deltay = deltay;
        this.is_falling = true;
    }
    public void set_pos(Vector3Int pos){
        this.pos = pos;
    }

    public void check_fall(bool[,] Field_bool){
        this.is_falling = true;
        this.fall_starty = this.pos.y;
        this.fall_deltay = 0;
        this.fall_time_elapsed = 0;

        var y = 0;
        while ( (this.pos.y - y ) >= 2 ){
            this.fall_deltay += System.Convert.ToInt16( Field_bool[pos.x,pos.y-y] );
            y += 1;
        }
        this.pos.y -= this.fall_deltay;
        this.fall_time = Configs.time_func[this.fall_deltay/2];
    }

    public void fall_puyo(){ // call after check_fall();
        var target_pos = this.fall_starty-this.fall_deltay;
        this.fall_time_elapsed += Time.deltaTime;
        var tpos = this.transform.localPosition;
        tpos.y = Mathf.Lerp(this.fall_starty,target_pos, this.fall_time_elapsed/this.fall_time );

        if (tpos.y <= target_pos){
            tpos.y = target_pos;
            this.is_falling = false;
        }
        this.transform.localPosition = tpos;
    }

    public void set_color(){
        if (this.puyo_color == 0){
            this.puyo_renderer = this.GetComponentInChildren<Renderer>();
            this.puyo_renderer.material.SetVector("_bcolor",Configs.one);
        }else if(this.puyo_color == 1){
            this.puyo_renderer = this.GetComponentInChildren<Renderer>();
            this.puyo_renderer.material.SetVector("_bcolor",Configs.two);
        }else if(this.puyo_color == 2){
            this.puyo_renderer = this.GetComponentInChildren<Renderer>();
            this.puyo_renderer.material.SetVector("_bcolor",Configs.three);
        }else if(this.puyo_color == 3){
            this.puyo_renderer = this.GetComponentInChildren<Renderer>();
            this.puyo_renderer.material.SetVector("_bcolor",Configs.four);
        }
    }
}