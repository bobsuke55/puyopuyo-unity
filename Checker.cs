using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checker : MonoBehaviour
{
    private GameObject c_Text;
    private Puyo_pair puyo_Pair;
    private TextMesh c_Text_component;
    private string tex;
    private GameObject puyo;

    public Checker(){
        init_Field();
    }

    public void init_Field(){ 
        //board の親object
        var prefab = (GameObject)Resources.Load ("checker/text") as GameObject;
        c_Text = (GameObject)Instantiate(prefab, new Vector3(-11,16,+0.01f), Quaternion.Euler(0, 0, 0));
        c_Text_component = c_Text.GetComponent<TextMesh>();

        tex = "angle:{0} \n"
            + "can_ymoving:{1} \n"
            + "can_xrightmoving:{2} \n"
            + "can_xleftmoving:{3} \n"
            + "lslide_flag:{4} \n"
            + "rslide_flag:{5} \n"
            + "uslide_flag:{6} \n"
            + "unrotate_flag:{7} \n"
            + "h:{8} "
            + "v:{9} \n"
            + "z:{10} \n"
            + "x:{11} \n"
            + "xtimeelapsed:{12} \n"
            + "ytimeelapsed:{13} \n"
            + "fixtimeelapsed:{14} \n"
            + "mainpos:{15} \n"
            + "subpos:{16} \n";
    }

    public void update_text(Puyo_pair puyo,float hkey,float vkey,bool zkey,bool xkey){
        c_Text_component.text=string.Format(tex,puyo.angle,puyo.can_ymoving,puyo.can_xrightmoving,puyo.can_xleftmoving,
                                puyo.lslide_flag,puyo.rslide_flag,puyo.uslide_flag,puyo.unrotate_flag,
                                hkey,vkey,zkey,xkey,puyo.xtimeElapsed,puyo.ytimeElapsed,puyo.fixtimeElapsed,
                                puyo.mainpos,puyo.subpos);
    }
}