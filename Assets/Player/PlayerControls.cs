using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public float MOVE_FORCE = 200f;
    public float MOVE_DRAG = 5f;
    public float JUMP_POWER = 80f;
    public float TILT_POWER = 100f;

    public float TILT_CORRECTION = 10f;

    Rigidbody Torso;
    Rigidbody Wheel;

    public static PlayerControls instance;

    bool is_grounded;


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        Torso = transform.Find("Torso").GetComponent<Rigidbody>();
        Wheel = transform.Find("Wheel").GetComponent<Rigidbody>();
    }

    private void Update()
    {
        GetUp();
    }

    void FixedUpdate()
    {
        GroundCheck();

        Tilting();
        Move();
        ApplyDrag();

        WheelControl();
    }

    void GroundCheck()
    {
        is_grounded = Wheel.GetComponent<WheelBehavior>().IsGrounded();
    }

    void Tilting()
    {
        bool squatting = InputManager.instance.LT() < 0.5f;
        bool is_getting_up = Time.time - last_getup_time < 1f;

        if (is_grounded || is_getting_up || squatting)
        {
            // get back to vertical

            // make it 


            Vector3 target_angle = Vector3.up;
            Vector3 target_angle_tilt_axis = Vector3.Cross(InputManager.instance.LeftStickCamera(), Vector3.up);
            target_angle = Quaternion.AngleAxis(0, target_angle_tilt_axis) * target_angle;

            Debug.DrawRay(Torso.position, target_angle);

            Vector3 torq_axis = Vector3.Cross(Torso.transform.up, target_angle);

            Torso.AddTorque(torq_axis * TILT_CORRECTION, ForceMode.Force);
        }
    }

    void Move()
    {
        if (is_grounded)
        {
            Vector3 move_dir = InputManager.instance.LeftStickCamera();

            Torso.AddForce(move_dir * MOVE_FORCE, ForceMode.Force);
        }
        else
        {
            Vector3 tilt_axis = InputManager.instance.LeftStickCamera();
            tilt_axis = Vector3.Cross(Vector3.up, tilt_axis);

            Torso.AddTorque(tilt_axis * TILT_POWER, ForceMode.Force);
        }
    }

    void ApplyDrag()
    {
        if (!is_grounded) return;

        Vector3 flat_vel = Torso.velocity;
        flat_vel.y = 0;
        flat_vel = MattMath.FRIndepLerp(flat_vel, Vector3.zero, MOVE_DRAG);

        Torso.velocity = new Vector3(
            flat_vel.x,
            Torso.velocity.y,
            flat_vel.z);
    }


    float last_getup_time = -Mathf.Infinity;
    float getup_time = 0.5f;

    void GetUp()
    {
        if (!InputManager.instance.Pressed(InputManager.Buttons.A)) return;

        // check if on the floor
        if (Vector3.Angle(Torso.transform.up, Vector3.up) < 75f) return;

        if (Time.time - last_getup_time < getup_time) return;

        Torso.velocity = Vector3.zero;
        Torso.AddForce(Vector3.up * JUMP_POWER, ForceMode.Impulse);
        last_getup_time = Time.time;
    }

    void WheelControl()
    {
        Vector3 wheel_pos = new Vector3(
            0, 
            Mathf.Lerp(0.5f, 1.2f, 1 - InputManager.instance.LT()),
            0);
        Wheel.GetComponent<ConfigurableJoint>().targetPosition = wheel_pos;
    }


    public Vector3 GetPos()
    {
        return Torso.position;
    }
}
