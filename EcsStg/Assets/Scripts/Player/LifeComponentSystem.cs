using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace GAME
{
    using GAME.DATA;

    
    [UpdateAfter(typeof(BulletCollisionSystem))]
    public class LifeComponentSystem : JobComponentSystem
    {

        // Use the [BurstCompile] attribute to compile a job with Burst. You may see significant speed ups, so try it!
        [BurstCompile]
        struct LifeJob : IJobForEach<PlayerData>
        {
            public void Execute( ref PlayerData _data )
            {
                _data.IsGameOver = _data.Life < 0;
            }
        }

        // OnUpdate runs on the main thread.
        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var job = new LifeJob();
            return job.Schedule(this,inputDependencies );
        }
    }
}

