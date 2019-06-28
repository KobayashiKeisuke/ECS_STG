using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Assertions;
using static Unity.Physics.Math;

namespace GAME
{
    using GAME.DATA;
    using GAME.UI;

    /// <summary>
    /// まっさきに入力を受け付けてから物理世界を更新かけるため更新順を定義
    /// </summary>
    [UpdateAfter(typeof(EcsUISystem))]
    public class PlayerComponentSystem : ComponentSystem
    {
        private const float MAX_MOVE_SPEED = 5.0f;
        private const float RANGE_LIMIT = 0.05f;// 上下左右N%を移動範囲制限
        private const float TARGET_DISTANCE = 40.0f;

        Camera MainCam;
        private EcsUISystem m_uiSystem;

        private bool m_isInitialized = false;
        public void ResetSystem()
        {
            m_isInitialized = false;
        }

        protected override void OnCreate()
        {
            World currentWorld = World.Active;
            m_uiSystem = currentWorld.GetOrCreateSystem<EcsUISystem>();
            Debug.Assert( m_uiSystem != null );
        }

        public void Initialize( Camera _cam )
        {
            MainCam = _cam;
            Debug.Assert( MainCam != null );
            m_isInitialized = true;
        }
        // OnUpdate runs on the main thread.
        protected override void OnUpdate()
        {
            if( !m_isInitialized )
            {
                return;
            }
            
            float3 normalizedForwardVec    = MainCam.transform.forward.normalized;

            float angle = m_uiSystem.UiData.Angle;
            float range = m_uiSystem.UiData.Range;
            Entities.ForEach( (ref Translation pos, ref PlayerData playerData, ref ObjectMoveData moveData )=>
            {
                // パラメータ更新 設定
                float speed = range * MAX_MOVE_SPEED;
                float2 diffScreenPos;
                diffScreenPos.x = math.cos( angle ) * speed;
                diffScreenPos.y = math.sin( angle ) * speed;

                float2 nextScreenPos = playerData.PrevScreenPos + diffScreenPos;
                nextScreenPos.x = math.clamp( nextScreenPos.x, Screen.width * RANGE_LIMIT, Screen.width * ( 1.0f - RANGE_LIMIT) );
                nextScreenPos.y = math.clamp( nextScreenPos.y, Screen.height * RANGE_LIMIT, Screen.height * ( 1.0f - RANGE_LIMIT) );

                float3 nextWorldPos = MainCam.ScreenToWorldPoint( new Vector3( nextScreenPos.x, nextScreenPos.y, TARGET_DISTANCE));

                moveData.Direction.Value = nextWorldPos - pos.Value;
                moveData.Speed = 1.0f;

                playerData.PrevScreenPos = nextScreenPos;
            });

        }
        /// <summary>
        /// カメラのローカルZ軸方向(法線ベクトル:n) にカメラからd 離れた点のカメラのローカルX-Y平面へ
        /// 点P を正射影した位置を計算する
        /// </summary>
        /// <param name="p">点P</param>
        /// <param name="n">カメラのローカルZ軸ベクトル</param>
        /// <param name="d">カメラのローカルZ軸の位置</param>
        /// <returns></returns>
        public static float3 CalcProjectionPoint( float3 p, float3 n, float d)
        {
            float coeff = d - math.dot( p, n) / math.dot( n, n);

            return new float3( coeff*n.x + p.x, coeff*n.y + p.y, coeff*n.z + p.z );
        }
    }

}
