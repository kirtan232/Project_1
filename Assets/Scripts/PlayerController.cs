using UnityEngine;
using UnityEngine.InputSystem;

// Attach to the Player (a Capsule with a CharacterController, tagged "Player").
// WASD / arrows = walk, Shift = run, Space = jump, mouse = turn.
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float jumpHeight = 1.6f;
    public float gravity = -20f;
    public float turnSpeed = 3f;
    public float fallLimit = -10f;    // below this height the player has fallen off the level.

    CharacterController controller;
    Vector3 startPosition;
    Quaternion startRotation;
    float verticalVelocity = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        startPosition = transform.position;
        startRotation = transform.rotation;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (GameManager.Instance.gameEnded) return;

        // Turn left/right with the mouse (the camera is a child, so it follows).
        Keyboard kb = Keyboard.current;
        Mouse mouse = Mouse.current;
        if (kb == null || mouse == null) return;

        // Mouse delta is in pixels, so scale it down to feel like the old "Mouse X" axis.
        transform.Rotate(0f, mouse.delta.ReadValue().x * turnSpeed * 0.05f, 0f);

        // WASD and the arrow keys both drive the movement.
        float h = 0f, v = 0f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) h += 1f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) h -= 1f;
        if (kb.wKey.isPressed || kb.upArrowKey.isPressed) v += 1f;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed) v -= 1f;
        Vector3 move = transform.right * h + transform.forward * v;
        if (move.magnitude > 1f) move.Normalize();   // no faster diagonal movement.

        bool running = kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed;
        float speed = running ? runSpeed : walkSpeed;

        if (controller.isGrounded)
        {
            verticalVelocity = -2f;                  // small push down keeps us stuck to the ground.
            if (kb.spaceKey.wasPressedThisFrame)
            {
                // Physics formula: speed needed to reach jumpHeight under this gravity.
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = move * speed + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);

        // Falling off the level (gaps, bridges) costs a life.
        if (transform.position.y < fallLimit)
        {
            GameManager.Instance.LoseLife();
        }
    }

    // Called by GameManager after a life is lost.
    public void Respawn()
    {
        // A CharacterController must be disabled before it can be teleported.
        controller.enabled = false;
        transform.position = startPosition;
        transform.rotation = startRotation;
        controller.enabled = true;
        verticalVelocity = 0f;
    }
}
