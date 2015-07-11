using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour {

    public Transform direction;
    public float speed = 6.0f;
    public float jumpSpeed = 4.0f;
    public float gravity = 9.81f;
    
    private CharacterController controller;
    private Vector3 move = Vector3.zero;

    void Start() {
        controller = GetComponent<CharacterController>();
        if (direction == null)
            Debug.LogWarning("Direction is null, moving with world axis", this);
    }

    void Update() {
        if (isGrounded()) {
            move = getPlayerMove();
            move = getRotatedMoveForDirection(move);
        }

        move = getAppendedGravity(move);
        controller.Move(move * Time.deltaTime);
    }

    public void clearMove() {
        move = Vector3.zero;
        move = getAppendedGravity(move);
        controller.Move(move * Time.deltaTime);
    }

    private bool isGrounded() {
        return controller.isGrounded;
    }

    private Vector3 getPlayerMove() {
        Vector3 move;
        move.x = Input.GetAxis("Horizontal");
        move.y = 0;
        move.z = Input.GetAxis("Vertical");
        move *= speed;
        
        if (Input.GetButton("Jump"))
            move.y = jumpSpeed;
        
        return move;
    }

    private Vector3 getRotatedMoveForDirection(Vector3 move) {
        if (direction == null)
            // Brak kierunku ruchu, więc nic nie możemy zrobić
            return move;

        Vector3 rotation = direction.rotation.eulerAngles;
        return Quaternion.Euler(0, rotation.y, 0) * move;
    }

    private Vector3 getAppendedGravity(Vector3 move) {
        move.y -= gravity * Time.deltaTime;
        return move;
    }
}
