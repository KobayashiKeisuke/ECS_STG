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

namespace GAME.INPUT
{
    /// <summary>
    /// まっさきに入力を受け付けてから物理世界を更新かけるため更新順を定義
    /// </summary>
    [UpdateBefore(typeof(BuildPhysicsWorld))]
    public class InputControlSystem : JobComponentSystem
    {
        protected override void OnCreate()
        {
        }
        // Use the [BurstCompile] attribute to compile a job with Burst. You may see significant speed ups, so try it!
        [BurstCompile]
        struct InputJob : IJobForEachWithEntity<InputData>
        {
            /// <summary> Screen座標</summary>
            public float3 Position;
            /// <summary> タッチ状態</summary>
            public TOUCH_STATE State;
            /// <summary> 現在時刻</summary>
            public float CurrentTime;
            /// <summary> 差分時間</summary>
            public float DeltaTime;

            public void Execute(Entity entity, int index, ref InputData _data)
            {
                if( State != TOUCH_STATE.NONE )
                {
                    _data.ScreenPosition  = Position.xy;
                    _data.CurrentTime     = CurrentTime;
                    _data.DeltaTime       = DeltaTime;
                    _data.IsPressed       = State != TOUCH_STATE.ENDED;
                }
            }
        }

        // OnUpdate runs on the main thread.
        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            TOUCH_STATE state = InputUtil.GetTouchState();
            float3 position = InputUtil.GetPosition();
            Debug.Log($"State:{state}, Pos({position.x},{position.y}");
            var job = new InputJob
            {
                Position = position,
                State = state,
                CurrentTime = Time.realtimeSinceStartup,
                DeltaTime = Time.deltaTime,
            };

            return job.Schedule(this, inputDependencies);
        }
    }

    /// <summary>
    /// 入力処理に関するユーティリティクラス
    /// </summary>
    public static class InputUtil
    {
        public static TOUCH_STATE GetTouchState()
        {
            #if UNITY_EDITOR
            if( Input.GetMouseButtonDown(0)) { return TOUCH_STATE.BEGIN; }
            if( Input.GetMouseButton(0)) { return TOUCH_STATE.MOVED; }
            if( Input.GetMouseButtonUp(0)) { return TOUCH_STATE.ENDED; }
            #else
            if( Input.touchCount > 0 )
            {
                return (TOUCH_STATE)((int)Input.GetTouch(0).phase);
            }
            #endif

            return TOUCH_STATE.NONE;
        }
        public static float3 GetPosition()
        {
            #if UNITY_EDITOR
            Vector3 position = Input.mousePosition;
            return new float3( position.x, position.y, position.z );
            #else
            if( Input.touchCount > 0 )
            {
                Touch t = Input.GetTouch(0);
                return new float3( t.position.x, t.position.y, t.position.z );
            }
            return float3.zero;
            #endif
        }
    }
}
