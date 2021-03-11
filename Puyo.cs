using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puyo : MonoBehaviour
{
    public Vector3Int pos;
    public int fall_starty;
    public int fall_deltay;
    public float fall_time;
    public float puyo_class;
    public float puyo_color;
    public bool is_falling = true;

    //instance
    public Puyo(){
        

    }

    public void set_fall_deltay(int deltay){
        this.fall_deltay = deltay;
        this.is_falling = true;
    }

    public void check_fall(bool[,] Field_bool){
        this.is_falling = true;
        this.pos   = Vector3Int.FloorToInt(this.transform.position);
        this.fall_starty = (int)this.transform.position.y;
        this.fall_deltay = 0;
        this.fall_time   = 0;

        while (Field_bool[this.pos.x,this.pos.y-this.fall_deltay-1]){
            this.fall_deltay += 1;
        }
    }

    public void fall_puyo(){
        this.fall_time += Time.deltaTime;
        var target_pos = this.fall_starty-this.fall_deltay;
        var pos = this.transform.position;

        pos.y = Mathf.Lerp(this.fall_starty,target_pos,
        this.fall_time/(Configs.fall_time*this.fall_deltay) );
        this.transform.position = pos;

        if (pos.y == target_pos){
            this.is_falling = false;
        }
    }
}