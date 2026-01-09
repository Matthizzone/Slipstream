using UnityEngine;

public class PlayerDisplayBehavior : MonoBehaviour
{
    public Transform player;

    float[] fixedUpdateTimes;
    Vector3[] fixedUpdatePositions;
    Quaternion[] fixedUpdateRotations;

    private void Start()
    {
        InitializeVariables();
    }

    void InitializeVariables()
    {
        fixedUpdateTimes = new float[2];
        fixedUpdatePositions = new Vector3[2];
        fixedUpdateRotations = new Quaternion[2];

        fixedUpdatePositions[0] = Vector3.zero;
        fixedUpdatePositions[1] = Vector3.zero;

        fixedUpdateRotations[0] = Quaternion.identity;
        fixedUpdateRotations[1] = Quaternion.identity;
    }

    private void Update()
    {
        InterpolationPosAndRot();
    }

    void InterpolationPosAndRot()
    {
        Vector3 olderPos = fixedUpdatePositions[0];
        Vector3 newerPos = fixedUpdatePositions[1];

        Quaternion olderRot = fixedUpdateRotations[0];
        Quaternion newerRot = fixedUpdateRotations[1];

        Vector3 interpPos = Vector3.Lerp(olderPos, newerPos, GetInterpolationFactor());
        Quaternion interpRot = Quaternion.Lerp(olderRot, newerRot, GetInterpolationFactor());

        transform.position = interpPos;
        transform.rotation = interpRot;
    }

    private void FixedUpdate()
    {
        StampPosAndRot();
    }

    void StampPosAndRot()
    {
        fixedUpdateTimes[0] = fixedUpdateTimes[1];
        fixedUpdateTimes[1] = Time.fixedTime;

        fixedUpdatePositions[0] = fixedUpdatePositions[1];
        fixedUpdatePositions[1] = GetPosition();

        fixedUpdateRotations[0] = fixedUpdateRotations[1];
        fixedUpdateRotations[1] = GetRotation();
    }

    Vector3 GetPosition()
    {
        return player.position;
    }

    Quaternion GetRotation()
    {
        return player.rotation;
    }

    float GetInterpolationFactor()
    {
        float olderTime = fixedUpdateTimes[0];
        float newerTime = fixedUpdateTimes[1];

        if (newerTime - olderTime == 0) return 1;

        return (Time.time - newerTime) / (newerTime - olderTime);
    }
}
