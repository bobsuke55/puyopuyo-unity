using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Puyo : MonoBehaviour
{

    public GameObject puyo;

    //配列参照用pos. Gameobjectと同じように動かす。
    public Vector3Int pos;
    public int fall_starty;
    public int fall_deltay;
    public float fall_time;
    public float fall_time_elapsed;

    public int puyo_color;
    public bool is_falling = true;
    public int side_flag = 0b0000;

    private Renderer puyo_renderer;

    public Puyo(string name, Vector3 init_pos,GameObject p_board){
     
    　　//puyo生成
        GameObject puyoprefab = (GameObject)Resources.Load ("puyo/puyo") as GameObject;
        this.puyo = Instantiate (puyoprefab,new Vector3(0,0,0) , Quaternion.Euler(0, 0, 0));
        this.puyo.name  = name;
        
        this.puyo.transform.parent  = p_board.transform;
        this.puyo.transform.localPosition = init_pos;
        this.pos = Vector3Int.FloorToInt(init_pos);
        this.puyo_color = UnityEngine.Random.Range(0,Configs.color_num);

    }
    public void movey(){
        float delta_y = -1 * Configs.ymove;
        this.puyo.transform.Translate(0,(int)delta_y,0,this.puyo.transform.parent);
        this.pos.y += (int)delta_y;
    }

    private void movex(float hkey){
        this.delta.x = Mathf.Sign(hkey) * Configs.xmove;
        this.is_xmoving = true;

        this.mpuyo.transform.Translate((int)this.delta.x,0,0,this.mpuyo.transform.parent);
        this.spuyo.transform.Translate((int)this.delta.x,0,0,this.spuyo.transform.parent);

        //GameObjectは2フレームかけて2マス移動するが、判定は1フレームで2マス分動かす。
        this.mpuyo.pos.x += (int)delta.x * 2;
        this.spuyo.pos.x += (int)delta.x * 2;
    }

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