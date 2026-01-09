using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public float MOVE_FORCE = 200f;
    public float AIR_MOVE_FORCE = 30f;
    public float MOVE_DRAG = 5f;
    public float JUMP_POWER = 60f;
    public float WALL_POWER = 40f;

    Rigidbody rb;
    Animator anim;

    public static PlayerControls instance;

    bool is_grounded = false;
    LayerMask GroundLayers = 1 << 6;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        rb = transform.Find("Physics").GetComponent<Rigidbody>();
        anim = transform.Find("MYNBOT").GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateVisuals();

        Jump();
    }

    void UpdateVisuals()
    {
        // change between idle/walk/jog/run

        Vector3 flat_vel = rb.velocity;
        flat_vel.y = 0;
        anim.SetFloat("Speed", flat_vel.magnitude);
    }
    public float TURN_SPEED = 1f;

    void HandleTurning()
    {
        Vector3 flat_vel = rb.velocity;
        flat_vel.y = 0;

        Vector3 target_lookdir = flat_vel;

        // if small enough, just cancel
        if (target_lookdir.magnitude < 0.1f) return;

        Quaternion targetRotation = Quaternion.LookRotation(target_lookdir);

        // smoothly calculate the next step's rotation
        Quaternion newRotation = Quaternion.RotateTowards(
            rb.rotation,
            targetRotation,
            TURN_SPEED * Time.fixedDeltaTime
        );

        rb.MoveRotation(newRotation);
    }

    void FixedUpdate()
    {
        GroundCheck();
        WallCheck();

        Move();
        ApplyDrag();
        HandleTurning();
    }

    void GroundCheck()
    {
        RaycastHit hit;
        bool new_grounded = Physics.Raycast(rb.position + Vector3.up * 0.5f, Vector3.down, out hit, 1f, GroundLayers);

        if (new_grounded && !is_grounded)
        {
            Land();
        }
        else if (!new_grounded && is_grounded)
        {
            LeaveGround();
        }

        is_grounded = new_grounded;
    }

    float wall = 0f;         // Raw wall detection result (-1, 0, 1)
    float wall_anim = 0f;    // Smoothed wall value used by the animator

    void WallCheck()
    {
        RaycastHit hit;

        wall = 0f;

        Vector3 origin = rb.position + Vector3.up;

        // Check left side
        if (Physics.Raycast(origin, -rb.transform.right, out hit, 0.6f, GroundLayers))
        {
            wall = -1f;
        }
        // Check right side
        else if (Physics.Raycast(origin, rb.transform.right, out hit, 0.6f, GroundLayers))
        {
            wall = 1f;
        }

        // Smoothly move wall value toward target_wall
        // This is clearer and safer than manually subtracting the difference
        wall_anim = Mathf.MoveTowards(
            wall_anim,
            wall,
            Time.deltaTime * 5f
        );

        // Update animator parameter
        anim.SetFloat("Wall", wall_anim);
    }


    void Land()
    {
        anim.CrossFadeInFixedTime("GroundMove", 0.2f);

        AudioManager.instance.ResetValues();
        AudioManager.instance.SetRoundRobin(10);
        AudioManager.instance.SetVol(0.35f);
        AudioManager.instance.PlaySound("Footstep/Metal", false);
    }

    void LeaveGround()
    {
        anim.CrossFadeInFixedTime("Midair", 0.2f);
    }

    void Move()
    {
        float stick_mult = is_grounded ? MOVE_FORCE : AIR_MOVE_FORCE;

        Vector3 move_dir = InputManager.instance.LeftStickCamera();

        rb.AddForce(move_dir * stick_mult, ForceMode.Force);
    }

    void ApplyDrag()
    {
        if (!is_grounded) return;

        Vector3 flat_vel = rb.velocity;
        flat_vel.y = 0;
        flat_vel = MattMath.FRIndepLerp(flat_vel, Vector3.zero, MOVE_DRAG);

        rb.velocity = new Vector3(
            flat_vel.x,
            rb.velocity.y,
            flat_vel.z);
    }

    void Jump()
    {
        if (!InputManager.instance.Pressed(InputManager.Buttons.A)) return;
        
        if (!(is_grounded || Mathf.Abs(wall) > 0.1f)) return;

        rb.velocity = new Vector3(
            rb.velocity.x,
            0,
            rb.velocity.z);

        rb.AddForce(Vector3.up * JUMP_POWER, ForceMode.Impulse);

        // add horizontal force if against wall
        if (!is_grounded && Mathf.Abs(wall) > 0.1f)
        {
            rb.AddForce(-rb.transform.right * wall * WALL_POWER, ForceMode.Impulse);
        }

        AudioManager.instance.ResetValues();
        AudioManager.instance.SetVol(0.35f);
        AudioManager.instance.SetRoundRobin(4);
        AudioManager.instance.PlaySound("Movement/LegSwoosh", false);
    }

    public Vector3 GetPos()
    {
        return rb.position;
    }
}
