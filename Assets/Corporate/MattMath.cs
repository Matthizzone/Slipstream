using UnityEngine;

public static class MattMath
{
    public static float FRIndepLerp(float a, float b, float lambda)
    {
        return Mathf.Lerp(a, b, 1 - Mathf.Exp(-lambda * Time.deltaTime));
    }

    public static Vector2 FRIndepLerp(Vector2 a, Vector2 b, float lambda)
    {
        return Vector2.Lerp(a, b, 1 - Mathf.Exp(-lambda * Time.deltaTime));
    }

    public static Vector3 FRIndepLerp(Vector3 a, Vector3 b, float lambda)
    {
        return Vector3.Lerp(a, b, 1 - Mathf.Exp(-lambda * Time.deltaTime));
    }

    public static Quaternion FRIndepLerp(Quaternion a, Quaternion b, float lambda)
    {
        return Quaternion.Lerp(a, b, 1 - Mathf.Exp(-lambda * Time.deltaTime));
    }

    public static float Erf(float x)
    {
        // Thanks John D. Cook, PhD!

        // constants
        float a1 = 0.254829592f;
        float a2 = -0.284496736f;
        float a3 = 1.421413741f;
        float a4 = -1.453152027f;
        float a5 = 1.061405429f;
        float p = 0.3275911f;

        // Save the sign of x
        int sign = 1;
        if (x < 0)
            sign = -1;
        x = Mathf.Abs(x);

        // A&S formula 7.1.26
        float t = 1 / (1 + p * x);
        float y = 1 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Mathf.Exp(-x * x);

        return sign * y;
    }

    public static float TransErf(float x)
    {
        // The transgender version of Erf. JK. Its for transsss~itions!

        x = Mathf.Clamp01(x);
        return (Erf(4 * x - 2) * 1.0047f + 1) / 2;
    }

    public static float SymmTransErf(float x)
    {
        // The symmetric version of the function above (kinda bell curve shaped)

        x = Mathf.Clamp01(x);
        return x < 0.5 ? TransErf(2 * x) : TransErf(-2 * x + 2);
    }

    public static int GetDigit(float a, int which)
    {
        // which = 10^(-which)
        // 543210.[-1][-2] etc.
        return ((int)(a / Mathf.Pow(10, which)) % 10);
    }

    public static Quaternion QuaternionSmoothDamp(Quaternion rot, Quaternion target, ref Quaternion velocity, float time)
    {
        if (Time.deltaTime < Mathf.Epsilon) return rot;
        // account for double-cover
        var Dot = Quaternion.Dot(rot, target);
        var Multi = Dot > 0f ? 1f : -1f;
        target.x *= Multi;
        target.y *= Multi;
        target.z *= Multi;
        target.w *= Multi;
        // smooth damp (nlerp approx)
        var Result = new Vector4(
            Mathf.SmoothDamp(rot.x, target.x, ref velocity.x, time),
            Mathf.SmoothDamp(rot.y, target.y, ref velocity.y, time),
            Mathf.SmoothDamp(rot.z, target.z, ref velocity.z, time),
            Mathf.SmoothDamp(rot.w, target.w, ref velocity.w, time)
        ).normalized;

        // ensure deriv is tangent
        var derivError = Vector4.Project(new Vector4(velocity.x, velocity.y, velocity.z, velocity.w), Result);
        velocity.x -= derivError.x;
        velocity.y -= derivError.y;
        velocity.z -= derivError.z;
        velocity.w -= derivError.w;

        return new Quaternion(Result.x, Result.y, Result.z, Result.w);
    }

    // takes the array and creates a new random order
    public static void FisherYatesShuffle(int[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            // Pick a random index
            int j = Random.Range(0, i + 1);

            // Swap array[i] with array[j]
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

    // Generates a random number from a normal distribution with mean μ and standard deviation σ.
    // This method is called "Box-Muller transform"
    public static float RandomFromNormal(float mean, float stdDev)
    {
        // Generate two uniform random numbers between 0 and 1
        float u1 = Random.value; // Random.value is between 0.0 and 1.0
        float u2 = Random.value;

        // Apply the Box-Muller transform
        float z = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Cos(2.0f * Mathf.PI * u2);

        // Scale and shift the result to match the specified mean and standard deviation
        return z * stdDev + mean;
    }

    public static float TriangleWave(float period, float t)
    {
        // traingle wave starting at 1 (like cosine)

        float pos = Mathf.Repeat(t, period) / period;

        if (pos < 0.5f)
        {
            return Mathf.Lerp(-1, 1, pos * 2f);
        }
        else
        {
            return Mathf.Lerp(1, -1, (pos - 0.5f) * 2f);
        }
    }

    public static string GetAlphabetLetter(int index)
    {
        if (index < 0 || index > 25)
        {
            return "_";
        }

        char letter = (char)('A' + index);
        return letter.ToString();
    }

    public static Vector3 RandomVector3()
    {
        // returns a vector in a random direction and has random length

        return new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f));
    }
}
