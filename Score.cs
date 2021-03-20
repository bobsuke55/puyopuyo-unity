using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Score : MonoBehaviour
{
    public float total_score = 0;
    public float score = 0;
    public int chain_num = 0;
    public int link_num = 0;
    
    private GameObject c_Text;
    private Puyo_Controller Play_puyo;
    private TextMesh c_Text_component;
    private string tex;

    public Score(){
        init_Field();
    }

    public void init_Field(){ 
        //board の親object
        var prefab = (GameObject)Resources.Load ("checker/text") as GameObject;
        c_Text = (GameObject)Instantiate(prefab, new Vector3(-11,24,+0.01f), Quaternion.Euler(0, 0, 0));
        c_Text_component = c_Text.GetComponent<TextMesh>();

        tex = "連鎖数:{0} \n"
            + "色数:{1} \n"
            + "ぷよ数:{2} \n"
            + "スコア:{3} \n"
            + "総スコア:{4} \n";
    }
/*
    Play_board.erase_chain_num
    Play_board.erase_color_list
    Play_board.erase_link_list
    Play_board.erase_link_num
*/
    public void update_text(Board Play_board){
        var chain_num = Play_board.erase_chain_num;
        int link_color = Play_board.erase_color_list.Count;
        List<int> link_lists = new List<int>();
        int link_bonus = 0;
        int link_sum = 0;

        foreach(List<Vector2Int> erase_puyos in Play_board.erase_ind_lists){
            Debug.Log(erase_puyos);
            link_sum   += erase_puyos.Count;
            link_bonus += Configs.link_bonus[erase_puyos.Count - 4];
        }

        var color_bonus = Configs.color_bonus[link_color-1];
        var chain_bonus = Configs.chain_bonus[chain_num-1];

        var bonus_score = link_bonus + color_bonus + chain_bonus;

        if (bonus_score == 0){
            bonus_score = 1;
        }
        var score = link_sum*(bonus_score)*10;
        this.total_score += score;

        c_Text_component.text=
        string.Format(tex,chain_num,link_color,link_sum,score,total_score);
    }


    public void calculate_score(Board Play_board){



    }
}
