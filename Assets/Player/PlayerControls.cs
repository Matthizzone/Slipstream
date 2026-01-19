using Unity.VisualScripting;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    Rigidbody rb;

    public float MOVE_POWER = 40f;
    public float SIDE_POWER = 10f;
    public float BRAKE_POWER = 30f;
    public float POWER_DISSIPATION = 0.0165f;
    public float JUMP_POWER = 12f;
    public float DASH_POWER = 20f;
    public float CAM_DASH_POWER = 15f;

    public static PlayerControls instance;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = Mathf.Infinity;
    }

    void Update()
    {
        UsePowerups();
    }

    void UsePowerups()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //if (jumps == 0) return;

            jumps--;

            // reset velocity ONLY if moving down
            if (rb.velocity.y < 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            }

            rb.AddForce(Vector3.up * JUMP_POWER, ForceMode.Impulse);
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            rb.velocity = Vector3.zero;

            Vector3 force_vec = MattMath.Flatten(Camera.main.transform.forward).normalized;
            rb.AddForce(force_vec * DASH_POWER, ForceMode.Impulse);
        }

        if (Input.GetMouseButtonDown(1))
        {
            rb.velocity = Vector3.zero;

            Vector3 force_vec = Camera.main.transform.forward;
            rb.AddForce(force_vec * CAM_DASH_POWER, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        ApplyMoveForces();

        //RollSound(); // runs more often
    }

    void ApplyMoveForces()
    {
        // convert WASD to 3D vector
        Vector3 cam_right = Camera.main.transform.forward;

        Vector3 right_vec = Vector3.Cross(Vector3.up, cam_right);
        Vector3 forward_vec = Vector3.Cross(right_vec, Vector3.up);

        Vector3 input_vec = Vector3.zero;
        if (Input.GetKey(KeyCode.A)) input_vec -= right_vec;
        if (Input.GetKey(KeyCode.D)) input_vec += right_vec;
        if (Input.GetKey(KeyCode.W)) input_vec += forward_vec;
        if (Input.GetKey(KeyCode.S)) input_vec -= forward_vec;
        if (input_vec.magnitude > 0.1f) input_vec.Normalize();


        // split input according to current motion

        Vector3 flat_vel = MattMath.Flatten(rb.velocity);
        Vector3 vel_input = Vector3.Project(input_vec, flat_vel);
        Vector3 side_input = input_vec - vel_input;

        Vector3 force_vec = Vector3.zero;

        if (Vector3.Dot(vel_input, rb.velocity) > 0)
        {
            // acceleration weakens with speed

            force_vec += vel_input * MOVE_POWER * Mathf.Pow(1 + POWER_DISSIPATION, -flat_vel.magnitude);
        }
        else
        {
            // braking is always powerful

            force_vec += vel_input * BRAKE_POWER;
        }

        // add in side forces
        force_vec += side_input * SIDE_POWER;

        


        rb.AddForce(force_vec, ForceMode.Force);
    }


    float last_roll_sfx_time;

    void RollSound()
    {
        //if (!grounded) return;

        if (Time.time > last_roll_sfx_time + Mathf.Pow(1.08f, -rb.velocity.magnitude) + 0.1f)
        {
            AudioManager.instance.ResetValues();
            AudioManager.instance.SetPitch(0.1f + rb.velocity.magnitude * 0.02f);
            AudioManager.instance.SetVol(Mathf.Min(rb.velocity.magnitude * 0.005f, 0.5f));
            AudioManager.instance.PlaySound("Ring/Roll", false);

            last_roll_sfx_time = Time.time;
        }
    }

    int jumps = 0;

    public void AddJump()
    {
        jumps++;
    }
}
