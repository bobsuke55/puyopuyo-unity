using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;


public class Board : MonoBehaviour
{

    //ボードとぷよの親座標。
    public GameObject p_board;

    public bool[,] field_bool = new bool[Configs.board_width,Configs.board_height];
    public GameObject[,] field_puyo = new GameObject[Configs.width,Configs.height];

    private GameObject[,]   c_boards = new GameObject[Configs.board_width,Configs.board_height];
    private Renderer[,]     c_boards_r = new Renderer[Configs.board_width,Configs.board_height];
    public List<Vector2Int> chainind_list = new List<Vector2Int>();

    public bool is_falling = false;
    public bool is_erasing = false;
    public int  sum_deltas = 0;
    public int erase_chain_num = 0;
    public int erase_color = 0;

    public List<int> erase_link_list;
    public List<int> erase_color_list = new List<int>();
    public List<List<Vector2Int>> erase_ind_lists = new List<List<Vector2Int>>();
    public List<Vector2Int> checked_puyo_lists = new List<Vector2Int>();

    public Board(){
        init_Field();
        return;
    }

    public void init_Field(){ 
        //board の親object
        this.p_board = new GameObject("boards");
        
        var prefab = (GameObject)Resources.Load ("board/Board") as GameObject;
        for (int x=0;x<Configs.board_width;++x){
            for (int y=0;y<Configs.board_height;++y){
                this.field_bool[x,y] = true;
                this.c_boards[x,y] = (GameObject)Instantiate(prefab, new Vector3(0,0,0), Quaternion.Euler(0, 0, 0));
                this.c_boards[x,y].transform.parent = p_board.transform;
                this.c_boards[x,y].transform.localPosition = new Vector3(x,y,0.01f);
                this.c_boards_r[x,y] = c_boards[x,y].transform.Find("Quad").gameObject.GetComponent<Renderer>();
            }
        }

        //
        for (int x=0;x<Configs.board_width;++x){
            this.field_bool[x,0] = false; //下端
            this.field_bool[x,1] = false; //下端
            }
        
        for (int y=0;y<Configs.board_height;++y){
            this.field_bool[0,y] = false; //左端
            this.field_bool[1,y] = false; //

            this.field_bool[Configs.board_width-2,y] = false; //右端
            this.field_bool[Configs.board_width-1,y] = false; //
        }

        for (int y=0;y<Configs.board_height;++y){
            this.field_bool[0,y] = false; //左端
            this.field_bool[1,y] = false; //

            this.field_bool[Configs.board_width-2,y] = false; //右端
            this.field_bool[Configs.board_width-1,y] = false; //
        }


        this.colorize_false();
        //PrefabUtility.SaveAsPrefabAsset(p_board,"Assets/Prefabs/hoge.prefab");

    }

    public void colorize_board(int i,int j){ 
        this.c_boards[i,j].GetComponentInChildren<Renderer>().material.SetColor("_Color",Color.red);
    }

    public void set_field_bool(GameObject puyo){
        var pos = Vector3Int.FloorToInt( puyo.GetComponent<Puyo>().pos );
        this.field_bool[pos.x,pos.y]     = false;
        this.field_bool[pos.x+1,pos.y]   = false;
        this.field_bool[pos.x,pos.y+1]   = false;
        this.field_bool[pos.x+1,pos.y+1] = false;

    }

    //puyoをfield_puyo上に設置する。
    public void set_field_puyo(GameObject puyo){
        var pos = Vector3Int.FloorToInt(puyo.GetComponent<Puyo>().pos);
        this.field_puyo[pos.x/2-1,pos.y/2-1] = puyo;
    }


    //puyoの位置を参照して、width 欠ける heightの1マスにpuyoへの参照をおく。
    //field_puyo上のindexと実際のぷよの位置が異なる場合、field_puyo上に再配置。fall()のあととか。
    public void update_field_puyo(){
        for (int x=0;x<Configs.width;++x){
            for (int y=0;y<Configs.height;++y){            
                if (this.field_puyo[x,y] != null){
                    Vector2Int pos = (Vector2Int)this.field_puyo[x,y].GetComponent<Puyo>().pos;
                    var ind = new Vector2Int(pos.x/2-1,pos.y/2-1);
                    if ( (ind.x != x) | (ind.y != y) ){ 
                        this.field_puyo[ind.x,ind.y] = this.field_puyo[x,y];
                        this.field_puyo[x,y] = null;
                    }
                }
            }
        }
    }


    //field_puyoのindexに従って、field_boolを更新する。
    public void update_field_bool(){
        for (int x=0;x<Configs.width;++x){
            for (int y=0;y<Configs.height;++y){
                if (this.field_puyo[x,y] != null){
                    this.field_bool[2*(x+1),2*(y+1)]     = false;
                    this.field_bool[2*(x+1)+1,2*(y+1)]   = false;
                    this.field_bool[2*(x+1),2*(y+1)+1]   = false;
                    this.field_bool[2*(x+1)+1,2*(y+1)+1] = false;
                }
                else{
                    this.field_bool[2*(x+1),2*(y+1)]     = true;
                    this.field_bool[2*(x+1)+1,2*(y+1)]   = true;
                    this.field_bool[2*(x+1),2*(y+1)+1]   = true;
                    this.field_bool[2*(x+1)+1,2*(y+1)+1] = true;
                }
            }
        }
    }


    public void check_fall_puyos(){
        this.is_falling = true;
        this.sum_deltas = 0;

        for (int x=0;x<Configs.width;++x){
            for (int y=0;y<Configs.height;++y){
                if (this.field_puyo[x,y] != null){
                    this.field_puyo[x,y].GetComponent<Puyo>().check_fall(this.field_bool);
                    this.sum_deltas +=this.field_puyo[x,y].GetComponent<Puyo>().fall_deltay;
                }
            }
        }
    }

    public void fall_puyos(){
        var bool_list = new List<bool>();

        for (int x=0;x<Configs.width;++x){
            for (int y=0;y<Configs.height;++y){
                if (this.field_puyo[x,y] != null){
                    this.field_puyo[x,y].GetComponent<Puyo>().fall_puyo();
                    bool_list.Add(this.field_puyo[x,y].GetComponent<Puyo>().is_falling);
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
                if(this.field_bool[x,y]){
                    this.c_boards_r[x,y].material.SetColor("_Color",Color.gray);
                }
                else{
                    this.c_boards_r[x,y].material.SetColor("_Color",Color.cyan);
                }
            }
        }
        this.c_boards_r[2*(2+1),  (12)*2].material.SetColor("_Color",Color.red);
        this.c_boards_r[2*(2+1)+1,(12)*2].material.SetColor("_Color",Color.red);
        this.c_boards_r[2*(2+1),  (12)*2+1].material.SetColor("_Color",Color.red);
        this.c_boards_r[2*(2+1)+1,(12)*2+1].material.SetColor("_Color",Color.red);
    }


    public void colorize_field_puyo(){
        for (int x=0;x<Configs.width;++x){
            for (int y=0;y<Configs.height;++y){
                UnityEngine.Color color = Color.black;
                if(this.field_puyo[x,y] != null){
                    var cls = this.field_puyo[x,y].GetComponent<Puyo>().puyo_color;
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
            this.c_boards_r[2*(x+1),2*(y+1)].material.SetColor("_Color",color);
            this.c_boards_r[2*(x+1)+1,2*(y+1)].material.SetColor("_Color",color);
            this.c_boards_r[2*(x+1),2*(y+1)+1].material.SetColor("_Color",color);
            this.c_boards_r[2*(x+1)+1,2*(y+1)+1].material.SetColor("_Color",color);
            }
        }
    }
    

    public void check_erasepuyos(){ //listに従ってメモリから消せばいいのかな。
        this.is_erasing = false;

        this.erase_color_list = new List<int>();
        this.erase_link_list  = new List<int>();
        this.erase_ind_lists  = new List<List<Vector2Int>>();
        this.checked_puyo_lists = new List<Vector2Int>();

        for (int x=0;x<Configs.width;++x){
            for (int y=0;y<Configs.height;++y){
                this.chainind_list  = new List<Vector2Int>();

                if(this.field_puyo[x,y] != null){
                    this.check_chainpuyo(new Vector2Int(x,y)); //
                    this.field_puyo[x,y].GetComponent<Puyo>().set_image();
                }
                //Confings.erase_num以上繋がっている。
                if(this.chainind_list.Count >= Configs.erase_num ){
                    this.is_erasing = true;
                    this.erase_ind_lists.Add(this.chainind_list);

                    foreach(Vector2Int vec in this.chainind_list){
                        this.field_puyo[vec.x,vec.y].GetComponent<Puyo>().set_blinking();
                    }
                    if ( !this.erase_color_list.Contains( this.erase_color ) ){
                        this.erase_color_list.Add(this.erase_color);
                    }
                }
            }
        }
    }

    //
    private void check_chainpuyo(Vector2Int pos){
        Vector2Int[] direction = new Vector2Int[4] {
        new Vector2Int(0,1),new Vector2Int(1,0),new Vector2Int(0,-1),new Vector2Int(-1,0)};

        //既に含まれていたら終わり。
        if ( this.chainind_list.Contains( pos ) | this.checked_puyo_lists.Contains( pos ) ){
            return;
        }
        this.chainind_list.Add( pos );
        this.checked_puyo_lists.Add( pos );

        foreach(var d in direction){
            //はみ出る
            if ( (0>pos.x+d.x) | (pos.x+d.x>Configs.width-1) | (0 > pos.y+d.y) | (pos.y+d.y > Configs.height-1) ){
                continue;
            }
            //ぷよがない
            if(this.field_puyo[pos.x+d.x,pos.y+d.y] == null){
                continue;
            }
            //色が異なる
            this.erase_color  = this.field_puyo[pos.x,pos.y].GetComponent<Puyo>().puyo_color;
            var ncolor = this.field_puyo[pos.x+d.x,pos.y+d.y].GetComponent<Puyo>().puyo_color;

            if (this.erase_color != ncolor ){
                continue;
            }
            this.field_puyo[pos.x,pos.y].GetComponent<Puyo>().set_sideflag(d);
            this.field_puyo[pos.x+d.x,pos.y+d.y].GetComponent<Puyo>().set_sideflag(-d);
            this.check_chainpuyo(pos+d);
        }
    }


    //puyoを消す & animationはどこ?
    public void erase_puyos(){
        this.erase_chain_num += 1;　//連鎖数を足す。
        foreach(List<Vector2Int> erase_puyos in this.erase_ind_lists){
            foreach(Vector2Int vec in erase_puyos){
                Destroy( this.field_puyo[vec.x,vec.y] );
                this.field_puyo[vec.x,vec.y] = null;
            }
        }
    }

    public void set_puyo_images(){
        for (int x=0;x<Configs.width;++x){
            for (int y=0;y<Configs.height;++y){
                this.field_puyo[x,y].GetComponent<Puyo>().set_image();
            }
        }
    }

/*
                if(this.chainind_list.Count >= Configs.erase_num ){
                    this.is_erasing = true;

                    this.erase_link_list.Add( this.chainind_list.Count );
                    if ( !this.erase_color_list.Contains( this.erase_color ) ){
                        this.erase_color_list.Add(this.erase_color);
                    }
                    foreach(Vector2Int vec in this.chainind_list){
                        Destroy( this.field_puyo[vec.x,vec.y] );
                        this.field_puyo[vec.x,vec.y] = null;
                    }
                }
*/

}