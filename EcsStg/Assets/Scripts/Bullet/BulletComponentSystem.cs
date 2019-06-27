using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

using UnityEngine;

using GAME.DATA;

public class BulletComponentSystem : JobComponentSystem
{
    const float DEFAULT_SIZE = 20.0f;
    private float maxSize = DEFAULT_SIZE;
        //------------------------------------------
        // 初期化
        //------------------------------------------
        #region ===== INITIALIZE =====

        public void Initialize( Camera _cam )
        {
            float frustumHeight = _cam.transform.position.y * Mathf.Tan(_cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float frustumWidth = frustumHeight / Screen.height * Screen.width;

            maxSize = math.max(frustumHeight*2.0f, frustumWidth*2.0f);
        }

        #endregion //) ===== INITIALIZE =====

    // Use the [BurstCompile] attribute to compile a job with Burst. You may see significant speed ups, so try it!
    [BurstCompile]
    struct BulletMoveJob : IJobForEach<Translation, BulletData, ObjectMoveData>
    {
        public float X_RANGE;
        public float Y_RANGE;
        public float Z_RANGE;

        public void Execute(ref Translation _pos, ref BulletData _bullet, ref ObjectMoveData _moveData )
        {
            // 未初期化は処理対象外
            if( !_bullet.IsInitialized )
            {
                return;
            }
            // 範囲内なら特になにもしない
            if( -X_RANGE < _pos.Value.x && _pos.Value.x < X_RANGE
            && -Y_RANGE < _pos.Value.y && _pos.Value.y < Y_RANGE
            && -Z_RANGE < _pos.Value.z && _pos.Value.z < Z_RANGE
            )
            {
                return;
            }
            // 移動停止
            _moveData.Direction.Value = float3.zero;
            _bullet.IsInitialized = false;
        }
    }
    // OnUpdate runs on the main thread.
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var manager = World.Active.EntityManager;

        var job = new BulletMoveJob
        {
            X_RANGE     = maxSize,
            Y_RANGE     = maxSize,
            Z_RANGE     = maxSize,
        };
        return job.Schedule( this, inputDependencies);
    }
}
