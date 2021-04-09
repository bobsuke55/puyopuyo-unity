using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Score : MonoBehaviour
{
    public int chain_num = 0;
    public int total_score = 0;
    
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

        tex = "総スコア:{0} \n";
/*
        tex = "連鎖数:{0} \n"
            + "色数:{1} \n"
            + "ぷよ数:{2} \n"
            + "スコア:{3} \n"
            + "総スコア:{4} \n";
*/
    }

    public void UpdateText(Board Play_board){
        c_Text_component.text=
        string.Format(tex,this.total_score);
    }


    public void CalculateScore(List<Board.EraseInfo> eraseinfo_list){
        var color_set = new HashSet<int>(); //消える色の数
        int erase_sum =  0; //消したぷよの数
        int link_bonus = 0; //連結ボーナス

        foreach(Board.EraseInfo eraseinfo in eraseinfo_list){
            color_set.Add(eraseinfo.color);
            var link_num = eraseinfo.chainlist.Count;
            link_bonus += Configs.link_bonus[link_num-4];
            erase_sum  += link_num;
        }

        var color_bonus = Configs.color_bonus[color_set.Count-1];
        var chain_bonus = Configs.chain_bonus[this.chain_num];
        var bonus_score = link_bonus + color_bonus + chain_bonus;
        bonus_score = Mathf.Clamp(bonus_score,1,1000);

        var erase_bonus = 10 * erase_sum;
        var score = erase_bonus*(bonus_score);
        this.total_score += score;
    }

    public void AddChainnum(){
        this.chain_num+=1;
    }
}
