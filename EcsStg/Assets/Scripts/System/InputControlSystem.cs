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
        InputData m_currentInputData;
        public InputData GetCurrentInputData() =>m_currentInputData;

        protected override void OnCreate()
        {
            m_currentInputData = new InputData();
        }
        // Use the [BurstCompile] attribute to compile a job with Burst. You may see significant speed ups, so try it!
        [BurstCompile]
        struct InputJob : IJobForEachWithEntity<InputData>
        {
            /// <summary> 最新データ</summary>
            public InputData Data;

            public void Execute(Entity entity, int index, ref InputData _data)
            {
                _data.State             = Data.State;
                _data.ScreenPosition    = Data.ScreenPosition;
                _data.CurrentTime       = Data.CurrentTime;
                _data.DeltaTime         = Data.DeltaTime;
                _data.DiffPosition      = Data.DiffPosition;
                _data.Angle             = Data.Angle;
            }
        }

        // OnUpdate runs on the main thread.
        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            float3 position = InputUtil.GetPosition();

            m_currentInputData.State            = InputUtil.GetTouchState();
            m_currentInputData.CurrentTime      = Time.realtimeSinceStartup;
            m_currentInputData.DeltaTime        = Time.deltaTime;
            m_currentInputData.DiffPosition     = position.xy - m_currentInputData.ScreenPosition;
            m_currentInputData.Angle            = math.atan2(  m_currentInputData.DiffPosition.y,  m_currentInputData.DiffPosition.x);
            m_currentInputData.ScreenPosition   = position.xy;

            var job = new InputJob
            {
                Data = GetCurrentInputData(),
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
                return new float3( t.position.x, t.position.y, 0f);
            }
            return float3.zero;
            #endif
        }
    }
}
