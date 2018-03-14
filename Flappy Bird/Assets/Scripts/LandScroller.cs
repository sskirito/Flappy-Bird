using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandScroller : MonoBehaviour {

    float scrollSpeed;
    float maxScrollSpeed = 6f;
    float minScrollSpeed = 3f;
    float tileSizeZ = 6.7f;
    GameController controller;

    [HideInInspector]
    public static float maxSpeedOffset;

    private Vector3 startPosition;

    // Use this for initialization
    void Start() {
        startPosition = transform.position;
        controller = FindObjectOfType<GameController>();
    }

    // Update is called once per frame
    void Update() {
        float currentMaxScrollSpeed = maxScrollSpeed + maxSpeedOffset;
        if (controller.gameStarted && !controller.gameOver) {
            scrollSpeed = Mathf.Lerp(minScrollSpeed, currentMaxScrollSpeed, Difficulty.GetDifficulty());
            float newPosition = Mathf.Repeat((Time.time - Difficulty.gameStartTime + Difficulty.offset) * scrollSpeed, tileSizeZ);
            transform.position = startPosition + Vector3.left * newPosition;
        }
    }
}
