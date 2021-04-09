using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puyo_Controller : MonoBehaviour

{   
    public static int num = 0;
    //mainpuyoとsubpuyoへの参照。
    public Puyo mpuyo;
    public Puyo spuyo;

    //m puyoの移動差分
    public Vector3 delta = new Vector3Int(0,0,0);
    public Vector3 slide_delta = new Vector3(0,0,0);
    //m puyoと s puyoの位置差分
    public Vector3 s_delta = new Vector3(0,2,0);
    public float angle = 90;

    public float target_angle;

    public float xtimeElapsed=0;
    public float ytimeElapsed=0;
    public float fixtimeElapsed=0;

    private int xmove_count = 0;
    private int smove_count = 0;

//    public bool is_ymoving;
    public bool  is_xmoving = false;
    private float dirx_old;

    public bool is_rotating = false;
    public bool is_sliding = false;

    public string state = "move";

    public Puyo_Controller(Vector3 init_pos,GameObject p_board){
        
        num++;
        this.mpuyo = new Puyo($"mpuyo_{num}",init_pos,p_board);
        this.spuyo = new Puyo($"spuyo_{num}",init_pos+this.s_delta,p_board);

    }
    ~Puyo_Controller(){
        Debug.Log("Good bye");
    }

    public void Move(bool[,] Field_bool,float vkey, float hkey, bool xkey, bool zkey){

        //ymove
        var ymove_timeflag = this.YtimeFlag(vkey);
        var ymove_posflag = Field_bool[this.mpuyo.Pos.x,this.mpuyo.Pos.y-1] & Field_bool[this.mpuyo.Pos.x+1,this.mpuyo.Pos.y-1]
                           &Field_bool[this.spuyo.Pos.x,this.spuyo.Pos.y-1] & Field_bool[this.spuyo.Pos.x+1,this.spuyo.Pos.y-1];
    
        if (ymove_posflag & ymove_timeflag){
            float deltay =  -Configs.ymove_amount;

            this.mpuyo.MoveObj(0,deltay);
            this.spuyo.MoveObj(0,deltay);

            this.mpuyo.MovePos(0,deltay);
            this.spuyo.MovePos(0,deltay);
        }

        //xmove 
        var xmove_timeflag = this.XtimeFlag();

        var xleftmove_posflag  = Field_bool[this.mpuyo.Pos.x-1,this.mpuyo.Pos.y] & Field_bool[this.mpuyo.Pos.x-1,this.mpuyo.Pos.y+1]
                                &Field_bool[this.spuyo.Pos.x-1,this.spuyo.Pos.y] & Field_bool[this.spuyo.Pos.x-1,this.spuyo.Pos.y+1]
                                &(hkey < 0);

        var xrightmove_posflag =  Field_bool[this.mpuyo.Pos.x+2,this.mpuyo.Pos.y] & Field_bool[this.mpuyo.Pos.x+2,this.mpuyo.Pos.y+1]
                                 &Field_bool[this.spuyo.Pos.x+2,this.spuyo.Pos.y] & Field_bool[this.spuyo.Pos.x+2,this.spuyo.Pos.y+1]
                                 &(hkey > 0);

        if( this.is_xmoving ){ //右移動
            this.mpuyo.MoveObj(this.dirx_old*Configs.xmove_amount,0);
            this.spuyo.MoveObj(this.dirx_old*Configs.xmove_amount,0);

            this.xmove_count += 1;
            if(this.xmove_count >= Configs.xmove_count){
                this.is_xmoving = false;
                this.delta.x  = 0;
                this.xmove_count = 0;
                this.xtimeElapsed = 0;
            }
        }
        else if( (xleftmove_posflag | xrightmove_posflag) & xmove_timeflag & !this.is_xmoving ){

            var dirx = (int)Mathf.Sign(hkey);

            this.mpuyo.MoveObj(dirx*Configs.xmove_amount,0);
            this.spuyo.MoveObj(dirx*Configs.xmove_amount,0);

            this.mpuyo.MovePos(dirx*Configs.grid_num,0);
            this.spuyo.MovePos(dirx*Configs.grid_num,0);

            this.is_xmoving = true;
            this.xmove_count += 1;
            this.dirx_old = dirx;
        }

        //rotate and slide
        zkey = zkey & (!xkey); 
        var angle_plus = this.PlusMod(this.angle,360f);

        bool leftposflag   =  Field_bool[this.mpuyo.Pos.x-1,this.mpuyo.Pos.y] & Field_bool[this.mpuyo.Pos.x-1,this.mpuyo.Pos.y+1]
                             &Field_bool[this.mpuyo.Pos.x-2,this.mpuyo.Pos.y] & Field_bool[this.mpuyo.Pos.x-2,this.mpuyo.Pos.y+1];
        bool rightposflag  =  Field_bool[this.mpuyo.Pos.x+2,this.mpuyo.Pos.y] & Field_bool[this.mpuyo.Pos.x+2,this.mpuyo.Pos.y+1]
                             &Field_bool[this.mpuyo.Pos.x+3,this.mpuyo.Pos.y] & Field_bool[this.mpuyo.Pos.x+3,this.mpuyo.Pos.y+1];
        bool downposflag   =  Field_bool[this.mpuyo.Pos.x,this.mpuyo.Pos.y-1] & Field_bool[this.mpuyo.Pos.x+1,this.mpuyo.Pos.y-1]
                             &Field_bool[this.mpuyo.Pos.x,this.mpuyo.Pos.y-2] & Field_bool[this.mpuyo.Pos.x+1,this.mpuyo.Pos.y-2];

        if (this.is_rotating | this.is_sliding){
            this.RotateIsRotating();
            this.SlideIsSliding();
        }
        else{
            if(xkey | zkey){
                if ( !(leftposflag | rightposflag) ){
                //2回おしたら180度回転
                Debug.Log("in between");
                }
                else{
                    Debug.Log("rotate");
                    var rslide_flag   = ( (zkey & angle_plus == 90)  | (xkey & angle_plus == 270) ) & !leftposflag;
                    var lslide_flag   = ( (xkey & angle_plus == 90)  | (zkey & angle_plus == 270) ) & !rightposflag;
                    var uslide_flag   = ( (zkey & angle_plus == 180) | (xkey & angle_plus == 0) ) & !downposflag;

                    this.Rotate(xkey,zkey);
                    this.is_rotating = true;

                    this.Slide(rslide_flag,lslide_flag,uslide_flag);
                    this.is_sliding = true;
                }
            }
        }
        
        
        //下が設置している & 横移動していない時 & 回転していない時にカウントする。
        var mpos_onflag = !(Field_bool[this.mpuyo.Pos.x,this.mpuyo.Pos.y-1] | Field_bool[this.mpuyo.Pos.x+1,this.mpuyo.Pos.y-1]);
        var spos_onflag = !(Field_bool[this.spuyo.Pos.x,this.spuyo.Pos.y-1] | Field_bool[this.spuyo.Pos.x+1,this.spuyo.Pos.y-1]);
        var fix_counting_flag = (mpos_onflag | spos_onflag)
                                &!(this.is_xmoving | this.is_rotating | this.is_sliding);

        if (fix_counting_flag){ //count 
            var fix_flag = this.FixTimeFlag(vkey);
            if(fix_flag){
                this.state = "split";
            }
        }
    }

    private bool YtimeFlag(float vkey){
        bool ymove_timeflag = false;
        if(vkey < 0){
            this.ytimeElapsed += Time.deltaTime*Configs.yspeed;
        }
        else{
            this.ytimeElapsed += Time.deltaTime;
        }

        if(this.ytimeElapsed > Configs.ytime){
            ymove_timeflag = true;
            this.ytimeElapsed = 0;
        }
        return ymove_timeflag;
    }

    private bool XtimeFlag(){
        bool xmove_timeflag = false;
        this.xtimeElapsed += Time.deltaTime;

        if(this.xtimeElapsed > Configs.xmove_inputdelay){
            xmove_timeflag = true;
            this.xtimeElapsed = 0;
        }
        return xmove_timeflag;
    }

    private bool FixTimeFlag(float vkey){
        bool fix_flag = false;
        if(vkey < 0){
            this.fixtimeElapsed += Time.deltaTime*Configs.fixspeed;
        }
        else{
            this.fixtimeElapsed += Time.deltaTime;
        }
        fix_flag = (this.fixtimeElapsed > Configs.fixtime);
        return fix_flag;
    }

    private void Rotate(bool xkey,bool zkey){　
        var angle_plus = this.PlusMod(this.angle,360f);
        if(xkey){ 
            this.target_angle = this.angle - 90;
        }
        else if(zkey){
            this.target_angle = this.angle + 90;
        }

        Vector3Int next_s_delta = new Vector3Int(0,0,0);
        next_s_delta.x = (int)(Configs.puyo_width *Mathf.Round(Mathf.Cos(Mathf.Deg2Rad*this.target_angle)*100)/100);
        next_s_delta.y = (int)(Configs.puyo_height*Mathf.Round(Mathf.Sin(Mathf.Deg2Rad*this.target_angle)*100)/100);
        this.spuyo.SetPos( this.mpuyo.Pos + next_s_delta );
    }

    private void RotateIsRotating(){

        this.angle = Mathf.MoveTowards(this.angle,this.target_angle,Configs.rotate_speed*Time.deltaTime);
        s_delta.x = Configs.puyo_width *Mathf.Round(Mathf.Cos(Mathf.Deg2Rad*this.angle)*100)/100;
        s_delta.y = Configs.puyo_height*Mathf.Round(Mathf.Sin(Mathf.Deg2Rad*this.angle)*100)/100;
        this.spuyo.SetObj(this.mpuyo.Pos_g + s_delta);

        if( this.angle == this.target_angle ){
            this.is_rotating = false;
        }
    }

    private void Slide(bool rslide_flag,bool lslide_flag,bool uslide_flag){

        if (rslide_flag){ //右にスライド。
            this.slide_delta.x = 1.0f * Configs.xmove_amount;
        }
        else if(lslide_flag){ //左にスライド。
            this.slide_delta.x = -1.0f * Configs.xmove_amount;
        }
        else if(uslide_flag){ //上にスライド。
            this.slide_delta.y = 1.0f * Configs.ymove_amount;
        }

        this.mpuyo.MoveObj(this.slide_delta.x,this.slide_delta.y);
        this.spuyo.MoveObj(this.slide_delta.x,this.slide_delta.y);

        this.mpuyo.MovePos(this.slide_delta.x*Configs.grid_num,this.slide_delta.y*Configs.grid_num);
        this.spuyo.MovePos(this.slide_delta.x*Configs.grid_num,this.slide_delta.y*Configs.grid_num);
    }

    private void SlideIsSliding(){

        this.mpuyo.MoveObj(this.slide_delta.x,this.slide_delta.y);
        this.spuyo.MoveObj(this.slide_delta.x,this.slide_delta.y);

        this.smove_count += 1;
        if(this.smove_count >= Configs.smove_count){
            this.is_sliding = false;
            this.slide_delta = new Vector3(0,0,0);
        }
    }


    private float PlusMod(float a,float b){
        return a-Mathf.Floor(a/b)*b;
    }

    //動きのアニメーションをつける。
    public void SetInitPos(Vector3 init_pos){
        this.mpuyo.SetObj(init_pos);
        this.spuyo.SetObj(init_pos+ this.s_delta);

        this.mpuyo.SetPos(init_pos);
        this.spuyo.SetPos(init_pos+ this.s_delta);
    }

    //動きのアニメーションをつける。
    public void MoveNext(Vector3 nextpos){
        this.mpuyo.SetObj(nextpos);
        this.spuyo.SetObj(nextpos + this.s_delta);
    }
}