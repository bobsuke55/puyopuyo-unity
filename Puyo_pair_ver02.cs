using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puyo_pair_ver02 : MonoBehaviour
{       
    public GameObject puyo;
    public GameObject mainpuyo;
    public GameObject subpuyo;
    public Renderer mainpuyo_r;
    public Renderer subpuyo_r;

    private Vector3Int init_pos = new Vector3Int(3,9,0);
    
    //main puyoと sub puyoの位置差分
    private Vector3 sub_delta = new Vector3(0,1,0);
    public float angle = 90;

    public float rtarget;
    public float xtarget = 0f;

    //mainpuyoとsubpuyoの位置(int)
    private Vector3Int mainpos;
    private Vector3Int subpos;

    bool is_ymoving;
    bool is_xmoving;
    bool is_rotating;

    bool can_ymoving;
    bool can_xrightmoving;
    bool can_xleftmoving;
    bool can_rotating;

    bool lslide_flag;
    bool rslide_flag;

    public Puyo_pair_ver02(){
        this.puyo = new GameObject("puyo");
        this.puyo.transform.position = init_pos;

    　　//puyo生成
        GameObject puyoprefab = (GameObject)Resources.Load ("puyo/puyo") as GameObject;
        this.mainpuyo = Instantiate (puyoprefab, init_pos, Quaternion.Euler(0, 0, 0));
        this.mainpuyo.name = "mainpuyo";
        this.subpuyo = Instantiate (puyoprefab, init_pos+sub_delta, Quaternion.Euler(0, 0, 0));
        this.subpuyo.name = "subpuyo";
        this.mainpuyo.transform.parent = this.puyo.transform;
        this.subpuyo.transform.parent  = this.puyo.transform;

        this.mainpuyo_r = mainpuyo.GetComponentInChildren<Renderer>();
        this.subpuyo_r = subpuyo.GetComponentInChildren<Renderer>();
        this.mainpuyo_r.material.SetColor("_bcolor",Color.blue);
        this.subpuyo_r.material.SetColor("_bcolor",Color.red);
        return;
    }

    public void move(bool[,] Field_bool,float vkey, float hkey, bool zkey, bool xkey){  
        //Debug.Log(this.can_ymoving,this.can_xleftmoving,this.can_xrightmoving);

        this.check_canmoving(Field_bool);
//        this.update_movey(vkey);
//        this.update_movex(hkey);
        this.update_rotating(zkey,xkey);

        if(this.can_ymoving){
            this.update_movey(vkey);
        }

//左に動いたらだめな時、hkeyが負なら消せばいい。
//右に動いたらダメな時、hkeyが正なら消せばいい。

        if(!this.can_xleftmoving){　　 //左移動
            if (hkey<0){
                hkey=0;
            }
        }
        else if(!this.can_xrightmoving){//右移動
            if (hkey > 0){
                hkey=0;
            }
        }
        this.update_movex(hkey);
        
        }

    public void check_canmoving(bool[,] Field_bool){ // 移動できるかの確認
        this.mainpos = Vector3Int.RoundToInt(this.puyo.transform.position);
        this.subpos  = mainpos + Vector3Int.RoundToInt( this.sub_delta );

        this.can_ymoving      = Field_bool[this.mainpos.x,this.mainpos.y-1] & Field_bool[this.subpos.x,this.subpos.y-1];
        this.can_xleftmoving  = Field_bool[this.mainpos.x-1,this.mainpos.y] & Field_bool[this.subpos.x-1,this.subpos.y];
        this.can_xrightmoving = Field_bool[this.mainpos.x+1,this.mainpos.y] & Field_bool[this.subpos.x+1,this.subpos.y];

        this.lslide_flag = Field_bool[this.mainpos.x+1,this.mainpos.y] & (angle == 90);
        this.rslide_flag = Field_bool[this.mainpos.x-1,this.mainpos.y] & (angle == 90);

    }

    //縦移動
    public void update_movey(float vkey){
        float deltay;

        if (vkey<0){
            deltay = -1.0f*Configs.yspeed*Time.deltaTime;
        }
        else {
            deltay = -1.0f*Configs.freeyspeed*Time.deltaTime;
        }
        this.puyo.transform.Translate( 0,deltay,0 );
    }
    //横移動
    public void update_movex(float hkey){
        if (this.is_xmoving){
            Vector3 xpos = this.puyo.transform.position;
            xpos.x = Mathf.MoveTowards(xpos.x,this.xtarget ,Configs.xspeed*Time.deltaTime);
            this.puyo.transform.position = xpos;

            if(this.puyo.transform.position.x == this.xtarget ){
                this.is_xmoving = false;
            }
        }
        else{
            if (hkey == 0){
            }
            else{
            this.xtarget 　= this.puyo.transform.position.x+Mathf.Sign(hkey)*Configs.xmove;
            this.is_xmoving = true;
            }
        }
    }
    //回転 & 回し & ずらし
    public void update_rotating(bool z,bool x){
        if (this.is_rotating){ 

            this.angle = Mathf.MoveTowards(this.angle,this.rtarget,Configs.rspeed*Time.deltaTime);
            this.sub_delta.x = Mathf.Cos(Mathf.Deg2Rad*this.angle);
            this.sub_delta.y = Mathf.Sin(Mathf.Deg2Rad*this.angle);
            this.subpuyo.transform.position  = this.mainpuyo.transform.position+this.sub_delta;

            if(this.angle == this.rtarget){
                is_rotating = false;
            }
        }
        else{ // 反時計回転 main_puyoの右側がFalseかつ angleが90のときに
            if (z){ // 反時計回転
            //main puyoの右側がfalseかつangleが90のとき、mainpuyoを左側に動かす。
                if(angle==90 & this.)
            
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
    //縦移動



}
