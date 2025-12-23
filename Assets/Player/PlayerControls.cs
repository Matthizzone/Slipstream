using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public float MOVE_FORCE = 100f;
    public float MOVE_DRAG = 5f;
    public float JUMP_POWER = 100f;

    Rigidbody Torso;

    public static PlayerControls instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        Torso = transform.Find("Torso").GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Jump();
    }

    void FixedUpdate()
    {
        Move();
        ApplyDrag();
    }

    void Move()
    {
        Vector3 move_dir = InputManager.instance.LeftStickCamera();

        Torso.AddForce(move_dir * MOVE_FORCE, ForceMode.Force);

        Vector3 flat_vel = Torso.velocity;
        flat_vel.y = 0;
        if (flat_vel.magnitude > 0.1f)
        {
            Torso.transform.LookAt(Torso.position + flat_vel);
        }
    }

    void ApplyDrag()
    {
        Vector3 flat_vel = Torso.velocity;
        flat_vel.y = 0;
        flat_vel = MattMath.FRIndepLerp(flat_vel, Vector3.zero, MOVE_DRAG);

        Torso.velocity = new Vector3(
            flat_vel.x,
            Torso.velocity.y,
            flat_vel.z);
    }

    void Jump()
    {
        if (!InputManager.instance.Pressed(InputManager.Buttons.A)) return;

        Torso.AddForce(Vector3.up * JUMP_POWER, ForceMode.Impulse);
    }


    public Vector3 GetPos()
    {
        return Torso.position;
    }
}
