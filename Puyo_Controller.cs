using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puyo_Controller : MonoBehaviour
{       
    //mainpuyoとsubpuyoへの参照。
    public GameObject mpuyo;
    public GameObject spuyo;
    public Puyo mpuyo_script;
    public Puyo spuyo_script;
    //puyo移動判定用のポジション。
    public Vector3Int mpos  = new Vector3Int(0,0,0);
    public Vector3Int spos  = new Vector3Int(0,0,0);

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
    public bool is_xmoving = false;
    public bool is_rotating = false;
    public bool is_sliding = false;

    public string state = "move";

    public Puyo_Controller(string num, Vector3 init_pos,GameObject p_board){

    　　//puyo生成
        GameObject puyoprefab = (GameObject)Resources.Load ("puyo/puyo") as GameObject;
        this.mpuyo = Instantiate (puyoprefab,new Vector3(0,0,0) , Quaternion.Euler(0, 0, 0));
        this.mpuyo.name  = $"mpuyo_{num}";
        this.spuyo = Instantiate (puyoprefab,new Vector3(0,0,0), Quaternion.Euler(0, 0, 0));
        this.spuyo.name  = $"spuyo_{num}";

        this.mpuyo.transform.parent  = p_board.transform;
        this.spuyo.transform.parent  = p_board.transform;

        this.mpuyo.transform.localPosition = init_pos;
        this.spuyo.transform.localPosition = init_pos+this.s_delta;

        this.mpos = Vector3Int.FloorToInt(init_pos);
        this.spos = Vector3Int.FloorToInt(init_pos+this.s_delta);

        this.mpuyo_script  = this.mpuyo.GetComponent<Puyo>();
        this.spuyo_script  = this.spuyo.GetComponent<Puyo>();

        this.mpuyo_script.puyo_color = UnityEngine.Random.Range(0,Configs.color_num);
        this.spuyo_script.puyo_color  = UnityEngine.Random.Range(0,Configs.color_num);

        this.mpuyo_script.set_color();
        this.spuyo_script.set_color();

        return;
    }
    ~Puyo_Controller(){
        Debug.Log("Good bye");
    }
    

    public void move(bool[,] Field_bool,float vkey, float hkey, bool xkey, bool zkey){  
        var ymove_timeflag = this.ytime_flag(vkey);

        //mainpuyoとsubpuyoの下は動けるか。
        var ymove_posflag = Field_bool[this.mpos.x,mpos.y-1]  & Field_bool[this.mpos.x+1,mpos.y-1]
                            &Field_bool[this.spos.x,spos.y-1] & Field_bool[this.spos.x+1,spos.y-1];
    
        if (ymove_posflag & ymove_timeflag){
            this.movey();
        }

        var xmove_timeflag = this.xtime_flag();

        var xleftmove_posflag  = Field_bool[this.mpos.x-1,this.mpos.y] & Field_bool[this.mpos.x-1,this.mpos.y+1]
                               &Field_bool[this.spos.x-1,this.spos.y] & Field_bool[this.spos.x-1,this.spos.y+1]
                               &(hkey < 0);

        var xrightmove_posflag = Field_bool[this.mpos.x+2,this.mpos.y] & Field_bool[this.mpos.x+2,this.mpos.y+1]
                               &Field_bool[this.spos.x+2,this.spos.y] & Field_bool[this.spos.x+2,this.spos.y+1]
                               &(hkey > 0);

        if( this.is_xmoving ){ //右移動
            this.movex_isxmoving();
        }
        else if( (xleftmove_posflag | xrightmove_posflag) & xmove_timeflag & !this.is_xmoving){
            this.movex(hkey);
        }

        //回転処理
        zkey = zkey & (!xkey); // xkeyとzkeyがtrueの場合は、zkeyはfalseになる。
        var angle_plus = this.plus_mod(this.angle,360f);

        bool leftposflag   =  Field_bool[this.mpos.x-1,this.mpos.y] & Field_bool[this.mpos.x-1,this.mpos.y+1]
                             &Field_bool[this.mpos.x-2,this.mpos.y] & Field_bool[this.mpos.x-2,this.mpos.y+1];
        bool rightposflag  =  Field_bool[this.mpos.x+2,this.mpos.y] & Field_bool[this.mpos.x+2,this.mpos.y+1]
                             &Field_bool[this.mpos.x+3,this.mpos.y] & Field_bool[this.mpos.x+3,this.mpos.y+1];
        bool downposflag   =  Field_bool[this.mpos.x,this.mpos.y-1] & Field_bool[this.mpos.x+1,this.mpos.y-1]
                             &Field_bool[this.mpos.x,this.mpos.y-2] & Field_bool[this.mpos.x+1,this.mpos.y-2];

        if (this.is_rotating | this.is_sliding){
            this.rotate_isrotating();
            this.slide_issliding();
        }
        else{
            if(xkey | zkey){
                if ( !(leftposflag | rightposflag) ){//挟まっている
                //2回おしたら180度回転
                Debug.Log("in between");
                }
                else{
                    Debug.Log("rotate");
                    //右側にsubpuyoをおさめるスペースあるか? 
                    var rslide_flag   = ( (zkey & angle_plus == 90)  | (xkey & angle_plus == 270) ) & !leftposflag;
                    //左側にsubpuyoをおさめるスペースがあるか?
                    var lslide_flag   = ( (xkey & angle_plus == 90)  | (zkey & angle_plus == 270) ) & !rightposflag;
                    //下側に回転ぷよをおさめるスペースがあるか?
                    var uslide_flag   = ( (zkey & angle_plus == 180) | (xkey & angle_plus == 0) ) & !downposflag;

                    this.rotate(xkey,zkey);
                    this.slide(rslide_flag,lslide_flag,uslide_flag);
                    }
            }
        }
        
        //下が設置している & 横移動していない時 & 回転していない時にカウントする。
        var mpos_onflag = !(Field_bool[this.mpos.x,mpos.y-1] | Field_bool[this.mpos.x+1,mpos.y-1]);
        var spos_onflag = !(Field_bool[this.spos.x,spos.y-1] | Field_bool[this.spos.x+1,spos.y-1]);
        var fix_counting_flag = (mpos_onflag | spos_onflag) 
                                &!(this.is_xmoving | this.is_rotating | this.is_sliding);

        if (fix_counting_flag){ // count 
            var fix_flag = this.fixtime_flag(vkey);
            if(fix_flag){
                this.mpuyo_script.set_pos(this.mpos);
                this.spuyo_script.set_pos(this.spos);
                this.state = "split";
            }
        }
    }
    private bool ytime_flag(float vkey){
        bool ymove_timeflag = false;

        if(vkey < 0){
            this.ytimeElapsed += Time.deltaTime*Configs.yspeed;
        }
        else{
            this.ytimeElapsed += Time.deltaTime;
        }
        ymove_timeflag = (this.ytimeElapsed > Configs.ytime);
        return ymove_timeflag;
    }

    private bool xtime_flag(){
        bool xmove_timeflag = false;
        this.xtimeElapsed += Time.deltaTime;
        xmove_timeflag = (this.xtimeElapsed > Configs.xtime);
        return xmove_timeflag;
    }

    private bool fixtime_flag(float vkey){
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



    private void rotate(bool xkey,bool zkey){　
        var angle_plus = this.plus_mod(this.angle,360f);
        this.is_rotating = true;

        if(xkey){ 
            this.target_angle = this.angle - 90;
        }
        else if(zkey){
            this.target_angle = this.angle + 90;
        }

        //回転後のsub_delta
        Vector3 next_s_delta = new Vector3(0,0,0);
        next_s_delta.x = 2*Mathf.Round(Mathf.Cos(Mathf.Deg2Rad*this.target_angle)*100)/100;
        next_s_delta.y = 2*Mathf.Round(Mathf.Sin(Mathf.Deg2Rad*this.target_angle)*100)/100;
        this.spos.x = this.mpos.x + (int)next_s_delta.x;
        this.spos.y = this.mpos.y + (int)next_s_delta.y;
    }

    private void rotate_isrotating(){

        this.angle = Mathf.MoveTowards(this.angle,this.target_angle,Configs.rspeed*Time.deltaTime);
        s_delta.x = 2*Mathf.Round(Mathf.Cos(Mathf.Deg2Rad*this.angle)*100)/100;
        s_delta.y = 2*Mathf.Round(Mathf.Sin(Mathf.Deg2Rad*this.angle)*100)/100;
        this.spuyo.transform.localPosition = (this.mpuyo.transform.localPosition + s_delta);

        if( this.angle == this.target_angle ){
            this.is_rotating = false;
        }
    }

    private void slide(bool rslide_flag,bool lslide_flag,bool uslide_flag){　
        this.is_sliding = true;

        if (rslide_flag){ //右にスライド。
            this.slide_delta.x = 1.0f * Configs.xmove;
        }
        else if(lslide_flag){ //左にスライド。
            this.slide_delta.x = -1.0f * Configs.xmove;
        }
        else if(uslide_flag){ //上にスライド。
            this.slide_delta.y = 1.0f * Configs.ymove;
        }

        this.mpuyo.transform.Translate(this.slide_delta.x,this.slide_delta.y,0);
        this.spuyo.transform.Translate(this.slide_delta.x,this.slide_delta.y,0);

        this.mpos.x += (int)this.slide_delta.x * 2;
        this.mpos.y += (int)this.slide_delta.y * 2;

        this.spos.x += (int)this.slide_delta.x * 2;
        this.spos.y += (int)this.slide_delta.y * 2;
    }

    private void slide_issliding(){
        this.mpuyo.transform.Translate((int)this.slide_delta.x,(int)this.slide_delta.y,0,this.mpuyo.transform.parent);
        this.spuyo.transform.Translate((int)this.slide_delta.x,(int)this.slide_delta.y,0,this.spuyo.transform.parent);

        this.smove_count += 1;
        if(this.smove_count >= Configs.smove_count){
            this.is_sliding = false;
            this.slide_delta = new Vector3(0,0,0);
        }
    }

    //縦移動
    private void movey(){
        this.ytimeElapsed = 0f;
        float delta_y = -1 * Configs.ymove;

        this.mpuyo.transform.Translate(0,(int)delta_y,0,this.mpuyo.transform.parent);
        this.spuyo.transform.Translate(0,(int)delta_y,0,this.spuyo.transform.parent);

        this.mpos.y += (int)delta_y;
        this.spos.y += (int)delta_y;
    }
    
    //右移動 2フレームかけて動くためにdelta.xに移動方向を格納。
    private void movex(float hkey){
        this.delta.x = Mathf.Sign(hkey) * Configs.xmove;
        this.is_xmoving = true;

        this.mpuyo.transform.Translate((int)this.delta.x,0,0,this.mpuyo.transform.parent);
        this.spuyo.transform.Translate((int)this.delta.x,0,0,this.spuyo.transform.parent);

        //GameObjectは2フレームかけて2マス移動するが、判定は1フレームで2マス分動かす。
        this.mpos.x += (int)delta.x * 2;
        this.spos.x += (int)delta.x * 2;
    }

    private void movex_isxmoving(){
        this.mpuyo.transform.Translate((int)this.delta.x,0,0,this.mpuyo.transform.parent);
        this.spuyo.transform.Translate((int)this.delta.x,0,0,this.spuyo.transform.parent);

        this.xmove_count += 1;
        if(this.xmove_count >= Configs.xmove_count){
            this.is_xmoving = false;
            this.delta.x = 0;
            this.xtimeElapsed = 0;
        }
    }

    private float plus_mod(float a,float b){
        return a-Mathf.Floor(a/b)*b;
    }
    public void set_init_pos(Vector3 init_pos){

        this.mpuyo.transform.localPosition = init_pos;
        this.spuyo.transform.localPosition = init_pos + this.s_delta;

        this.mpos = Vector3Int.FloorToInt(init_pos);
        this.spos = Vector3Int.FloorToInt(init_pos+this.s_delta);

        Debug.Log("fucking");
    }



}