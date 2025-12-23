using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CamShake : MonoBehaviour
{
    float shake_amt = 0;
    float dissipation = 0;

    public void BeginDissipateShake(float new_shake_amt, float new_dissipation)
    {
        shake_amt = new_shake_amt;
        dissipation = new_dissipation;
    }

    void LateUpdate()
    {
        ApplyShake(shake_amt);
        shake_amt = MattMath.FRIndepLerp(shake_amt, 0, dissipation);
    }

    void ApplyShake(float amt)
    {
        transform.Rotate(new Vector3(
            Random.Range(-amt, amt),
            Random.Range(-amt, amt),
            Random.Range(-amt, amt)
            ));
    }
}
