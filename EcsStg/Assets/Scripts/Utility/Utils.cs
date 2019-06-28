using Unity.Mathematics;

namespace GAME
{
    public static class Utils
    {

        /// <summary>
        /// ベクトルの回転
        /// </summary>
        /// <param name="_index">Index</param>
        /// <param name="n_way">Nway</param>
        /// <param name="baseDir">基準ベクトル</param>
        /// <param name="angle">回転角</param>
        /// <returns></returns>
        public static float3 CalcDirection( int _index, int n_way, float3 baseDir, float angle )
        {
            // 起点まで回転
            var initVec = baseDir.Rotate_Y_Axis( -angle * 0.5f );
            return initVec.Rotate_Y_Axis(  angle * _index / n_way );
        }
    }
}
