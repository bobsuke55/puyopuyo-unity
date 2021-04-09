using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Puyo : MonoBehaviour
{

    public GameObject puyo;

    //配列参照用pos. Gameobjectと同じように動かす。
    private Vector3Int pos;

    public Vector3 delta = new Vector3Int(0,0,0);

    public int fall_starty;
    public int fall_deltay;
    public float fall_time;
    public float fall_time_elapsed;

    public int puyo_color;
    public bool is_falling = true;
    public int side_flag = 0b0000;

    private Renderer puyo_renderer;

    public Vector3 Pos_g
    {
        set{this.puyo.transform.localPosition = value;}
        get{return this.puyo.transform.localPosition;}
    }

    public Vector3Int Pos
    {
        set{pos = value;}
        get{return pos;}
    }

    public Puyo(string name, Vector3 init_pos,GameObject p_board){
     
    　　//puyo生成
        GameObject puyoprefab = (GameObject)Resources.Load ("puyo/puyo") as GameObject;
        this.puyo = Instantiate (puyoprefab,new Vector3(0,0,0) , Quaternion.Euler(0, 0, 0));
        this.puyo.name  = name; 
        
        this.puyo.transform.parent  = p_board.transform;
        this.puyo.transform.localPosition = init_pos;
        
        this.pos = Vector3Int.FloorToInt(init_pos);
        this.puyo_color = UnityEngine.Random.Range(0,Configs.color_num);

        this.puyo_renderer = this.puyo.GetComponentInChildren<Renderer>();
        this.SetColor();
    }

    public void MoveObj(float delta_x,float delta_y){
        this.puyo.transform.Translate((int)delta_x,(int)delta_y,0,this.puyo.transform.parent);
    }
    
    public void MovePos(float delta_x,float delta_y){
        this.pos.x += (int)delta_x;
        this.pos.y += (int)delta_y;
    }

    public void SetObj(Vector3 pos){
        this.puyo.transform.localPosition = pos;
    }

    public void SetPos(Vector3 pos){
        this.pos = Vector3Int.FloorToInt(pos);
    }

    public void ResetAdj(Vector2Int d){
        this.side_flag = 0b0000;
    }

//繋がるぷよをside_flagに保持する。
    public void SetAdj(Vector2Int d){
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

    public void SetImage(){
        this.puyo_renderer.material.SetFloat("_Image_num",this.side_flag);
    }

    public void SetBlinking(){
        this.puyo_renderer.material.SetFloat("_Blinking_gate",1);
    }

    public void SetFallDeltay(int deltay){
        this.fall_deltay = deltay;
        this.is_falling = true;
    }

    public void CheckFall(bool[,] Field_bool){
        this.is_falling = true;
        this.fall_starty = this.pos.y;
        this.fall_deltay = 0;
        this.fall_time_elapsed = 0;

        var y = 0;
        while ( (this.pos.y - y ) >= Configs.height_out ){
            this.fall_deltay += System.Convert.ToInt16( Field_bool[pos.x,pos.y-y] );
            y += 1;
        }
        this.fall_time = Configs.time_func[this.fall_deltay/2];
        this.MovePos(0,-this.fall_deltay);
    }

    public void FallPuyo(){ // call after CheckFall();
        var target_pos = this.fall_starty-this.fall_deltay;
        this.fall_time_elapsed += Time.deltaTime;
        var tpos = this.Pos_g;
        tpos.y = Mathf.Lerp(this.fall_starty,target_pos,this.fall_time_elapsed/this.fall_time );

        if (tpos.y <= target_pos){
            tpos.y = target_pos;
            this.is_falling = false;
        }
        this.SetObj(tpos);
    }


    public void DestroyPuyo(){
        Destroy(this.puyo);
        this.puyo = null;
    }

    private void SetColor(){
        if (this.puyo_color == 0){
            this.puyo_renderer.material.SetVector("_bcolor",Configs.one);
        }else if(this.puyo_color == 1){
            this.puyo_renderer.material.SetVector("_bcolor",Configs.two);
        }else if(this.puyo_color == 2){
            this.puyo_renderer.material.SetVector("_bcolor",Configs.three);
        }else if(this.puyo_color == 3){
            this.puyo_renderer.material.SetVector("_bcolor",Configs.four);
        }
    }
}