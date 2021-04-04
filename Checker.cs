using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checker : MonoBehaviour
{
    private GameObject c_Text;
    private Puyo_Controller Play_puyo;
    private TextMesh c_Text_component;
    private string tex;

    public Checker(){
        init_Field();
    }

    public void init_Field(){ 
        //board の親object
        var prefab = (GameObject)Resources.Load ("checker/text") as GameObject;
        c_Text = (GameObject)Instantiate(prefab, new Vector3(-11,16,+0.01f), Quaternion.Euler(0, 0, 0));
        c_Text_component = c_Text.GetComponent<TextMesh>();

        tex = "angle:{0} \n"
            + "h:{1} "
            + "v:{2} \n"
            + "z:{3} \n"
            + "x:{4} \n"
            + "xtimeelapsed:{5} \n"
            + "ytimeelapsed:{6} \n"
            + "fixtimeelapsed:{7} \n"
            + "mainpos:{8} \n"
            + "subpos:{9} \n"
            + "is_rotating:{10} \n"
            + "is_sliding:{11} \n"
            + "is_xmoving:{12} \n";
    }

    public void update_text(Puyo_Controller puyo,float hkey,float vkey,bool zkey,bool xkey){
        c_Text_component.text=string.Format(tex,puyo.angle,hkey,vkey,zkey,xkey,puyo.s_delta,puyo.ytimeElapsed,puyo.fixtimeElapsed,
                                           puyo.mpos,puyo.spos,puyo.is_rotating,puyo.is_sliding,puyo.is_xmoving);
    }
}