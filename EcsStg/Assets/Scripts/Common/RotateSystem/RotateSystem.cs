using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

using GAME.DATA;

/// <summary>
/// オブジェクトの回転に特化したシステム
/// </summary>
public class RotateSystem : JobComponentSystem
{
    // Use the [BurstCompile] attribute to compile a job with Burst. You may see significant speed ups, so try it!
    [BurstCompile]
    struct ObjectRotateJob : IJobForEach<Rotation, ObjectRotateData>
    {
        public float DeltaTime;
        public void Execute(ref Rotation _rot, ref ObjectRotateData _rotData )
        {
            _rotData.CurrentLerpTime = math.clamp(_rotData.CurrentLerpTime + DeltaTime, 0f, _rotData.LerpTime );

            _rot.Value = math.slerp( _rot.Value, _rotData.Rot.Value, _rotData.CurrentLerpTime / _rotData.LerpTime );
        }
    }

    // OnUpdate runs on the main thread.
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        float dt = Time.deltaTime;
        var job = new ObjectRotateJob
        {
            DeltaTime = dt,
        };

        return job.Schedule(this, inputDependencies);
    }
}
