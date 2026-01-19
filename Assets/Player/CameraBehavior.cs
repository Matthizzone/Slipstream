using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Transform Subject;
    public Vector3 Offset;
    public Vector3 LookatOffset;

    public Vector2 Y_Bounds;
    float max_mouse_sensitivity = 10f;

    Vector3 mouse_angle;

    float start_angle;

    private void OnEnable()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Start()
    {
        start_angle = Vector3.Angle(Vector3.forward, transform.forward);
        ResetAngle();
    }

    void Update()
    {
        transform.position = Subject.position + Offset;
        transform.LookAt(Subject.position + LookatOffset);
        transform.RotateAround(Subject.position, Vector3.up, mouse_angle.x);
        transform.RotateAround(Subject.position, transform.right, -mouse_angle.y);


        Vector3 mouse_delta = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
        mouse_angle += mouse_delta * max_mouse_sensitivity * GameState.mouse_sensitivity;
        if (mouse_angle.y > Y_Bounds.y) mouse_angle.y = Y_Bounds.y;
        if (mouse_angle.y < Y_Bounds.x) mouse_angle.y = Y_Bounds.x;


        // DELETE THISSSSS print
        if (Input.GetKeyDown(KeyCode.I))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        // DELETE THISSSSS print
        if (Input.GetKeyDown(KeyCode.U))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void ResetAngle()
    {
        mouse_angle.x = start_angle;
        mouse_angle.y = 0;
    }
}