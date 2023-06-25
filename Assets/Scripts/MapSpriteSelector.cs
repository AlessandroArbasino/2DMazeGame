using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpriteSelector : MonoBehaviour
{
    public Sprite spu, spd, spr, spl, spUD, spRL, spUL, spDr, spDl, spULD, spRUL, spDL, spLDR, swpUDRL;

    public bool up, down, left, right;

    public int type;

    public Color normalColor, EnterColor;

    Color mainColor;
    SpriteRenderer rend;
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        mainColor = normalColor;

        PickSprite();
        PickColor();
    }

    private void PickColor()
    {
        if( type == 0)
        {
            mainColor = normalColor;
        }else if (type == 1)
        {
            mainColor = EnterColor;
        }
        rend.color = mainColor;
        //check transparency in inspector
    }

    private void PickSprite()
    {
        //switch for sprites
    }

    
}
