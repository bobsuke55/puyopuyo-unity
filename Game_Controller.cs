using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Game_Controller : MonoBehaviour
{
    private Puyo_Controller playpuyo;
    private Puyo_Controller[] nextplaypuyos = new Puyo_Controller[Configs.next_num];

    private Board playboard;
    private Checker playchecker;
    private Score playscore;

    private string state = "none";

    float hkey;
    float vkey;
    bool zkey;
    bool xkey;
    bool ekey;

    private Vector3Int mpos;
    private Vector3Int spos;

    public float old_time = 0;
    public float elapsed_time = 0;
    public float elapsed_createtime = 0;
    public float elapsed_erase_time = 0;

    public float fall_starttime = 0;
    public float fall_fintime = 0;

    void Start(){
        Application.targetFrameRate = 60;
        this.playboard   = new Board();
        this.playchecker = new Checker();
        this.playscore   = new Score();
        state = "create";
    }

    void Update(){
        hkey = Input.GetAxis("Horizontal");
        vkey = Input.GetAxis("Vertical");
        xkey = Input.GetKeyDown(KeyCode.X);
        zkey = Input.GetKeyDown(KeyCode.Z);
        ekey = Input.GetKeyDown(KeyCode.E);
        this.elapsed_time += Time.deltaTime;

        //Debug.Log(status);
        switch (this.state){

            case "create"://1フレーム
            
                this.playboard.CreateNextPuyo();
                this.state = "movenext";
                this.elapsed_createtime = 0;
                break;
        
            case "movenext"://Configs.create_time

                this.elapsed_createtime += Time.deltaTime;
                this.playboard.MoveNextPuyos(this.elapsed_createtime/Configs.create_time);

                if  (this.elapsed_createtime>Configs.create_time){
                    this.playpuyo = this.playboard.CreatePuyo();
                    this.playpuyo.SetInitPos(Configs.init_pos);
                    this.old_time = this.elapsed_time;
                    state = "move";
                }
                break;

            case "move"://設置するまで

                this.playpuyo.Move(this.playboard.field_bool,vkey,hkey,xkey,zkey);
                playchecker.update_text(this.playpuyo,hkey,vkey,xkey,zkey);
                state = this.playpuyo.state;

                if (this.playpuyo.state == "split"){
                    this.playboard.SetFieldPuyo(this.playpuyo.mpuyo);
                    this.playboard.SetFieldPuyo(this.playpuyo.spuyo);
                    //this.playpuyo = null;
                    state = "check_fall";
                }
                break;

            case "check_fall"://1フレーム

                this.playboard.CheckFallPuyos();
                if(playboard.is_falling){
                    state = "fall";
                    this.fall_starttime = Time.time;
                }
                else{
                    state = "chain";
                }
                
                break;

            case "fall":
                this.playboard.FallPuyos();                    
                if( !playboard.is_falling ){
                    Debug.Log(Time.time-this.fall_starttime);

                    playboard.UpdateFieldPuyo();
                    playboard.UpdateFieldBool();
                    state = "chain";
                    this.elapsed_erase_time = 0;
                }
                break;

            case "chain":
                this.elapsed_erase_time += Time.deltaTime;
                playboard.CheckErasePuyos();
                playboard.SetPuyoImages();

                if(playboard.is_erasing){ // ぷよを消せる。
                    if(this.elapsed_erase_time >= Configs.erase_time){
                        playboard.ErasePuyos();
                        playboard.UpdateFieldPuyo();
                        playboard.UpdateFieldBool();
                        playscore.CalculateScore(playboard.eraseinfo_list);
                        playscore.AddChainnum();
                        playscore.UpdateText(playboard);

                        state = "check_fall";
                    }
                }
                else{
                    if ( playboard.field_puyo[2,11] ){
                        state = "batan";
                    }
                    else{
                        state = "create";
                    }
                }
                break;

            case "batan":
                //Debug.Log("batan kyu");
                break;
            
        }
        Debug.Log(state);
        playboard.ColorizeFalse();
    }
}