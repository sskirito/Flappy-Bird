using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPicController : MonoBehaviour {
	
	void Start () {
        SpriteRenderer spr = gameObject.GetComponent<SpriteRenderer>();
        System.Random rand = new System.Random();
        Texture2D texture2d = (Texture2D)Resources.Load("monster1");//更换为红色主题英雄角色图片  
        int seed = rand.Next();
        if (seed % 100 < 50)
            texture2d = (Texture2D)Resources.Load("monster2");//更换为红色主题英雄角色图片  
        Sprite sp = Sprite.Create(texture2d, spr.sprite.textureRect, new Vector2(0.5f, 0.5f));//注意居中显示采用0.5f值     
        spr.sprite = sp;
    }
}
