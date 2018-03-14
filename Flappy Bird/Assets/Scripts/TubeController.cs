using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeController : MonoBehaviour {

    [HideInInspector]
    public float moveSpeed;
    float hardestMoveSpeed = 6f;
    float easiestMoveSpeed = 3f;
    public PlayerController player;
    public GameController controller;
    [HideInInspector]
    public static float hardestMoveSpeedOffset;

    bool isScored;

	// Use this for initialization
	void Start () {
        player = FindObjectOfType<PlayerController>();
        controller = FindObjectOfType<GameController>();
        if (!gameObject.CompareTag("Tube")) {
            isScored = true;
        }
	}
	
	// Update is called once per frame
	void Update () {

        float currentHardestMoveSpeed = hardestMoveSpeed + hardestMoveSpeedOffset;
        if (controller.gameStarted && !controller.gameOver) {
            moveSpeed = Mathf.Lerp(easiestMoveSpeed, currentHardestMoveSpeed, Difficulty.GetDifficulty());

            Vector2 targetPosition = new Vector2(transform.position.x - moveSpeed * Time.deltaTime, transform.position.y);
            transform.position = targetPosition;
            if(transform.position.x < -8f) {
                Destroy(gameObject);
            }
            if(transform.position.x  < -6 && !isScored) {
                controller.score++;
                isScored = true;
                if (ClientSocket.connnetionSuccess) {
                    ClientSocket.EncryptSend(controller.score.ToString());
                }
            }
        }
	}
}
