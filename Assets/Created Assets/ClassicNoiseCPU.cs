using UnityEngine;

public class ClassicNoiseCPU : MonoBehaviour
{ 
    public float perlinScale = 0.5f;
    public float waveHeight = 2.5f;
    public float waveSpeed = 0.25f;
    public float theScriptTime;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //theScriptTime = Time.time;
        //GetComponent<Renderer>().material.SetFloat("_scriptTime", theScriptTime);

        //Get copy of mesh filter
        MeshFilter meshFil = GetComponent<MeshFilter>();


        //Get a copy of the vertex list
        Vector3[] vertList = meshFil.mesh.vertices;

        //Loop through and calculate perlin values
        for (int i = 0; i < vertList.Length; i++)
        {
            vertList[i].y = 0.0f;

            Vector2 uv = new Vector2(vertList[i].x,vertList[i].z) * 4.0f + new Vector2(0.2f, 1.0f) * theScriptTime;

            float y_val = 0.5f;

            Vector3 coord = new Vector3(uv.x * perlinScale, uv.y * perlinScale, theScriptTime * waveSpeed);
            y_val += cnoise(coord) * waveHeight;

            vertList[i].y = y_val;

            //Calculate a Perlin reference value from it's position and the scale of perlin noise
            //Then add an offset for 
            //float pX = (vertList[i].x * perlinScale) + (waveSpeed * theScriptTime);
            // pZ = (vertList[i].z * perlinScale) + (waveSpeed * theScriptTime);
            //vertList[i].y = Mathf.PerlinNoise(pX, pZ) * waveHeight;
        }

        meshFil.mesh.vertices = vertList;
        meshFil.mesh.RecalculateNormals();
        meshFil.mesh.RecalculateBounds();
    }

    public static Vector3 mod(Vector3 x, Vector3 y)
    {
        //return x minus (y times (floor of (x divided by y))) It's literally x mod y
        //return x - y * new Vector3(Mathf.Floor(x.x/y.x), Mathf.Floor(x.y/y.y), Mathf.Floor(x.z/y.z));
        return new Vector3(x.x % y.x, x.y % y.y, x.z % y.z);
    }

    public static Vector3 mod289(Vector3 x)
    {
        //return x - Mathf.Floor(x / 289.0) * 289.0;
        return new Vector3(x.x % 289, x.y % 289, x.z % 289);
    }

    public static Vector4 mod289(Vector4 x)
    {
        //return x - Mathf.Floor(x / 289.0) * 289.0;
        return new Vector4(x.x % 289, x.y % 289, x.z % 289, x.w % 289);
    }

    public static Vector4 permute(Vector4 x)
    {
        Vector4 temp = ((x * 34.0f) + new Vector4(1, 1, 1, 1));
        return mod289(Vec4Mul(temp, x));
    }

    public static Vector4 taylorInvSqrt(Vector4 r)
    {
        //return new Vector4(1.79284291400159f - r * 0.85373472095314f;
        Vector4 s = new Vector4(1.79284291400159f, 1.79284291400159f, 1.79284291400159f, 1.79284291400159f);
        Vector4 t = r * 0.85373472095314f;
        return s - t;
    }

    public static Vector3 fade(Vector3 t)
    {
        ////return t * t * t * (t * (t * 6.0 - 15.0) + 10.0);
        //Vector3 t1 = (t * 6.0f) - new Vector3(15, 15, 15);
        //Vector3 t2 = new Vector3(t.x * t1.x, t.y * t1.y, t.z * t1.z) + new Vector3(10, 10, 10);
        //t2.x *= t.x * t.x * t.x;
        //t2.y *= t.y * t.y * t.y;
        //t2.z *= t.z * t.z * t.z;
        //return t2;

        return Vec3Mul(t, Vec3Mul(t, Vec3Mul(t, Vec3Mul(t, ((t * 6.0f) - new Vector3(15, 15, 15))) + new Vector3(10, 10, 10))));
    }

    public static Vector3 frac(Vector3 x)
    {
        return x - floor(x);
    }

    public static Vector4 frac(Vector4 x)
    {
        return x - floor(x);
    }

    public static Vector4 abs(Vector4 a)
    {
        Vector4 b;
        b.x = Mathf.Abs(a.x);
        b.y = Mathf.Abs(a.y);
        b.z = Mathf.Abs(a.z);
        b.w = Mathf.Abs(a.w);
        return b;
    }

    public static Vector4 step(Vector4 s, Vector4 t)
    {
        Vector4 u;
        bool my_bool = (t.x >= s.x);
        u.x = my_bool ? 1 : 0;
        my_bool = (t.y >= s.y);
        u.y = my_bool ? 1 : 0;
        my_bool = (t.z >= s.z);
        u.z = my_bool ? 1 : 0;
        my_bool = (t.w >= s.w);
        u.w = my_bool ? 1 : 0;
        return u;
    }

    public static Vector3 Vec3Mul(Vector3 lhs, Vector3 rhs)
    {
        return new Vector3(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z);
    }

    public static Vector4 Vec4Mul(Vector4 lhs, Vector4 rhs)
    {
        return new Vector4(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z, lhs.w * rhs.w);
    }

    public static Vector3 floor(Vector3 f)
    {
        f.x = Mathf.Floor(f.x);
        f.y = Mathf.Floor(f.y);
        f.z = Mathf.Floor(f.z);
        return f;
    }

    public static Vector4 floor(Vector4 f)
    {
        f.x = Mathf.Floor(f.x);
        f.y = Mathf.Floor(f.y);
        f.z = Mathf.Floor(f.z);
        f.w = Mathf.Floor(f.w);
        return f;
    }

    // Classic Perlin noise
    public static float cnoise(Vector3 P)
    {
        Vector3 Pi0 = floor(P); // Integer part for indexing
        Vector3 Pi1 = Pi0 + new Vector3(1,1,1); // Integer part + 1
        Pi0 = mod289(Pi0);
        Pi1 = mod289(Pi1);
        Vector3 Pf0 = frac(P); // Fractional part for interpolation
        Vector3 Pf1 = Pf0 - new Vector3(1, 1, 1); // Fractional part - 1.0
        Vector4 ix = new Vector4(Pi0.x, Pi1.x, Pi0.x, Pi1.x);
        Vector4 iy = new Vector4(Pi0.y, Pi0.y, Pi1.y, Pi1.y);
        Vector4 iz0 = new Vector4(Pi0.z, Pi0.z, Pi0.z, Pi0.z);
        Vector4 iz1 = new Vector4(Pi1.z, Pi1.z, Pi1.z, Pi1.z);

        Vector4 ixy = permute(permute(ix) + iy);
        Vector4 ixy0 = permute(ixy + iz0);
        Vector4 ixy1 = permute(ixy + iz1);

        Vector4 gx0 = ixy0 / 7.0f;
        Vector4 gy0 = frac(floor(gx0) / 7.0f) - new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
        gx0 = frac(gx0);
        Vector4 gz0 = new Vector4(0.5f, 0.5f, 0.5f, 0.5f) - abs(gx0) - abs(gy0);
        Vector4 sz0 = step(gz0, new Vector4(0,0,0,0));
        gx0 -= Vec4Mul(sz0, (step(new Vector4(0,0,0,0), gx0) - new Vector4(0.5f, 0.5f, 0.5f, 0.5f)));
        gy0 -= Vec4Mul(sz0, (step(new Vector4(0,0,0,0), gy0) - new Vector4(0.5f, 0.5f, 0.5f, 0.5f)));

        Vector4 gx1 = ixy1 / 7.0f;
        Vector4 gy1 = frac(floor(gx1) / 7.0f) - new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
        gx1 = frac(gx1);
        Vector4 gz1 = new Vector4(0.5f, 0.5f, 0.5f, 0.5f) - abs(gx1) - abs(gy1);
        Vector4 sz1 = step(gz1, new Vector4(0, 0, 0, 0));
        gx1 -= Vec4Mul(sz1, (step(new Vector4(0, 0, 0, 0), gx1) - new Vector4(0.5f, 0.5f, 0.5f, 0.5f)));
        gy1 -= Vec4Mul(sz1, (step(new Vector4(0, 0, 0, 0), gy1) - new Vector4(0.5f, 0.5f, 0.5f, 0.5f)));

        Vector3 g000 = new Vector3(gx0.x, gy0.x, gz0.x);
        Vector3 g100 = new Vector3(gx0.y, gy0.y, gz0.y);
        Vector3 g010 = new Vector3(gx0.z, gy0.z, gz0.z);
        Vector3 g110 = new Vector3(gx0.w, gy0.w, gz0.w);
        Vector3 g001 = new Vector3(gx1.x, gy1.x, gz1.x);
        Vector3 g101 = new Vector3(gx1.y, gy1.y, gz1.y);
        Vector3 g011 = new Vector3(gx1.z, gy1.z, gz1.z);
        Vector3 g111 = new Vector3(gx1.w, gy1.w, gz1.w);

        Vector4 norm0 = taylorInvSqrt(new Vector4(Vector3.Dot(g000, g000), Vector3.Dot(g010, g010), Vector3.Dot(g100, g100), Vector3.Dot(g110, g110)));
        g000 *= norm0.x;
        g010 *= norm0.y;
        g100 *= norm0.z;
        g110 *= norm0.w;

        Vector4 norm1 = taylorInvSqrt(new Vector4(Vector3.Dot(g001, g001), Vector3.Dot(g011, g011), Vector3.Dot(g101, g101), Vector3.Dot(g111, g111)));
        g001 *= norm1.x;
        g011 *= norm1.y;
        g101 *= norm1.z;
        g111 *= norm1.w;

        float n000 = Vector3.Dot(g000, Pf0);
        float n100 = Vector3.Dot(g100, new Vector3(Pf1.x, Pf0.y, Pf0.z));
        float n010 = Vector3.Dot(g010, new Vector3(Pf0.x, Pf1.y, Pf0.z));
        float n110 = Vector3.Dot(g110, new Vector3(Pf1.x, Pf1.y, Pf0.z));
        float n001 = Vector3.Dot(g001, new Vector3(Pf0.x, Pf0.y, Pf1.z));
        float n101 = Vector3.Dot(g101, new Vector3(Pf1.x, Pf0.y, Pf1.z));
        float n011 = Vector3.Dot(g011, new Vector3(Pf0.x, Pf1.y, Pf1.z));
        float n111 = Vector3.Dot(g111, Pf1);

        Vector3 fade_xyz = fade(Pf0);
        Vector4 n_z = Vector4.Lerp(new Vector4(n000, n100, n010, n110), new Vector4(n001, n101, n011, n111), fade_xyz.z);
        Vector2 n_yz = Vector2.Lerp(new Vector2(n_z.x, n_z.y), new Vector2(n_z.z, n_z.w), fade_xyz.y);
        float n_xyz = Mathf.Lerp(n_yz.x, n_yz.y, fade_xyz.x);
        return 2.2f * n_xyz;
    }
}