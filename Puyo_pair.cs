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

    private Vector3Int init_pos = new Vector3Int(3,9,0);
    
    //main puyoの移動差分
    public Vector3 delta = new Vector3(0,0,0);
    public Vector3 sdelta = new Vector3(0,0,0);
    //main puyoと sub puyoの位置差分
    private Vector3 sub_delta = new Vector3(0,1,0);
    public float angle = 90;

    public float rtarget;
    public float xtarget = 0f;

    private float xtimeElapsed;
    private float ytimeElapsed;

    private int xmove_count = 0;
    private int ymove_count = 0;
    private int smove_count = 0;

    //mainpuyoとsubpuyoの位置(int)
    private Vector3Int mainpos;
    private Vector3Int subpos = new Vector3Int(0,0,0);

    public bool is_ymoving;
    public bool is_xmoving;
    public bool is_rotating;

    public bool can_ymoving;
    public bool can_xrightmoving;
    public bool can_xleftmoving;
    public bool can_rotating;


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
        
        this.check_canmoving(Field_bool,hkey,zkey,xkey); // 移動可否の確認 & deltaを消す。

        //is_ ingは動作中を表す。
        if (this.can_ymoving){
            this.movey(vkey); 
        }

        if(!this.can_xleftmoving & hkey < 0){
            hkey = 0;
        }
        else if(!this.can_xrightmoving & hkey > 0){
            hkey = 0;
        }
        else if(this.is_rotating){
            hkey = 0;
        }
        this.movex(hkey);

        this.rotating_slide(zkey,xkey);
        }


    public void check_canmoving(bool[,] Field_bool,float hkey,bool zkey,bool xkey){ // 
        this.mainpos = this.mainpos_round();
        this.subpos  = this.subpos_round();
        var angle_plus = this.plus_mod(this.angle,360f);
        Debug.Log(angle_plus);
        this.can_ymoving      = Field_bool[this.mainpos.x,this.mainpos.y-1] & Field_bool[this.subpos.x,this.subpos.y-1];
        this.can_xleftmoving  = Field_bool[this.mainpos.x-1,this.mainpos.y] & Field_bool[this.subpos.x-1,this.subpos.y];
        this.can_xrightmoving = Field_bool[this.mainpos.x+1,this.mainpos.y] & Field_bool[this.subpos.x+1,this.subpos.y];

        this.rslide_flag   = ( (zkey & angle_plus == 90)  | (xkey & angle_plus == 270) ) & !Field_bool[this.mainpos.x-1,this.mainpos.y];
        this.lslide_flag   = ( (xkey & angle_plus == 90)  | (zkey & angle_plus == 270) ) & !Field_bool[this.mainpos.x+1,this.mainpos.y];
        this.uslide_flag   = ( (zkey & angle_plus == 180) | (xkey & angle_plus == 0) )   & !Field_bool[this.mainpos.x,this.mainpos.y-1];
        this.unrotate_flag = ( !Field_bool[this.mainpos.x+1,this.mainpos.y] & !Field_bool[this.mainpos.x-1,this.mainpos.y] );
        }
    
    //縦移動
    public void movey(float vkey){
        if (vkey < 0){
            this.ytimeElapsed += Time.deltaTime * Configs.yspeed;
        }
        else{
            this.ytimeElapsed += Time.deltaTime;
        }

        if (this.ytimeElapsed >= Configs.ytime){
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
            if (hkey != 0 & (this.xtimeElapsed > Configs.xtime)){ //左移動 or 右移動
                this.xmove_count = 0;
                this.delta.x = Mathf.Sign(hkey) * Configs.xmove;
                this.is_xmoving = true;
            }
            else{
                this.delta.x = 0;
                this.is_xmoving = false;
                this.xtimeElapsed += Time.deltaTime;
            }
        }
        this.puyo.transform.Translate(this.delta.x,0,0);
    }

    public void rotating_slide(bool z,bool x){
        if (this.is_rotating){
            this.smove_count += 1;
            this.angle = Mathf.MoveTowards(this.angle,this.rtarget,Configs.rspeed*Time.deltaTime);
            this.sub_delta.x = Mathf.Round(Mathf.Cos(Mathf.Deg2Rad*this.angle)*100)/100;
            this.sub_delta.y = Mathf.Round(Mathf.Sin(Mathf.Deg2Rad*this.angle)*100)/100;
            if( this.smove_count>=Configs.smove_count ){
                this.sdelta = new Vector3(0,0,0);
                if(this.angle == this.rtarget){
                    is_rotating = false;
                    this.sdelta = new Vector3(0,0,0);
                    this.smove_count = 0;
                }
            }
        }
        else{
            //zとxに応じて次の回転角を決める。
            if (z){
                this.rtarget = this.angle + 90;
                this.is_rotating = true;
                }
            else if(x){ //時計回転
                this.rtarget = this.angle - 90;
                this.is_rotating = true;
                }
            else{
                this.is_rotating = false;
            }

            if (this.rslide_flag){ //右にスライド
                this.sdelta.x = 1.0f * Configs.xmove;
                Debug.Log("aaa");
            }
            else if(this.lslide_flag){
                this.sdelta.x = -1.0f * Configs.xmove;
            }
            else if(this.uslide_flag){
                this.sdelta.y = 1.0f * Configs.ymove;
            }
            else if(this.unrotate_flag){
                this.sdelta.y = 1.0f * Configs.ymove;
            }
            else{
                Debug.Log("fuck");
            }
        }
        this.puyo.transform.Translate( sdelta.x,sdelta.y,0 );
        this.subpuyo.transform.position  = this.mainpuyo.transform.position + this.sub_delta;
    }


    //回転 & 回し & ずらし
    public void rotating(bool z,bool x){
        if (this.is_rotating){
            this.angle = Mathf.MoveTowards(this.angle,this.rtarget,Configs.rspeed*Time.deltaTime);
            this.sub_delta.x = Mathf.Round(Mathf.Cos(Mathf.Deg2Rad*this.angle)*100)/100;
            this.sub_delta.y = Mathf.Round(Mathf.Sin(Mathf.Deg2Rad*this.angle)*100)/100; 
    
            if(this.angle == this.rtarget){
                is_rotating = false;
            }
        }
        else{ 
            if (z){        
                this.rtarget = this.angle + 90;
                this.is_rotating = true;
                }
            else if(x){ //時計回転
                this.rtarget = this.angle - 90;
                this.is_rotating = true;
                }
            else{
                this.is_rotating = false;
            }
        }
        this.subpuyo.transform.position  = this.mainpuyo.transform.position + this.sub_delta;
    }

    //今のmainぷよの位置の四捨五入した値を返す。 (Mathで実装することで、銀行丸めを回避している。)
    private Vector3Int mainpos_round(){
        Vector3Int roundpos = new Vector3Int(0,0,0);
        var pos = this.puyo.transform.position;

        roundpos.x = (int)Math.Round( pos.x,MidpointRounding.AwayFromZero );
        roundpos.y = (int)Math.Round( pos.y,MidpointRounding.AwayFromZero );
        roundpos.z = (int)Math.Round( pos.z,MidpointRounding.AwayFromZero );

        return roundpos;
    }
    //今のsubぷよの位置の四捨五入した値を返す。
    private Vector3Int subpos_round(){
        Vector3Int roundpos = new Vector3Int(0,0,0);
        var subpos = this.puyo.transform.position + this.sub_delta;

        roundpos.x = (int)Math.Round( subpos.x,MidpointRounding.AwayFromZero );
        roundpos.y = (int)Math.Round( subpos.y,MidpointRounding.AwayFromZero );
        roundpos.z = (int)Math.Round( subpos.z,MidpointRounding.AwayFromZero );

        return roundpos;
    }

    private float plus_mod(float a,float b){
        return a- Mathf.Floor(a/b)*b;
    }

}
