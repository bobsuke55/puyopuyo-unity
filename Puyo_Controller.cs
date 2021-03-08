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

    private Puyo_pair[,] Field_puyos = new Puyo_pair[Configs.right+2,Configs.top+1];


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
                    break;
        }

        Play_checker.update_text(Play_puyo,hkey,vkey,zkey,xkey);
    }



}