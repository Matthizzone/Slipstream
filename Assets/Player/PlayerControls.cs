using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public Quaternion rot;

    void Start()
    {
        foreach (Transform child in transform)
        {
            ConfigurableJoint joint = child.GetComponent<ConfigurableJoint>();

            if (joint == null) continue;

            joint.autoConfigureConnectedAnchor = false;
            joint.anchor = Vector3.zero;
            joint.connectedAnchor = transform.Find("Torso").InverseTransformPoint(transform.position);
            joint.rotationDriveMode = RotationDriveMode.Slerp;
            joint.slerpDrive = new JointDrive
            {
                positionSpring = 800,
                positionDamper = 80,
                maximumForce = Mathf.Infinity
            };

        }
    }

    void Update()
    {
        foreach (Transform child in transform)
        {
            ConfigurableJoint joint = child.GetComponent<ConfigurableJoint>();

            if (joint == null) continue;

            joint.targetRotation = rot;


        }
    }

    public static void SetJointTargetRotation(
        ConfigurableJoint joint, Quaternion targetWorldRotation, Quaternion jointWorldRotation)
    {
        Quaternion targetLocal = Quaternion.Inverse(jointWorldRotation) * targetWorldRotation;

        joint.targetRotation = Quaternion.Inverse(targetLocal);
    }

}
