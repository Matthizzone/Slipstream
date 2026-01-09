using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraBehavior : MonoBehaviour
{
    public Transform subject;
    public Vector3 positionOffset;
    public Vector3 lookatOffset;

    Vector3 prev_subj_pos;
    Vector3 subj_vel;
    public Vector3 subj_vel_smooth;
    Vector3 subj_vel_smooth_vel;
    public float smoothTime = 1;


    public float rotationPower;

    public LayerMask GroundLayers;

    float lookUpAmt;

    private void Start()
    {
        subj_vel_smooth = transform.forward;
    }

    private void LateUpdate()
    {
        UpdateSubVelSmooth();
        CalculateLookDown();

        CalculateCameraPos();

        RightStickEffect();

        prev_subj_pos = subject.position;
    }



    void UpdateSubVelSmooth()
    {
        Vector3 new_subj_vel = subject.position - prev_subj_pos;
        new_subj_vel.y = 0;
        new_subj_vel /= Time.deltaTime;

        if (new_subj_vel.magnitude > 2f) subj_vel = new_subj_vel;

        bool auto_rotate = Input.GetKeyDown(KeyCode.RightShift);
        if (Gamepad.all.Count > 0) auto_rotate |= Gamepad.all[0].rightTrigger.ReadValue() > 0.5f;

        if (auto_rotate)
        {
            subj_vel_smooth = Vector3.SmoothDamp(subj_vel_smooth, subj_vel, ref subj_vel_smooth_vel, smoothTime);
            subj_vel_smooth = subj_vel_smooth.normalized;
        }
        else
        {
            subj_vel_smooth_vel = Vector3.zero;
        }
    }

    void CalculateCameraPos()
    {
        Vector3 targetAngleForward = subj_vel_smooth;
        Vector3 targetAngleRight = Vector3.Cross(targetAngleForward, Vector3.up);

        Vector3 offsetWithLookDown = Quaternion.AngleAxis(lookDownAmtSmooth, Vector3.right) * positionOffset;

        Vector3 cam_offset =
            targetAngleRight * offsetWithLookDown.x +
            Vector3.up * offsetWithLookDown.y +
            targetAngleForward * offsetWithLookDown.z;

        bool lookUp = Input.GetKey(KeyCode.UpArrow);
        if (Gamepad.all.Count > 0) lookUp |= Gamepad.all[0].rightStick.ReadValue().y > 0.5f;
        lookUpAmt = MattMath.FRIndepLerp(lookUpAmt, lookUp ? 1 : 0, 7);

        transform.position = subject.position + cam_offset;
        transform.LookAt(subject.position + lookatOffset + Vector3.up * 3 * lookUpAmt);
    }

    void RightStickEffect()
    {
        float rot_amt = (Input.GetKey(KeyCode.LeftArrow) ? 1 : 0) + (Input.GetKey(KeyCode.RightArrow) ? -1 : 0);
        if (Gamepad.all.Count > 0) rot_amt += Gamepad.all[0].rightStick.ReadValue().x;

        subj_vel_smooth = Quaternion.AngleAxis(rot_amt * Time.deltaTime * rotationPower, Vector3.up) * subj_vel_smooth;
    }





    const float LOOK_DOWN_HEIGHT_INFLUENCE = 0.03f;
    const float LOOK_DOWN_MAX_ROTATION = 60f;
    float lookDownAmt;
    float lookDownAmtSmooth;
    float lookDownAmtSmoothVel;

    void CalculateLookDown()
    {
        float height = 20;

        RaycastHit hit;
        if (Physics.Raycast(subject.position + Vector3.up * 0.5f, Vector3.down, out hit, 100, GroundLayers))
        {
            height = hit.distance;
        }

        lookDownAmt = 1 - Mathf.Exp(-LOOK_DOWN_HEIGHT_INFLUENCE * height);
        lookDownAmt *= LOOK_DOWN_MAX_ROTATION;


        lookDownAmtSmooth = Mathf.SmoothDamp(lookDownAmtSmooth, lookDownAmt, ref lookDownAmtSmoothVel, 0.2f);
    }
}
