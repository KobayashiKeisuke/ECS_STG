using Unity.Mathematics;

public static class MathematicsExtention
{
    public static float3 Rotate_X_Axis(this float3 _baseVec, float _rotateAngle )
    {
        float theta = _rotateAngle * math.PI / 180.0f;
        float sinT = math.sin( theta );
        float cosT = math.cos( theta );

        //      [ 1 0   0    ]
        // Rx = [ 0 cos -sin ]
        //      [ 0 sin cos  ]
        float3x3 R = new float3x3(
            1f, 0f, 0f,
            0f, cosT, -sinT,
            0f, sinT, cosT );
        return CalcRotateVec( R, _baseVec);
    }

    public static float3 Rotate_Y_Axis(this float3 _baseVec, float _rotateAngle )
    {
        float theta = _rotateAngle * math.PI / 180.0f;
        float sinT = math.sin( theta );
        float cosT = math.cos( theta );
        //      [ cos  0 sin ]
        // Ry = [ 0    1 0   ]
        //      [ -sin 0 cos ]
        float3x3 R = new float3x3(
            cosT, 0f, sinT,
            0f, 1f, 0f,
            -sinT, 0f, cosT );
        return CalcRotateVec( R, _baseVec );
    }
    public static float3 Rotate_Z_Axis(this float3 _baseVec, float _rotateAngle )
    {
        float theta = _rotateAngle * math.PI / 180.0f;
        float sinT = math.sin( theta );
        float cosT = math.cos( theta );
        //      [ cos -sin 0 ]
        // Rz = [ sin cos  0 ]
        //      [ 0   0    1 ]
        float3x3 R = new float3x3(
            cosT,  sinT, 0f,
            -sinT, cosT, 0f,
            -sinT, 0f, cosT );
        return CalcRotateVec( R, _baseVec);
    }

    public static float3 CalcRotateVec( float3x3 R, float3 v )
    {
        return new float3(
            R.c0.x * v.x + R.c0.y * v.y + R.c0.z * v.z,
            R.c1.x * v.x + R.c1.y * v.y + R.c1.z * v.z,
            R.c2.x * v.x + R.c2.y * v.y + R.c2.z * v.z);
    }
}
