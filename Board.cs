using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;


public class Board : MonoBehaviour
{

    public bool[,] Field_bool = new bool[Configs.board_width,Configs.board_height];
    public GameObject[,] Field_puyo = new GameObject[Configs.width,Configs.height];

    //boardの親
    private GameObject p_Board;
    //boardの子
    private GameObject[,] c_Boards = new GameObject[Configs.board_width,Configs.board_height];
    private Renderer[,]   c_Boards_r = new Renderer[Configs.board_width,Configs.board_height];

    public List<Vector2Int> chainind_list = new List<Vector2Int>();
    public List<GameObject> chainpuyo_list = new List<GameObject>();

    public bool is_falling = false;

    public Board(){
        init_Field();
        return;
    }

    public void init_Field(){ 
        //board の親object
        this.p_Board = new GameObject("Boards");
        
        var prefab = (GameObject)Resources.Load ("board/Board") as GameObject;
        for (int x=0;x<Configs.board_width;++x){                
            for (int y=0;y<Configs.board_height;++y){
                this.Field_bool[x,y] = true;
                this.c_Boards[x,y] = (GameObject)Instantiate(prefab, new Vector3(x,y,+0.01f), Quaternion.Euler(0, 0, 0));
                this.c_Boards[x,y].transform.parent = p_Board.transform;
                this.c_Boards_r[x,y] = c_Boards[x,y].transform.Find("Quad").gameObject.GetComponent<Renderer>();
            }
        }

        //
        for (int x=0;x<Configs.board_width;++x){
            this.Field_bool[x,0] = false; //下端
            this.Field_bool[x,1] = false; //下端
            }
        
        for (int y=0;y<Configs.board_height;++y){
            this.Field_bool[0,y] = false; //左端
            this.Field_bool[1,y] = false; //

            this.Field_bool[Configs.board_width-2,y] = false; //右端
            this.Field_bool[Configs.board_width-1,y] = false; //
        }

        this.colorize_false();

        //PrefabUtility.SaveAsPrefabAsset(p_Board,"Assets/Prefabs/hoge.prefab");

    }

    public void colorize_Borad(int i,int j){ 
        this.c_Boards[i,j].GetComponentInChildren<Renderer>().material.SetColor("_Color",Color.red);
    }

    //Puyo_listの位置情報に従って、Field_puyoを更新する。
    /*
    public void update_Field_puyo(List<GameObject> Puyo_list){
        this.Field_puyo = new GameObject[Configs.width,Configs.height];
        foreach(GameObject item in Puyo_list){
            var pos = Vector3Int.FloorToInt(item.GetComponent<Puyo>().transform.position);
            this.Field_puyo[pos.x/2-1,pos.y/2-1] = item;
        }
    }

*/  
    public void set_Field_bool(GameObject puyo){
        var pos = Vector3Int.FloorToInt( puyo.transform.position );

        this.Field_bool[pos.x,pos.y]     = false;
        this.Field_bool[pos.x+1,pos.y]   = false;
        this.Field_bool[pos.x,pos.y+1]   = false;
        this.Field_bool[pos.x+1,pos.y+1] = false;

    }

    //puyoをField_puyo上に設置する。
    public void set_Field_puyo(GameObject puyo){
        var pos = Vector3Int.FloorToInt(puyo.GetComponent<Puyo>().transform.position);
        this.Field_puyo[pos.x/2-1,pos.y/2-1] = puyo;
    }


    //puyoの位置を参照して、width 欠ける heightの1マスにpuyoへの参照をおく。
    public void update_Field_puyo(){
        for (int x=0;x<Configs.width;++x){
            for (int y=0;y<Configs.height;++y){            
                if (this.Field_puyo[x,y] != null){
                    var pos = Vector2Int.FloorToInt( this.Field_puyo[x,y].transform.position );
                    var ind = new Vector2Int(pos.x/2-1,pos.y/2-1);
                    if ( (ind.x != x) | (ind.y != y) ){ //Field_puyo上のindexと実際のぷよの位置が異なる場合、Field_puyo上に再配置。fall()のあととか。
                        this.Field_puyo[ind.x,ind.y] = this.Field_puyo[x,y];
                        this.Field_puyo[x,y] = null;
                    }
                }
            }
        }
    }


    //Field_puyoのindexに従って、Field_boolを更新する。
    public void update_Field_bool(){
        for (int x=0;x<Configs.width;++x){
            for (int y=0;y<Configs.height;++y){
                if (this.Field_puyo[x,y] != null){
                    this.Field_bool[2*(x+1),2*(y+1)]     = false;
                    this.Field_bool[2*(x+1)+1,2*(y+1)]   = false;
                    this.Field_bool[2*(x+1),2*(y+1)+1]   = false;
                    this.Field_bool[2*(x+1)+1,2*(y+1)+1] = false;
                }
                else{
                    this.Field_bool[2*(x+1),2*(y+1)]     = true;
                    this.Field_bool[2*(x+1)+1,2*(y+1)]   = true;
                    this.Field_bool[2*(x+1),2*(y+1)+1]   = true;
                    this.Field_bool[2*(x+1)+1,2*(y+1)+1] = true;
                }
            }
        }
    }


    public void check_fall(){
        this.is_falling = true;

        for (int x=0;x<Configs.width;++x){
            for (int y=0;y<Configs.height;++y){
                if (this.Field_puyo[x,y] != null){
                    this.Field_puyo[x,y].GetComponent<Puyo>().check_fall(this.Field_bool);
                }
            }
        }
    }

    public void fall(){
        var bool_list = new List<bool>();

        for (int x=0;x<Configs.width;++x){
            for (int y=0;y<Configs.height;++y){
    
                if (this.Field_puyo[x,y] != null){
                    this.Field_puyo[x,y].GetComponent<Puyo>().fall_puyo();
                    bool_list.Add(this.Field_puyo[x,y].GetComponent<Puyo>().is_falling);

                }
            }
        }

        if( bool_list.All(i => i == false ) ){
            this.is_falling = false;
        }
    }


    public void colorize_false(){
        for (int x=0;x<Configs.board_width;++x){
            for (int y=0;y<Configs.board_height;++y){
                if(this.Field_bool[x,y]){
                    this.c_Boards_r[x,y].material.SetColor("_Color",Color.gray);
                }
                else{
                    this.c_Boards_r[x,y].material.SetColor("_Color",Color.cyan);
                }
            }
        }
    }


    public void colorize_field_puyo(){
        for (int x=0;x<Configs.width;++x){
            for (int y=0;y<Configs.height;++y){
                UnityEngine.Color color = Color.black;
                if(this.Field_puyo[x,y] != null){
                    var cls = this.Field_puyo[x,y].GetComponent<Puyo>().puyo_color;
                    if (cls == 0){
                        color = Color.red;
                    }else if(cls == 1){
                        color = Color.green;
                    }else if(cls == 2){
                        color = Color.blue;
                    }else if(cls == 3){
                        color = Color.yellow;
                    }
                }
            this.c_Boards_r[2*(x+1),2*(y+1)].material.SetColor("_Color",color);
            this.c_Boards_r[2*(x+1)+1,2*(y+1)].material.SetColor("_Color",color);
            this.c_Boards_r[2*(x+1),2*(y+1)+1].material.SetColor("_Color",color);
            this.c_Boards_r[2*(x+1)+1,2*(y+1)+1].material.SetColor("_Color",color);
            }
        }
    }
    
    //
    private void check_chainpuyo(Vector2Int pos){
        Vector2Int[] direction = new Vector2Int[4] {
        new Vector2Int(0,1),new Vector2Int(1,0),new Vector2Int(0,-1),new Vector2Int(-1,0)};

        //既に含まれていたら終わり。
        if ( this.chainind_list.Contains( pos ) ){
            return;
        }

        this.chainpuyo_list.Add( this.Field_puyo[pos.x,pos.y] );
        this.chainind_list.Add( pos );

        foreach(var d in direction){
            //はみ出る
            if ( (0>pos.x+d.x) | (pos.x+d.x>Configs.width-1) | (0 > pos.y+d.y) | (pos.y+d.y > Configs.height-1) ){
                continue;
            }
            //ぷよがない
            if(this.Field_puyo[pos.x+d.x,pos.y+d.y] == null){
                continue;
            }
            //色が異なる
            var color = this.Field_puyo[pos.x,pos.y].GetComponent<Puyo>().puyo_color;
            var ncolor = this.Field_puyo[pos.x+d.x,pos.y+d.y].GetComponent<Puyo>().puyo_color;
            Debug.Log(string.Format("color:{0},ncolor{1}",color,ncolor) );

            if (color != ncolor ){
                continue;
            }

            this.check_chainpuyo(pos+d);
        }
    }

    public void erase_puyo(){ //listに従ってメモリから消せばいいのかな。

        for (int x=0;x<Configs.width;++x){
            for (int y=0;y<Configs.height;++y){ 
                this.chainpuyo_list = new List<GameObject>();
                this.chainind_list  = new List<Vector2Int>();

                if(this.Field_puyo[x,y] != null){
                    this.check_chainpuyo(new Vector2Int(x,y)); // 
                }

                //Confings.erase_num以上繋がっているなら、
                //this.cnainpuyo_list内のpuyoを消す
                if(this.chainpuyo_list.Count >= Configs.erase_num ){
                    foreach(GameObject item in this.chainpuyo_list){
                        Destroy( item );
                    }
                }
            }
        }
    }

}