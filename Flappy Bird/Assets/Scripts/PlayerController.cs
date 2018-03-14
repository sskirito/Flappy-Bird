using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float gravity = 10f;
    public float jumpForce = 10f;
    public float scaleExtendRatio;

    float timeToHighest;
    float timeToFall;

    float halfScreenHeight;
    float currentHeight;
    float currentTimeForFalling;
    float rotationZSmoothing;
    float targetRotationZ = -90f;
    float targetRotationZForUp = 25f;

    public event System.Action OnGameStartHints;
    public event System.Action OnGameOverHints;

    bool isFalling;
    bool isUpping;
    bool canTouch;
    
    GameController controller;

    Vector3 velocity;

    private void Start() {
        controller = FindObjectOfType<GameController>();
        scaleExtendRatio = transform.localScale.y;

        halfScreenHeight = Camera.main.orthographicSize;

        timeToHighest = jumpForce / gravity;

    }

    void Update () {

        SetGameStart();
        if(Input.touchCount == 0) {
            canTouch = true;
        }

        if (controller.gameStarted) {

            if (!controller.gameOver) {
                OnSpaceDown();
            }

            SetVelocityAndRotation();
        }

        EnsureBirdInBoundary();
    }

    private void EnsureBirdInBoundary() {
        if (transform.position.y < -halfScreenHeight + transform.localScale.y / 2 / scaleExtendRatio + 0.9f) {
            transform.position = new Vector3(transform.position.x, -halfScreenHeight + transform.localScale.y / 2 / scaleExtendRatio + 0.9f, transform.position.z);
            velocity.y = 0;
            if (!controller.gameOver) {
                SetGameOver();
            }
        }
        if (transform.position.y > halfScreenHeight - transform.localScale.y / 2 / scaleExtendRatio) {
            transform.position = new Vector3(transform.position.x, halfScreenHeight - transform.localScale.y / 2 / scaleExtendRatio, transform.position.z);
            velocity.y = 0;
        }
        if (transform.eulerAngles.z > 90 && transform.eulerAngles.z < 270) {
            transform.eulerAngles = new Vector3(0, 0, 270);
        }
    }

    private void SetVelocityAndRotation() {
        velocity.y += -gravity * Time.deltaTime;

        if(velocity.y < 2f && velocity.y >1.9f && transform.eulerAngles.z < 20f) {
            transform.eulerAngles = new Vector3(0, 0, 25f);
        }
        if(velocity.y <= 0 && isUpping) {
            isUpping = false;
        }
        if (velocity.y <= -jumpForce / 2 && !isFalling) {
            isFalling = true;
            isUpping = false;
            rotationZSmoothing = 0f;

        }
        if (isFalling) {
            float currentRotationZ = Mathf.SmoothDamp(transform.eulerAngles.z, targetRotationZ, ref rotationZSmoothing, timeToFall);
            transform.eulerAngles = new Vector3(0, 0, currentRotationZ);
        }

        if (isUpping) {
            if (transform.eulerAngles.z < 90) {
                float currentRotationZ = Mathf.SmoothDamp(transform.rotation.eulerAngles.z, targetRotationZForUp, ref rotationZSmoothing, timeToHighest / 4);
                transform.eulerAngles = new Vector3(0, 0, currentRotationZ);
            } else {
                float currentRotationZ = Mathf.SmoothDamp(transform.rotation.eulerAngles.z, 360, ref rotationZSmoothing, timeToHighest / 4);
                transform.eulerAngles = new Vector3(0, 0, currentRotationZ);
                if(transform.rotation.eulerAngles.z > 350f) {
                    transform.eulerAngles = Vector3.zero;
                }
            }
        }
        

        transform.Translate(velocity * Time.deltaTime, Space.World);
    }

    void SetGameStart() {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.touchCount == 1) && !controller.gameOver && !controller.gameStarted && controller.canGameStart) {
            controller.gameStarted = true;
            OnGameStartHints();
            Difficulty.gameStartTime = Time.time;
            controller.PlayGame.SetActive(false);
        }
    }

    void OnSpaceDown() {
        if (Input.GetKeyDown(KeyCode.Space) || Input.touchCount == 1 && canTouch) {
            canTouch = false;
            velocity.y = jumpForce;
            isFalling = false;
            isUpping = true;

            rotationZSmoothing = 0f;
            //transform.eulerAngles = Vector3.zero;

            currentHeight = transform.position.y - (-halfScreenHeight + transform.localScale.y / 2 / scaleExtendRatio);
            timeToFall = Mathf.Sqrt(2 * currentHeight / gravity) - timeToHighest/4;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Tube") && !controller.gameOver) {
            SetGameOver();
        }
        else if(collision.CompareTag("Dead") && !controller.gameOver) {
            SetGameOver();
        }else if(collision.CompareTag("Nice") && !controller.gameOver) {
            controller.score++;
        } else if(collision.CompareTag("Bad") && !controller.gameOver) {
            controller.score--;
        }else if(collision.CompareTag("SpeedUp") && !controller.gameOver){
            Difficulty.offset += 10;
        }else if(collision.CompareTag("SpeedDown") && !controller.gameOver) {
            Difficulty.offset = Difficulty.gameStartTime - Time.time;
        } 
        else if(collision.CompareTag("Random") && !controller.gameOver) {
            float random = Random.Range(0f, 1f);
            if(random < .01f) {
                SetGameOver();
            }else if(random < .15f) {
                controller.score--;
            }else if(random < .5f){
                controller.score++;
            }else if(random < .75f) {
                Difficulty.offset += 10;
                TubeController.hardestMoveSpeedOffset += 1f;
                LandScroller.maxSpeedOffset += 1f;
            }else {
                Difficulty.offset = Difficulty.gameStartTime - Time.time;
            }
        }

        if (!collision.CompareTag("Tube")) {
            Destroy(collision.gameObject);
        }
    }

    private void SetGameOver() {
        OnGameOverHints();
        controller.gameOver = true;
    }
}
