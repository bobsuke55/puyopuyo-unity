using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puyo_pair_ver01 : MonoBehaviour
{       
    public GameObject mainpuyo;
    public GameObject subpuyo;
    public Transform mainpuyo_t;
    public Transform subpuyo_t;
    public Renderer mainpuyo_r;
    public Renderer subpuyo_r;

    Vector3 init_pos = new Vector3(3,9,0);

    //main puyoの移動差分
    public Vector3 delta = new Vector3();

    //main puyoと sub puyoの位置関係 
    public Vector3 sub_delta = new Vector3(0,1,0);
    public float angle = 90;

    public float xmove_sum;
    public float rtarget;

    bool is_ymoving;
    bool is_xmoving;
    bool is_rotating;

    bool can_ymoving;
    bool can_xrightmoving;
    bool can_xleftmoving;
    bool can_rotating;

    public Puyo_pair_ver01(){
        
        GameObject puyoprefab = (GameObject)Resources.Load ("puyo/puyo") as GameObject;
        this.mainpuyo = Instantiate (puyoprefab, init_pos, Quaternion.Euler(0, 0, 0));
        this.subpuyo = Instantiate (puyoprefab, init_pos+sub_delta, Quaternion.Euler(0, 0, 0));
        this.mainpuyo_t = mainpuyo.transform;
        this.subpuyo_t = subpuyo.transform;
        this.mainpuyo_r = mainpuyo.GetComponentInChildren<Renderer>();
        this.subpuyo_r = subpuyo.GetComponentInChildren<Renderer>();
    
        this.mainpuyo_r.material.SetColor("_bcolor",Color.blue);
        this.subpuyo_r.material.SetColor("_bcolor",Color.red);
        return;
    }

    public void move(bool[,] Field_bool,float vkey, float hkey, bool zkey, bool xkey){  
        //Debug.Log(this.can_ymoving,this.can_xleftmoving,this.can_xrightmoving);

        this.check_canmoving(Field_bool);
/*
        if(this.can_xleftmoving & hkey<0){ //左移動
            this.update_deltax(hkey);
        }
        else if(this.can_xrightmoving & hkey>0){ //右移動
            this.update_deltax(hkey);
        }
        else{
            delta.x = 0;
        }
        
*/
        this.update_deltax(hkey);
        this.update_deltay(hkey);
        this.update_rotating(zkey,xkey);

        this.subpuyo_t.position  = this.mainpuyo_t.position+this.sub_delta;
        }

    public void check_canmoving(bool[,] Field_bool){ // 移動できるかの確認
        Vector3Int main_pos = this.main_roundpos();
        Vector3Int sub_pos = this.sub_roundpos();


        this.can_ymoving = Field_bool[main_pos.x,main_pos.y-1] | Field_bool[sub_pos.x,sub_pos.y-1];
                    //main_pos.y >= Configs.bottom & sub_pos.y >= Configs.bottom;

        this.can_xleftmoving = Field_bool[main_pos.x-1,main_pos.y] | Field_bool[sub_pos.x-1,sub_pos.y];
                    // | main_pos.x <= Configs.left & sub_pos.x <= Configs.left;

        this.can_xrightmoving = Field_bool[main_pos.x+1,main_pos.y] | Field_bool[sub_pos.x+1,sub_pos.y];
                    //main_pos.x >= Configs.bottom & sub_pos.x >= Configs.right;

    }
    //縦移動
    public void update_deltay(float vkey){
        if (vkey<0){
            delta.y = -1.0f*Configs.yspeed*Time.deltaTime;
        }
        else {
            delta.y = -1.0f*Configs.freeyspeed*Time.deltaTime;
        }
    }

    //横移動
    public void update_deltax(float hkey){
        if (this.is_xmoving){ 
            this.xmove_sum += Mathf.Abs( this.delta.x );

            if(this.xmove_sum >= Configs.xmove){
                this.is_xmoving = false;
                this.delta.x = Mathf.Sign(delta.x)*(Configs.xmove - this.xmove_sum);
            }
        }
        else{ 
            if (hkey < 0){ //左移動
                this.xmove_sum = 0;
                this.delta.x = -1.0f * Configs.xspeed * Time.deltaTime;
                this.is_xmoving = true;
                }
            else if(hkey > 0){ //右移動
                this.xmove_sum = 0;
                this.delta.x = Configs.xspeed * Time.deltaTime;
                this.is_xmoving = true;
                }
            else{
                this.xmove_sum = 0;
                this.delta.x = 0;
                this.is_xmoving = false;
            }
        }
    }

    //回転 & 回し & ずらし
    public void update_rotating(bool z,bool x){
        if (this.is_rotating){ // 回転中
            Debug.Log(this.rtarget);
            Debug.Log(this.angle);

            this.angle = Mathf.MoveTowards(this.angle,this.rtarget,Configs.rspeed*Time.deltaTime);
            this.sub_delta.x = Mathf.Cos(Mathf.Deg2Rad*this.angle);
            this.sub_delta.y = Mathf.Sin(Mathf.Deg2Rad*this.angle);

            if(this.angle == this.rtarget){
                is_rotating = false;
            }
        }
        else{ // 反時計回転
            if (z){
                this.rtarget = this.angle + 90;
                this.is_rotating = true;
                Debug.Log(this.rtarget);
                }
            else if(x){ //時計回転
                this.rtarget = this.angle - 90;
                this.is_rotating = true;
                }
            else{
                this.is_rotating = false;
            }
        }
    }
    public Vector3Int main_roundpos(){
        Vector3Int main_pos = new Vector3Int( Mathf.RoundToInt(mainpuyo_t.position.x),
                                Mathf.RoundToInt(mainpuyo_t.position.y),
                                Mathf.RoundToInt(mainpuyo_t.position.z) );
        return main_pos;
    }
    public Vector3Int sub_roundpos(){
        Vector3Int sub_pos = new Vector3Int( Mathf.RoundToInt(subpuyo_t.position.x),
                                Mathf.RoundToInt(subpuyo_t.position.y),
                                Mathf.RoundToInt(subpuyo_t.position.z) );
        return sub_pos;
    }
    public Vector3Int main_floorpos(){
        Vector3Int main_pos = new Vector3Int( Mathf.FloorToInt(mainpuyo_t.position.x),
                                Mathf.FloorToInt(mainpuyo_t.position.y),
                                Mathf.FloorToInt(mainpuyo_t.position.z) );
        return main_pos;
    }
    public Vector3Int sub_floorpos(){
        Vector3Int sub_pos = new Vector3Int( Mathf.FloorToInt(subpuyo_t.position.x),
                                Mathf.FloorToInt(subpuyo_t.position.y),
                                Mathf.FloorToInt(subpuyo_t.position.z) );
        return sub_pos;
    }


    public void fix(){ // 現在の位置にぷよを固定。
        //new Vector3[2] puyopos = this.round_pos();
        this.mainpuyo_t.position = this.main_roundpos();
        this.subpuyo_t.position  = this.sub_roundpos();
    }

    public void fix_x(){ // 現在の位置にぷよを固定。
        //new Vector3[2] puyopos = this.round_pos();
        this.mainpuyo_t.position = this.main_roundpos();
        this.subpuyo_t.position  = this.sub_roundpos();
    }

}
