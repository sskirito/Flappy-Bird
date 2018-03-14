using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PropPicController : MonoBehaviour {

    float speed = 3f;

    void Start() {
        SpriteRenderer spr = gameObject.GetComponent<SpriteRenderer>();
        System.Random rand = new System.Random();
        Texture2D texture2d = (Texture2D)Resources.Load("wenhao2");
        int seed = rand.Next();
        if (seed % 100 < 33)
            texture2d = (Texture2D)Resources.Load("wenhao3");
        else if(seed % 100 < 66) {
            texture2d = (Texture2D)Resources.Load("wenhao4");
        }
        Sprite sp = Sprite.Create(texture2d, spr.sprite.textureRect, new Vector2(0.5f, 0.5f));
        spr.sprite = sp;
    }

    void Update() {
        transform.Rotate(Vector3.up * speed);
    }
}
