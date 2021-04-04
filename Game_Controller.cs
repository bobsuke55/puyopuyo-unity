using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Game_Controller : MonoBehaviour
{
    private Puyo_Controller Play_puyo;
    private Puyo_Controller next_Play_puyo;
    private Puyo_Controller nextnext_Play_puyo;

    private Board play_board;
    private Checker play_checker;
    private Score play_score;

    private string state = "none";

    float hkey;
    float vkey;
    bool zkey;
    bool xkey;
    bool ekey;

    private Vector3Int mpos;
    private Vector3Int spos;
    private int puyo_count = 0;

    public float old_time = 0;
    public float elapsed_time = 0;
    public float elapsed_create_time = 0;
    public float elapsed_erase_time = 0;

    public float fall_starttime = 0;
    public float fall_fintime = 0;


    //1個ずつのぷよへの参照は、リスト内にスタックしていくのがいいのかな。。。? そっちのがいい気がする。
    //private List<GameObject> Puyo_list = new List<GameObject>();

    //private Puyo_pair[,] field_puyos = new Puyo_pair[2*(Configs.width+2),2*(Configs.height+1];

    void Start(){
        play_board   = new Board();
        play_checker = new Checker();
        play_score   = new Score();
        Application.targetFrameRate = 60;

        state = "create";

        //this.next_Play_puyo     = new Puyo_Controller(puyo_count.ToString(),Configs.next_pos,    play_board.p_board);
        //this.nextnext_Play_puyo = new Puyo_Controller(puyo_count.ToString(),Configs.nextnext_pos,play_board.p_board);
    }

    void Update(){
        hkey = Input.GetAxis("Horizontal");
        vkey = Input.GetAxis("Vertical");
        xkey = Input.GetKeyDown(KeyCode.X);
        zkey = Input.GetKeyDown(KeyCode.Z);
        ekey = Input.GetKeyDown(KeyCode.E);

        this.elapsed_time += Time.deltaTime;

        //Debug.Log(status);
        switch (state){
            case "create":

                this.elapsed_create_time += Time.deltaTime;
                if  (this.elapsed_create_time>Configs.create_time){
                    
                    this.elapsed_create_time = 0;
                    this.puyo_count += 1;
                    //this.Play_puyo = this.next_Play_puyo;
                    //this.next_Play_puyo = this.nextnext_Play_puyo;
                    //this.nextnext_Play_puyo = new Puyo_Controller( puyo_count.ToString(),Configs.nextnext_pos,this.play_board.p_board );
                    this.Play_puyo = new Puyo_Controller(puyo_count.ToString(),Configs.init_pos,this.play_board.p_board);
                    this.Play_puyo.set_init_pos(Configs.init_pos);

                    //this.nextnext_Play_puyo.transform.position  =  Configs.nextnext_pos;
                    //Debug.Log(this.elapsed_time - this.old_time);
                    this.old_time = this.elapsed_time;
                    state = "move";
                }
                break;

            case "move":
                this.Play_puyo.move(this.play_board.field_bool,vkey,hkey,xkey,zkey);
                play_checker.update_text(Play_puyo,hkey,vkey,xkey,zkey);
                state = Play_puyo.state;

                if (this.Play_puyo.state == "split"){
                    this.play_board.set_field_puyo(Play_puyo.mpuyo);
                    this.play_board.set_field_puyo(Play_puyo.spuyo);
                    this.play_board.set_field_bool(Play_puyo.mpuyo);
                    this.play_board.set_field_bool(Play_puyo.spuyo);                    
                    this.Play_puyo = null;
                    state = "check_fall";
                }
                break;

            case "check_fall":
                play_board.check_fall_puyos();
                state = "fall";
                this.fall_starttime = Time.time;
                break;

            //GetComponent<Puyo>は重い処理らしいので毎フレーム呼び出すのは要検討だけど一旦これで
            case "fall":
                play_board.fall_puyos();                    
                if( !play_board.is_falling ){
                    Debug.Log(Time.time-this.fall_starttime);

                    play_board.update_field_puyo();
                    play_board.update_field_bool();
                    state = "chain";
                    this.elapsed_erase_time = 0;
                }
                break;

            case "chain":
                this.elapsed_erase_time += Time.deltaTime;
                //消せるぷよがなくなったときにcreateに遷移。
                play_board.check_erasepuyos();
                //play_board.set_puyo_images();

                if(play_board.is_erasing){ // ぷよを消せる。
                    if(this.elapsed_erase_time >= Configs.erase_time){
                        play_board.erase_puyos();
                        play_board.update_field_puyo();
                        play_board.update_field_bool();
                        play_score.update_text(play_board);
                        state = "check_fall";
                    }
                }
                else{
                    //初期化メソッドに突っ込むべきかな。
                    play_board.erase_chain_num = 0;
            
                    if ( play_board.field_puyo[2,11] ){
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
        //play_board.colorize_false();
        //play_board.colorize_field_puyo();
        //Debug.Log(state);
        //play_checker.update_text(Play_puyo,hkey,vkey,zkey,xkey);
    }
}