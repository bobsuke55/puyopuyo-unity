using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Game_Controller : MonoBehaviour
{
    private Puyo_Controller Play_puyo;
    private Puyo_Controller next_Play_puyo;
    private Puyo_Controller nextnext_Play_puyo;

    private Board Play_board;
    private Checker Play_checker;
    private Score Play_score;

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

    //private Puyo_pair[,] Field_puyos = new Puyo_pair[2*(Configs.width+2),2*(Configs.height+1];

    void Start(){
        Play_board = new Board();
        Play_checker = new Checker();
        Play_score = new Score();
        Application.targetFrameRate = 60;

        state = "create";

        this.next_Play_puyo     = new Puyo_Controller(puyo_count.ToString(),Configs.next_pos);
        this.nextnext_Play_puyo = new Puyo_Controller(puyo_count.ToString(),Configs.nextnext_pos);

    }

    void Update(){
        hkey = Input.GetAxis("Horizontal");
        vkey = Input.GetAxis("Vertical");
        zkey = Input.GetKeyDown(KeyCode.Z);
        xkey = Input.GetKeyDown(KeyCode.X);
        ekey = Input.GetKeyDown(KeyCode.E);

        this.elapsed_time += Time.deltaTime;

        //Debug.Log(status);
        switch (state){
            case "create":

                this.elapsed_create_time += Time.deltaTime;
                if  (this.elapsed_create_time>Configs.create_time){
                    
                    this.elapsed_create_time = 0;
                    this.puyo_count += 1;
                    this.Play_puyo = this.next_Play_puyo;
                    this.next_Play_puyo = this.nextnext_Play_puyo;
                    this.nextnext_Play_puyo = new Puyo_Controller( puyo_count.ToString(),Configs.nextnext_pos );

                    this.Play_puyo.puyo.transform.position      = Configs.init_pos;
                    this.next_Play_puyo.puyo.transform.position = Configs.next_pos;
                    //this.nextnext_Play_puyo.transform.position  =  Configs.nextnext_pos;


                    //Debug.Log(this.elapsed_time - this.old_time);
                    this.old_time = this.elapsed_time;

                    state = "move";
                }
                break;

            case "move":
                Play_puyo.move(this.Play_board.Field_bool,vkey,hkey,zkey,xkey);
                var pos = Vector3Int.RoundToInt(Play_puyo.puyo.transform.position);
                Play_checker.update_text(Play_puyo,hkey,vkey,zkey,xkey);
                state = Play_puyo.state;
                break;

            case "split":
                this.Play_board.set_Field_puyo(Play_puyo.mainpuyo);
                this.Play_board.set_Field_puyo(Play_puyo.subpuyo);
                this.Play_board.set_Field_bool(Play_puyo.mainpuyo);
                this.Play_board.set_Field_bool(Play_puyo.subpuyo);

                state = "check_fall";
                break;

            case "check_fall":
                Play_board.check_fall_puyos();
                state = "fall";
                this.fall_starttime = Time.time;
                break;

            //GetComponent<Puyo>は重い処理らしいので毎フレーム呼び出すのは要検討だけど一旦これで
            case "fall":
                Play_board.fall_puyos();                    
                if( !Play_board.is_falling ){
                    Debug.Log(Time.time-this.fall_starttime);
                    state = "fix";
                }
                break;

            case "fix":
                Play_board.update_Field_puyo();
                Play_board.update_Field_bool();
                state = "chain";

                this.elapsed_erase_time = 0;
                break;

            case "chain":
                this.elapsed_erase_time += Time.deltaTime;
                //消せるぷよがなくなったときにcreateに遷移。
                Play_board.check_erasepuyos();
                //Play_board.set_puyo_images();

                if(Play_board.is_erasing){ // ぷよを消せる。
                    if(this.elapsed_erase_time >= Configs.erase_time){
                        Play_board.erase_puyos();
                        Play_board.update_Field_puyo();
                        Play_board.update_Field_bool();
                        Play_score.update_text(Play_board);
                        
                        state = "check_fall";
                    }
                }
                else{
                    //初期化メソッドに突っ込むべきかな。
                    Play_board.erase_chain_num = 0;
            
                    if ( Play_board.Field_puyo[2,11] ){
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
        //Play_board.colorize_false();
        //Play_board.colorize_field_puyo();
        //Debug.Log(state);
        //Play_checker.update_text(Play_puyo,hkey,vkey,zkey,xkey);
    }
}