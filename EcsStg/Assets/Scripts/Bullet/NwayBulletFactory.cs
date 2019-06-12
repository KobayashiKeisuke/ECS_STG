using System.Collections;
using System.Collections.Generic;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

using GAME.Entity;

namespace GAME
{
    /// <summary>
    /// N-way弾生成バレットファクトリークラス
    /// </summary>
    public class NwayBulletFactory : ISpawner
    {
        //------------------------------------------
        // 定数関連
        //------------------------------------------
        #region ===== CONSTS =====

        /// <summary> 初期化パラメータ </summary>
        public class InitParameter : BulletFactory.InitParameter
        {
            /// <summary> Nway数 </summary>
            public int NwayCount = 1;

            /// <summary> バレット発射角度 </summary>
            public float Angle = 180f;
        }
        #endregion //)===== CONSTS =====

        //------------------------------------------
        // メンバ変数
        //------------------------------------------
        #region ===== MEMBER_VARIABLES =====

        private int m_Nway=1;
        List<BulletFactory> m_factories;
        #endregion //) ===== MEMBER_VARIABLES =====

        public NwayBulletFactory( InitParameter _param )
        {
            m_factories = new List<BulletFactory>( _param.NwayCount );

            for (int i = 0; i < _param.NwayCount; i++)
            {
                var f = new BulletFactory( new BulletFactory.InitParameter(){
                    BulletPrefab = _param.BulletPrefab,
                    ParentObject    = _param.ParentObject,
                    PositionOffset  = _param.PositionOffset,
                    RotationOffset  = _param.RotationOffset,
                    ScreenSize      = _param.ScreenSize,
                    SpawnCycle      = _param.SpawnCycle,

                    Speed           = _param.Speed,
                    MoveDirection   = CalcDirection( i, _param.NwayCount-1, _param.MoveDirection, _param.Angle ),// N-1 にしないと水平に出ない
                    Damage          = _param.Damage,
                });

                m_factories.Add( f );
            }
        }
        private Vector3 Rotate_X_Axis( Vector3 _baseVec, float _rotateAngle )
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

        private Vector3 Rotate_Y_Axis( Vector3 _baseVec, float _rotateAngle )
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
        private Vector3 Rotate_Z_Axis( Vector3 _baseVec, float _rotateAngle )
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

        private Vector3 CalcRotateVec( float3x3 R, Vector3 v )
        {
            return new Vector3(
                R.c0.x * v.x + R.c0.y * v.y + R.c0.z * v.z,
                R.c1.x * v.x + R.c1.y * v.y + R.c1.z * v.z,
                R.c2.x * v.x + R.c2.y * v.y + R.c2.z * v.z);
        }


        private Vector3 CalcDirection( int _index, int n_way, Vector3 baseDir, float angle )
        {
            Vector3 initVec = Rotate_Y_Axis( baseDir, -angle * 0.5f );
            return Rotate_Y_Axis( initVec, angle * _index / n_way );
        }

        public void Spawn( )
        {
            foreach (var item in m_factories)
            {
                item.Spawn();
            }
        }
    }
}

