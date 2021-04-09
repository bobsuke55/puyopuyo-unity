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
    public Puyo[,] field_puyo = new Puyo[Configs.width,Configs.height];
    public Puyo_Controller[] next_puyos = new Puyo_Controller[Configs.next_num+1];

    private GameObject[,]   c_boards = new GameObject[Configs.board_width,Configs.board_height];
    private Renderer[,]     c_boards_r = new Renderer[Configs.board_width,Configs.board_height];

    public bool is_falling = false;
    public bool is_erasing = false;
    public int  sum_deltas = 0;

    public List<EraseInfo> eraseinfo_list;

    public Board(){
        this.init_Field();
        this.InitializePuyo();
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


        this.ColorizeFalse();
        //PrefabUtility.SaveAsPrefabAsset(p_board,"Assets/Prefabs/hoge.prefab");

    }



    //puyoをfield_puyo上に設置する。
    public void SetFieldPuyo(Puyo puyo){
        var pos = puyo.Pos;
        this.field_puyo[pos.x/2-1,pos.y/2-1] = puyo;

        this.field_bool[pos.x,pos.y]     = false;
        this.field_bool[pos.x+1,pos.y]   = false;
        this.field_bool[pos.x,pos.y+1]   = false;
        this.field_bool[pos.x+1,pos.y+1] = false;
    }


    //puyoの位置を参照して、width 欠ける heightの1マスにpuyoへの参照をおく。
    public void UpdateFieldPuyo(){
        for (int x=0;x<Configs.width;++x){
            for (int y=0;y<Configs.height;++y){
                if ( this.field_puyo[x,y] is object ){
                    Vector2Int pos = (Vector2Int)this.field_puyo[x,y].Pos;
                    var ind = new Vector2Int(pos.x/2-1,pos.y/2-1);
                    if ( (ind.x != x) | (ind.y != y) ){ 
                        this.field_puyo[ind.x,ind.y] = this.field_puyo[x,y];
                        this.field_puyo[x,y] = null;
                    }
                }
            }
        }
    }

    public void InitializePuyo(){ 
        for (int x=0;x<Configs.next_num;++x){ 
            this.next_puyos[x] = new Puyo_Controller(Configs.next_pos[x+1],this.p_board);
        }
    }

    public void CreateNextPuyo(){ 
        this.next_puyos[Configs.next_num] = new Puyo_Controller( Configs.next_pos[Configs.next_num+1],this.p_board );
    }

    public void MoveNextPuyos(float t){
        Vector3 startpos;
        Vector3 endpos;
        Vector3 nextpos;

        for (int x=0;x<Configs.next_num+1;++x){ // 0,1,2
            startpos = Configs.next_pos[x+1];
            endpos   = Configs.next_pos[x];
            nextpos  = Vector3.Lerp(startpos,endpos,t);
            next_puyos[x].MoveNext(nextpos);
        }
    }

    public Puyo_Controller CreatePuyo(){
        var playpuyo = this.next_puyos[0];
        for (int x=0;x<Configs.next_num;++x){ 
            this.next_puyos[x] = this.next_puyos[x+1];
        }
        return playpuyo;
    }

    public void UpdateFieldBool(){
        for (int x=0;x<Configs.width;++x){
            for (int y=0;y<Configs.height;++y){
                if ( this.field_puyo[x,y] is object ){
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


    public void CheckFallPuyos(){
        this.is_falling = false;

        for (int x=0;x<Configs.width;++x){
            for (int y=0;y<Configs.height;++y){
                if (this.field_puyo[x,y] is object){
                    this.field_puyo[x,y].CheckFall(this.field_bool);
                    this.sum_deltas +=this.field_puyo[x,y].fall_deltay;
                }
            }
        }
        this.is_falling = (this.sum_deltas > 0);;
    }

    public void FallPuyos(){
        var bool_list = new List<bool>();
        for (int x=0;x<Configs.width;++x){
            for (int y=0;y<Configs.height;++y){
                if (this.field_puyo[x,y] is object){
                    this.field_puyo[x,y].FallPuyo();
                    bool_list.Add(this.field_puyo[x,y].is_falling);
                }
            }
        }
        if( bool_list.All(i => i == false ) ){
            this.is_falling = false;
        }
    }


    public void ColorizeFalse(){
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

    public struct EraseInfo{
        public int color;
        public List<Vector2Int> chainlist;

        public EraseInfo(int color,List<Vector2Int> chainlist){
            this.color = color;
            this.chainlist = chainlist;
        }
    }


    //puyoを消す & animationはどこ?
    public void ErasePuyos(){
        foreach(EraseInfo eraseinfo in this.eraseinfo_list){
            foreach(Vector2Int vec in eraseinfo.chainlist){
                this.field_puyo[vec.x,vec.y].DestroyPuyo();
                Destroy( this.field_puyo[vec.x,vec.y] );
                this.field_puyo[vec.x,vec.y] = null;
            }
        }
    }

    public void CheckErasePuyos(){
        this.is_erasing = false;
        var checked_cnainlist  = new List<Vector2Int>();
        var eraseinfo = new EraseInfo();
        this.eraseinfo_list = new List<EraseInfo>();
 
        for (int x=0;x<Configs.width;++x){
            for (int y=0;y<Configs.height;++y){
                var chainlist  = new List<Vector2Int>();
                var pos = new Vector2Int(x,y);
                if( this.field_puyo[x,y] is Object ){
                    if(!checked_cnainlist.Contains(pos)){
                        var erase_color = this.field_puyo[x,y].puyo_color;
                        chainlist = this.CheckChainPuyo(chainlist,pos);
                        checked_cnainlist.AddRange(chainlist);
                        eraseinfo = new EraseInfo(erase_color,chainlist);
                    }
                }
                if(chainlist.Count >= Configs.erase_num ){
                    this.is_erasing = true;
                    this.eraseinfo_list.Add(eraseinfo);

                    foreach(Vector2Int vec in chainlist){
                        this.field_puyo[vec.x,vec.y].SetBlinking();
                    }
                }
            }
        }
    }

    private List<Vector2Int> CheckChainPuyo(List<Vector2Int> chainlist,Vector2Int pos){
        Vector2Int[] direction = new Vector2Int[4] {
        new Vector2Int(0,1),new Vector2Int(1,0),new Vector2Int(0,-1),new Vector2Int(-1,0)};

        if ( chainlist.Contains( pos ) ){
            return chainlist;
        }
        chainlist.Add( pos );

        foreach(var d in direction){
            //はみ出る
            if ( (0>pos.x+d.x) | (pos.x+d.x>Configs.width-1) | (0 > pos.y+d.y) | (pos.y+d.y > Configs.height-1) ){
                continue;
            }
            //ぷよがない
            if(this.field_puyo[pos.x+d.x,pos.y+d.y] is null){
                continue;
            }
            //色が異なる
            var color  = this.field_puyo[pos.x,pos.y].puyo_color;
            var adjcolor = this.field_puyo[pos.x+d.x,pos.y+d.y].puyo_color;

            if (color != adjcolor ){
                continue;
            }
            this.field_puyo[pos.x,pos.y].SetAdj(d);
            this.field_puyo[pos.x+d.x,pos.y+d.y].SetAdj(-d);
            chainlist = this.CheckChainPuyo(chainlist,pos+d);
        }
        return chainlist;
    }


    public void SetPuyoImages(){
        for (int x=0;x<Configs.width;++x){
            for (int y=0;y<Configs.height;++y){
                if(this.field_puyo[x,y] is object){
                    this.field_puyo[x,y].SetImage();
                }
            }
        }
    }
    
    public void colorize_board(int i,int j){ 
        this.c_boards[i,j].GetComponentInChildren<Renderer>().material.SetColor("_Color",Color.red);
    }
}