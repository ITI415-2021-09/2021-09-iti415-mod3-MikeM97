using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    Vector2 _mouseAbsolute;
    Vector2 _smoothMouse;

    public Vector2 clampInDegrees = new Vector2(360, 180);
    public Vector2 sensitivity = new Vector2(2, 2);
    public Vector2 smoothing = new Vector2(3, 3);
    public Vector2 targetDirection;
    public Vector2 targetCharacterDirection;

    // Assign this if there's a parent object controlling motion, such as a Character Controller.
    // Yaw rotation will affect this object instead of the camera if set.
    // Use this for initialization
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        mouseOrigin = Input.mousePosition;

        // Set target direction to the camera's initial orientation.
        targetDirection = transform.localRotation.eulerAngles;

        // Set target direction for the character body to its inital state.
        Vector3 vec = transform.localRotation.eulerAngles;
        targetCharacterDirection = vec;
    }

    public Vector3 mouseOrigin = new Vector3();
    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    public float turnSpeed = 200.0F;
    private Vector3 moveDirection = Vector3.zero;
    void Update()
    {

        CharacterController controller = GetComponent<CharacterController>();
        if (controller.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Vertical"), 0, -Input.GetAxis("Horizontal"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;

        }
        //else
        //{
        moveDirection.y -= gravity * Time.deltaTime;
        //moveDirection.y -= gravity;
        //}
        controller.Move(moveDirection * Time.deltaTime);
        //Camera/Rotation
        //Vector3 pos = GameObject.Find("Camera").GetComponent<Camera>().ScreenToViewportPoint((Input.mousePosition) - mouseOrigin);
        //GameObject.Find("Head").transform.RotateAround(GameObject.Find("Head").transform.position, transform.right, -pos.y * turnSpeed);
        //transform.RotateAround(transform.position, Vector3.up, pos.x * turnSpeed);
        //mouseOrigin = Input.mousePosition;
        // Ensure the cursor is always locked when set
        //if (lockCursor)
        //{
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //}

        // Allow the script to clamp based on a desired target value.
        var targetOrientation = Quaternion.Euler(targetDirection);
        var targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);

        // Get raw mouse input for a cleaner reading on more sensitive mice.
        var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        // Scale input against the sensitivity setting and multiply that against the smoothing value.
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

        // Interpolate mouse movement over time to apply smoothing delta.
        _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
        _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

        // Find the absolute mouse movement value from point zero.
        _mouseAbsolute += _smoothMouse;

        // Clamp and apply the local x value first, so as not to be affected by world transforms.
        if (clampInDegrees.x < 360)
            _mouseAbsolute.x = Mathf.Clamp(_mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);

        // Then clamp and apply the global y value.
        if (clampInDegrees.y < 360)
            _mouseAbsolute.y = Mathf.Clamp(_mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

        GameObject.Find("Head").transform.localRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right) * targetOrientation;

        // If there's a character body that acts as a parent to the camera
        var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, Vector3.up);
        transform.localRotation = yRotation * targetCharacterOrientation;
    }
}
