using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puyo_Controller : MonoBehaviour
{
    private Puyo_pair Play_puyo;
    private Board Play_board;
    private Checker Play_checker;
    
    private string status = "none";

    float hkey;
    float vkey;
    bool zkey;
    bool xkey;
    bool ekey;

    //private Puyo_pair[,] Field_puyos = new Puyo_pair[2*(Configs.width+2),2*(Configs.height+1];


    void Start(){
        Play_board = new Board();
        Play_checker = new Checker();
        
        status = "create";
    }

    void Update(){
        hkey = Input.GetAxis("Horizontal");
        vkey = Input.GetAxis("Vertical");
        zkey = Input.GetKeyDown(KeyCode.Z);
        xkey = Input.GetKeyDown(KeyCode.X);
        ekey = Input.GetKeyDown(KeyCode.E);

        //Debug.Log(status);
        switch (status){

                case "create":
                    Play_puyo = new Puyo_pair("1");
                    status = "moving";
                    break;

                case "moving":
                    Play_puyo.move(this.Play_board.Field_bool,vkey,hkey,zkey,xkey);
                
                    var pos = Vector3Int.RoundToInt(Play_puyo.puyo.transform.position);
                    Play_board.colorize_Borad((int)pos.x,(int)pos.y);

                    Play_checker.update_text(Play_puyo,hkey,vkey,zkey,xkey);

                    if (ekey){
                        status = "fix";
                    }                    
                    break;

                case "fix":
                    var mpos = Play_puyo.mainpos;
                    var spos = Play_puyo.subpos;

                    this.Play_board.Field_bool[mpos.x,mpos.y] = false;
                    this.Play_board.Field_bool[mpos.x+1,mpos.y] = false;
                    this.Play_board.Field_bool[mpos.x,mpos.y+1] = false;
                    this.Play_board.Field_bool[mpos.x+1,mpos.y+1] = false;

                    this.Play_board.Field_bool[spos.x,spos.y] = false;
                    this.Play_board.Field_bool[spos.x+1,spos.y] = false;
                    this.Play_board.Field_bool[spos.x,spos.y+1] = false;
                    this.Play_board.Field_bool[spos.x+1,spos.y+1] = false;

                    status = "create";
                    break;

                case "hoge":
                    Debug.Log("hogehoge");
                    break;
        }
        //Play_checker.update_text(Play_puyo,hkey,vkey,zkey,xkey);
    }
}