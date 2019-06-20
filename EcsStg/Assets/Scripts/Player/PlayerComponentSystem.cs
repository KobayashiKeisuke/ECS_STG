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
    public class PlayerComponentSystem : JobComponentSystem
    {
        private const float MAX_MOVE_SPEED = 0.05f;
        private const float RANGE_LIMIT = 0.05f;// 上下左右N%を移動範囲制限


        Camera MainCam;
        private EcsUISystem m_uiSystem;
        
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
        }
        // Use the [BurstCompile] attribute to compile a job with Burst. You may see significant speed ups, so try it!
        [BurstCompile]
        struct PlayerMoveJob : IJobForEachWithEntity<Translation, PlayerData, ObjectMoveData >
        {
            /// <summary> World 座標のX軸移動範囲</summary>
            public float2 X_Range;
            /// <summary> World 座標のY軸移動範囲</summary>
            public float2 Z_Range;
            /// <summary> 移動速度</summary>
            public float MoveSpeed;
            /// <summary> 移動速度</summary>
            public float MoveRange;
            /// <summary> 移動方向[rad]</summary>
            public float MoveAngle;

            public void Execute(Entity entity, int index, [ReadOnly] ref Translation pos, ref PlayerData playerData, ref ObjectMoveData moveData )
            {
                // パラメータ更新 設定
                moveData.Speed = MoveRange * MoveSpeed;
                moveData.Direction.Value.x = math.cos( MoveAngle );
                moveData.Direction.Value.z = math.sin( MoveAngle );
                // 移動範囲チェック
                float nextXPos = pos.Value.x + moveData.Direction.Value.x * moveData.Speed;
                float nextZPos = pos.Value.z + moveData.Direction.Value.z * moveData.Speed;
                if( nextXPos < X_Range.x || X_Range.y <= nextXPos )
                {
                    moveData.Speed = 0f;
                }
                if( nextZPos < Z_Range.x || Z_Range.y <= nextZPos )
                {
                    moveData.Speed = 0f;
                }
            }
        }

        // OnUpdate runs on the main thread.
        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            // Base の可動域の設定
            float frustumHeight = MainCam.transform.position.y * Mathf.Tan(MainCam.fieldOfView * 0.5f * Mathf.Deg2Rad) * ( 1.0f - RANGE_LIMIT);
            float frustumWidth = frustumHeight / Screen.height * Screen.width * ( 1.0f - RANGE_LIMIT);
            Vector3 frustumLeftBot = new Vector3( -frustumWidth, 0f, -frustumHeight);
            Vector3 frustumRightTop = new Vector3( frustumWidth, 0f, frustumHeight);

            var job = new PlayerMoveJob
            {
                X_Range     = new float2( frustumLeftBot.x, frustumRightTop.x),
                Z_Range     = new float2( frustumLeftBot.z, frustumRightTop.z),
                MoveSpeed   = MAX_MOVE_SPEED,
                MoveRange   = m_uiSystem.UiData.Range,
                MoveAngle   = m_uiSystem.UiData.Angle,
            };

            return job.Schedule(this, inputDependencies);
        }
    }

}
