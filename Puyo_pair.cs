using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puyo_pair : MonoBehaviour
{       
    public GameObject puyo;
    public GameObject mainpuyo;
    public GameObject subpuyo;
    public Renderer mainpuyo_r;
    public Renderer subpuyo_r;

    private Vector3 init_pos = new Vector3(6*Configs.scale,11*Configs.scale,0);
    
    //main puyoの移動差分
    public Vector3 delta = new Vector3(0,0,0);
    public Vector3 slide_delta = new Vector3(0,0,0);
    //main puyoと sub puyoの位置差分
    private Vector3 sub_delta = new Vector3(0,2,0);
    public float angle = 90;

    public float target_angle;
    public float xtarget = 0f;

    public float xtimeElapsed;
    public float ytimeElapsed;
    public float fixtimeElapsed;

    private int xmove_count = 0;
    private int ymove_count = 0;
    private int smove_count = 0;

    //mainpuyoとsubpuyoの位置(int)
    public Vector3Int mainpos = new Vector3Int(0,0,0);
    public Vector3Int subpos  = new Vector3Int(0,0,0);

    //flag管理のためのposition.
    private Vector3Int next_mainpos;
    private Vector3Int next_subpos;
    private Vector3 next_subdelta;

    public bool is_ymoving;
    public bool is_xmoving;
    public bool is_rotating;

    public bool can_ymoving;
    public bool can_xrightmoving;
    public bool can_xleftmoving;
    public bool can_rotating;
    public string state;

    /*
    rotate_flag //回転。
    rslide_flag //main puyoを右にずらす & 回転。 angleが90でzを押したときor270でxを押したとき かつ main puyoの左側のField_boolがfalseの場合。
    lslide_flag //main puyoを左にずらす & 回転。 angleが90でxを押したときor270でzを押したとき かつ main puyoの右側のFiedl_boolがfalseの場合。
    uslide_flag //main puyoを上にずらす & 回転。 angleが180でzを押したとき or 0でxを押したとき かつ main puyoの下側のField_boolがfalseの場合。
    unrotate_flag // 未回転 & 未移動。mainpuyoの両側にぷよがある場合。
    */

    public bool lslide_flag;
    public bool rslide_flag;
    public bool uslide_flag;
    public bool unrotate_flag;

    public Puyo_pair(string num){
        this.puyo = new GameObject($"puyo_{num}");
        this.puyo.transform.position = init_pos;

    　　//puyo生成
        GameObject puyoprefab = (GameObject)Resources.Load ("puyo/puyo") as GameObject;
        this.mainpuyo = Instantiate (puyoprefab, init_pos, Quaternion.Euler(0, 0, 0));
        this.mainpuyo.name = $"mainpuyo_{num}";
        this.subpuyo = Instantiate (puyoprefab, init_pos+sub_delta, Quaternion.Euler(0, 0, 0));
        this.subpuyo.name  = $"subpuyo_{num}";
        this.mainpuyo.transform.parent = this.puyo.transform;
        this.subpuyo.transform.parent  = this.puyo.transform;

        this.mainpuyo_r = mainpuyo.GetComponentInChildren<Renderer>();
        this.subpuyo_r = subpuyo.GetComponentInChildren<Renderer>();
        this.mainpuyo_r.material.SetColor("_bcolor",Color.blue);
        this.subpuyo_r.material.SetColor("_bcolor",Color.red);
        return;
    }

    public void move(bool[,] Field_bool,float vkey, float hkey, bool zkey, bool xkey){  
        
        this.check_canmoving(Field_bool,hkey,zkey,xkey); 

        this.ytimeElapsed += Time.deltaTime;
        this.xtimeElapsed += Time.deltaTime;

        if (this.can_ymoving){
            this.movey(vkey); 
        }

        this.check_canmoving(Field_bool,hkey,zkey,xkey);

        if( (this.can_xleftmoving) & (hkey < 0) ){  //左移動
            this.movex(hkey);
        }
        else if( (this.can_xrightmoving) & (hkey > 0) ){　//右移動
            this.movex(hkey);
        }
        else if(this.is_xmoving){ //横移動処理中..
            this.movex(hkey); 
        }

        this.check_canmoving(Field_bool,hkey,zkey,xkey);

        this.rotating_slide(zkey,xkey);
    }


    public void check_canmoving(bool[,] Field_bool,float hkey,bool zkey,bool xkey){ // 
        this.mainpos   = Vector3Int.FloorToInt(this.puyo.transform.position);
        this.subpos    = Vector3Int.FloorToInt(this.puyo.transform.position + this.sub_delta );
        var angle_plus = this.plus_mod(this.angle,360f);

        this.can_ymoving      = Field_bool[this.mainpos.x,mainpos.y-1] & Field_bool[this.mainpos.x+1,mainpos.y-1]
                               &Field_bool[this.subpos.x,subpos.y-1]   & Field_bool[this.subpos.x+1,subpos.y-1];

        this.can_xleftmoving  = Field_bool[this.mainpos.x-1,this.mainpos.y] & Field_bool[this.mainpos.x-1,this.mainpos.y+1]
                               &Field_bool[Mathf.Clamp(this.subpos.x-1,0,Configs.board_width-1),this.subpos.y] 
                               &Field_bool[Mathf.Clamp(this.subpos.x-1,0,Configs.board_width-1),this.subpos.y+1];

        this.can_xrightmoving = Field_bool[this.mainpos.x+2,this.mainpos.y] & Field_bool[this.mainpos.x+2,this.mainpos.y+1]
                               &Field_bool[Mathf.Clamp(this.subpos.x+2,0,Configs.board_width-1),this.subpos.y]
                               &Field_bool[Mathf.Clamp(this.subpos.x+2,0,Configs.board_width-1),this.subpos.y+1];
    
        this.rslide_flag   = ( (zkey & angle_plus == 90)  | (xkey & angle_plus == 270) ) & //右側に回転ぷよをおさめるスペースがあるか?
                             (!Field_bool[this.mainpos.x-1,this.mainpos.y]   | !Field_bool[this.mainpos.x-2,this.mainpos.y]
                            | !Field_bool[this.mainpos.x-1,this.mainpos.y+1] | !Field_bool[this.mainpos.x-2,this.mainpos.y+1]);


        this.lslide_flag   = ( (xkey & angle_plus == 90)  | (zkey & angle_plus == 270) ) & //左側に回転ぷよをおさめるスペースがあるか?
                             (!Field_bool[this.mainpos.x+2,this.mainpos.y]   | !Field_bool[this.mainpos.x+3,this.mainpos.y]
                            | !Field_bool[this.mainpos.x+2,this.mainpos.y+1] | !Field_bool[this.mainpos.x+3,this.mainpos.y+1]);


        this.uslide_flag   = ( (zkey & angle_plus == 180) | (xkey & angle_plus == 0) ) & //下側に回転ぷよをおさめるスペースがあるか?
                             ( !Field_bool[this.mainpos.x,  this.mainpos.y-1] | !Field_bool[this.mainpos.x+1,  this.mainpos.y-1]
                             | !Field_bool[this.mainpos.x,  this.mainpos.y-2] | !Field_bool[this.mainpos.x+1,  this.mainpos.y-2]);
            

        //はさまってる。
        //this.unrotate_flag = ( !Field_bool[this.mainpos.x+1,this.mainpos.y] & !Field_bool[this.mainpos.x-1,this.mainpos.y] );
        if (this.is_rotating){
            Debug.Log("nextmain" + this.next_mainpos);
            Debug.Log("nextsub" + this.next_subpos);

            this.can_ymoving  = Field_bool[this.next_mainpos.x,next_mainpos.y-1] & Field_bool[this.next_mainpos.x+1,next_mainpos.y-1]
                               &Field_bool[this.next_subpos.x,next_subpos.y-1] 
                               &Field_bool[this.next_subpos.x+1,next_subpos.y-1];

            this.can_xleftmoving = Field_bool[this.next_mainpos.x-1,this.next_mainpos.y] & Field_bool[this.next_mainpos.x-1,this.next_mainpos.y+1]
                               &Field_bool[Mathf.Clamp(this.next_subpos.x-1,0,Configs.board_width-1),this.next_subpos.y]
                               &Field_bool[Mathf.Clamp(this.next_subpos.x-1,0,Configs.board_width-1),this.next_subpos.y+1];

            this.can_xrightmoving = Field_bool[this.next_mainpos.x+2,this.next_mainpos.y]&Field_bool[this.next_mainpos.x+2,this.next_mainpos.y+1]
                               &Field_bool[Mathf.Clamp(this.next_subpos.x+2,0,Configs.board_width-1),this.next_subpos.y]
                               &Field_bool[Mathf.Clamp(this.next_subpos.x+2,0,Configs.board_width-1),this.next_subpos.y+1];
        }
    }


    //縦移動
    public void movey(float vkey){
        float time;
        if (vkey < 0){
            time = Configs.ytime / Configs.yspeed;
        }
        else{
            time = Configs.ytime;
        }

        if (this.ytimeElapsed >= time){
            this.ytimeElapsed = 0f;
            this.delta.y = -1.0f * Configs.ymove;
            this.puyo.transform.Translate(0,this.delta.y,0);
        }
    }
    
    //横移動
    public void movex(float hkey){
        if (this.is_xmoving){
            this.xmove_count += 1;
            if(this.xmove_count >= Configs.xmove_count){
                this.is_xmoving = false;
                this.delta.x = 0;
                this.xtimeElapsed = 0;
            }
        }
        else{
            if (this.xtimeElapsed > Configs.xtime){ //左移動 or 右移動
                this.xmove_count = 0;
                this.delta.x = Mathf.Sign(hkey) * Configs.xmove;
                this.is_xmoving = true;
            }
        }
        this.puyo.transform.Translate(this.delta.x,0,0);
    }

    public void rotating_slide(bool z,bool x){
        if (this.is_rotating){
            this.smove_count += 1;

            this.angle = Mathf.MoveTowards(this.angle,this.target_angle,Configs.rspeed*Time.deltaTime);
            this.sub_delta.x = 2*Mathf.Round(Mathf.Cos(Mathf.Deg2Rad*this.angle)*100)/100;
            this.sub_delta.y = 2*Mathf.Round(Mathf.Sin(Mathf.Deg2Rad*this.angle)*100)/100;

            if( this.smove_count>=Configs.smove_count ){
                this.slide_delta = new Vector3(0,0,0);

                if(this.angle == this.target_angle){
                    this.is_rotating = false;
                    this.smove_count = 0;
                }
            }
        }
        else{
            //zとxに応じて次の回転角を決める。
            if (z){
                this.target_angle = this.angle + 90;
                this.is_rotating = true;
                }
            else if(x){ //時計回転
                this.target_angle = this.angle - 90;
                this.is_rotating = true;
                }
            else{
                this.is_rotating = false;
            }

            this.next_subdelta.x = 2*Mathf.Round(Mathf.Cos(Mathf.Deg2Rad*this.target_angle)*100)/100;
            this.next_subdelta.y = 2*Mathf.Round(Mathf.Sin(Mathf.Deg2Rad*this.target_angle)*100)/100;

            if (this.rslide_flag){ //右にスライド
                this.slide_delta.x = 1.0f * Configs.xmove;
            }
            else if(this.lslide_flag){
                this.slide_delta.x = -1.0f * Configs.xmove;
            }
            else if(this.uslide_flag){
                this.slide_delta.y = 1.0f * Configs.ymove;
                this.ytimeElapsed = 0;
            }
            else if(this.unrotate_flag){
                this.slide_delta.y = -1.0f * Configs.ymove;
            }
            this.next_mainpos  = Vector3Int.FloorToInt( this.puyo.transform.position + this.slide_delta );
            this.next_subpos   = Vector3Int.FloorToInt( this.puyo.transform.position + this.slide_delta + this.next_subdelta);
        }
        this.puyo.transform.Translate( slide_delta.x,slide_delta.y,0 );
        this.subpuyo.transform.position  = this.mainpuyo.transform.position + this.sub_delta;
    }


    private float plus_mod(float a,float b){
        return a- Mathf.Floor(a/b)*b;
    }


}
