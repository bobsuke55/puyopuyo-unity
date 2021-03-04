using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{

    public bool[,] Field_bool = new bool[Configs.right+2,Configs.top+1];

    //boardの親
    private GameObject p_Board;
    //boardの子
    private GameObject[,] c_Boards = new GameObject[Configs.right+2,Configs.top+1];
    // Start is called before the first frame update
    // Update is called once per frame

    public Board(){
        init_Field();
        return;
    }

    public void init_Field(){ 
        //board の親object
        this.p_Board = new GameObject("Boards");  
        
        var prefab = (GameObject)Resources.Load ("board/Quad") as GameObject;
        for (int x=0;x<Configs.right+2;++x){                
            for (int y=0;y<Configs.top+1;++y){
                this.Field_bool[x,y] = true;
                this.c_Boards[x,y] = (GameObject)Instantiate(prefab, new Vector3(x,y,+0.01f), Quaternion.Euler(0, 0, 0));
                this.c_Boards[x,y].transform.parent = p_Board.transform;
            }
        }

        //
        for (int x=0;x<Configs.right+2;++x){
            this.Field_bool[x,0] = false; //下端
            }
        
        for (int y=0;y<Configs.top+1;++y){
            this.Field_bool[0,y] = false; //
            this.Field_bool[Configs.right+1,y] = false; //
            }

        for (int x=0;x<Configs.right+2;++x){                
            for (int y=0;y<Configs.top+1;++y){
                if(this.Field_bool[x,y]){
                    this.c_Boards[x,y].GetComponent<Renderer>().material.SetColor("_Color",Color.gray);
                }
                else{
                    this.c_Boards[x,y].GetComponent<Renderer>().material.SetColor("_Color",Color.cyan);
                }
            }
        }
    }

    public void colorize_Borad(int i,int j){ 
        this.c_Boards[i,j].GetComponent<Renderer>().material.SetColor("_Color",Color.red);
    }
}