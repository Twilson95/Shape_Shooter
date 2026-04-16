using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundUpdater : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite backgroundSprite1;
    public Sprite backgroundSprite2;
    public Sprite backgroundSprite3;
    public Sprite backgroundSprite4;
    public Sprite backgroundSprite5;


    public void UpdateBackground(int level)
    {
        if((level % 30) < 6)
        {
            spriteRenderer.sprite = backgroundSprite1;
            return;
        }
        if((level % 30) < 11)
        {
            spriteRenderer.sprite = backgroundSprite2;
            return;
        }
        if((level % 30) < 16)
        {
            spriteRenderer.sprite = backgroundSprite3;
            return;
        }
        if((level % 30) < 21)
        {
            spriteRenderer.sprite = backgroundSprite4;
            return;
        }
        if((level % 30) < 26)
        {
            spriteRenderer.sprite = backgroundSprite5;
            return;
        }
    }
}
