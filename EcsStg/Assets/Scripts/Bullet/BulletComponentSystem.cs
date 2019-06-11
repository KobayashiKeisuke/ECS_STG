using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

using GAME.Entity;

// public class BulletComponentSystem : ComponentSystem
// {
//     protected override void OnUpdate()
//     {
//         // Entities.ForEach( this.UpdateTransform );
//         Entities.ForEach( (ref BulletData _bullet, ref Translation _pos)=>
//         {
//             _pos.Value += _bullet.Direction.Value * _bullet.Speed;
//         });
//     }

//     void UpdateTransform( ref BulletData _bullet, ref Translation _pos)
//     {
//         _pos.Value += _bullet.Direction.Value * _bullet.Speed;
//     }
// }

public class BulletMoveSystem : JobComponentSystem
{
    // Use the [BurstCompile] attribute to compile a job with Burst. You may see significant speed ups, so try it!
    [BurstCompile]
    struct BulletMoveJob : IJobForEach<Translation, BulletData>
    {
        // The [ReadOnly] attribute tells the job scheduler that this job will not write to rotSpeedSpawnAndRemove
        public void Execute(ref Translation _pos, [ReadOnly] ref BulletData _bullet)
        {
             _pos.Value += _bullet.Direction.Value * _bullet.Speed;
        }
    }

    // OnUpdate runs on the main thread.
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new BulletMoveJob
        {
        };

        return job.Schedule(this, inputDependencies);
    }
}

public class ObjectDestroySystem : JobComponentSystem
{
    const float X_RANGE = 20.0f;
    const float Y_RANGE = 20.0f;
    const float Z_RANGE = 20.0f;

    EntityCommandBufferSystem m_Barrier;

    protected override void OnCreate()
    {
        m_Barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    // Use the [BurstCompile] attribute to compile a job with Burst.
    // You may see significant speed ups, so try it!
    [BurstCompile]
    struct BulletDestroyJob : IJobForEachWithEntity< Translation>
    {
        [WriteOnly]
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public void Execute(Entity entity, int jobIndex, ref Translation _currentPosition)
        {
            // 範囲内なら特になにもしない
            if( -X_RANGE < _currentPosition.Value.x && _currentPosition.Value.x < X_RANGE
            && -Y_RANGE < _currentPosition.Value.y && _currentPosition.Value.y < Y_RANGE
            && -Z_RANGE < _currentPosition.Value.z && _currentPosition.Value.z < Z_RANGE
            )
            {
                return;
            }
            CommandBuffer.DestroyEntity(jobIndex, entity);
        }
    }

    // OnUpdate runs on the main thread.
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var commandBuffer = m_Barrier.CreateCommandBuffer().ToConcurrent();

        var job = new BulletDestroyJob
        {
            CommandBuffer = commandBuffer,

        }.Schedule(this, inputDependencies);

        m_Barrier.AddJobHandleForProducer(job);

        return job;
    }
}