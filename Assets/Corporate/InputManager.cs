using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    bool keyboard = false; // Maybe implement this better later????


    public enum Buttons { A, B, X, Y, LB, RB, DPAD_up, DPAD_down, DPAD_left, DPAD_right,
                          L_stick_button, R_stick_button, Start, Select };

    float enableTime;

    private void Awake()
    {
        if (instance != null) Destroy(this);
        else instance = this;

        //Cursor.visible = false;

        prev_gamepad_count = Gamepad.all.Count;
    }

    int prev_gamepad_count;

    private void Update()
    {
        if (prev_gamepad_count > 0 && Gamepad.all.Count == 0)
        {
            print("No controller connected");
        }
    }

    public void Rumble(float lowFrequency, float highFrequency, float duration)
    {
        if (FailChecks()) return;

        Gamepad gamepad = Gamepad.all[0];
        gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
        StartCoroutine(StopRumbleAfterDelay(gamepad, duration));
    }

    private System.Collections.IEnumerator StopRumbleAfterDelay(Gamepad gamepad, float delay)
    {
        yield return new WaitForSeconds(delay);
        gamepad.SetMotorSpeeds(0, 0);
    }

    ButtonControl GetButton(Buttons button)
    {
        switch ((int)button)
        {
            case 0: return Gamepad.all[0].aButton;
            case 1: return Gamepad.all[0].bButton;
            case 2: return Gamepad.all[0].xButton;
            case 3: return Gamepad.all[0].yButton;
            case 4: return Gamepad.all[0].leftShoulder;
            case 5: return Gamepad.all[0].rightShoulder;
            case 6: return Gamepad.all[0].dpad.up;
            case 7: return Gamepad.all[0].dpad.down;
            case 8: return Gamepad.all[0].dpad.left;
            case 9: return Gamepad.all[0].dpad.right;
            case 10: return Gamepad.all[0].leftStickButton;
            case 11: return Gamepad.all[0].rightStickButton;
            case 12: return Gamepad.all[0].startButton;
            case 13: return Gamepad.all[0].selectButton;
        }

        return null;
    }

    public bool Pressed(Buttons button)
    {
        if (FailChecks()) return false;

        return GetButton(button).wasPressedThisFrame;
    }

    public bool Held(Buttons button)
    {
        if (FailChecks()) return false;

        return GetButton(button).isPressed;
    }

    public bool Released(Buttons button)
    {
        if (FailChecks()) return false;

        // prevent button releases on the first frame
        if (Time.time < 0.5f) return false;

        return GetButton(button).wasReleasedThisFrame;
    }





    bool FailChecks()
    {
        if (Gamepad.all.Count == 0) return true;
        if (Time.time < enableTime) return true;

        return false;
    }

    public Vector2 LeftStick()
    {
        if (FailChecks()) return Vector2.zero;

        Vector2 l_stick = Vector2.zero;

        if (keyboard)
        {
            if (Input.GetKey(KeyCode.W)) l_stick += new Vector2(0, 1);
            if (Input.GetKey(KeyCode.A)) l_stick += new Vector2(-1, 0);
            if (Input.GetKey(KeyCode.S)) l_stick += new Vector2(0, -1);
            if (Input.GetKey(KeyCode.D)) l_stick += new Vector2(1, 0);

            if (Input.GetKey(KeyCode.UpArrow)) l_stick += new Vector2(0, 1);
            if (Input.GetKey(KeyCode.LeftArrow)) l_stick += new Vector2(-1, 0);
            if (Input.GetKey(KeyCode.DownArrow)) l_stick += new Vector2(0, -1);
            if (Input.GetKey(KeyCode.RightArrow)) l_stick += new Vector2(1, 0);

            l_stick = l_stick.normalized;
        }
        else
        {
            l_stick = Gamepad.all[0].leftStick.ReadValue();
        }

        return l_stick;
    }

    public Vector3 LeftStickCamera()
    {
        Vector3 cam_right = Vector3.Cross(Vector3.up, Camera.main.transform.forward);
        Vector3 cam_forward = Vector3.Cross(cam_right, Vector3.up);

        return cam_right * LeftStick().x + cam_forward * LeftStick().y;
    }

    public Vector2 RightStick()
    {
        if (FailChecks()) return Vector2.zero;

        return Gamepad.all[0].rightStick.ReadValue();
    }

    public float LT()
    {
        if (FailChecks()) return 0;

        float lt;

        if (keyboard)
        {
            lt = Input.GetKey(KeyCode.S) ? 1 : 0;
        }
        else
        {
            lt = Gamepad.all[0].leftTrigger.ReadValue();
        }

        return lt;
    }

    public float RT()
    {
        if (FailChecks()) return 0;


        float rt;

        if (keyboard)
        {
            rt = Input.GetKey(KeyCode.W) ? 1 : 0;
        }
        else
        {
            rt = Gamepad.all[0].rightTrigger.ReadValue();
        }

        return rt;
    }

    public void DisabledUntil(float newEnableTime)
    {
        enableTime = newEnableTime;
    }
}
