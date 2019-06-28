using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

using GAME.DATA;

/// <summary>
/// オブジェクトの移動に特化したシステム
/// </summary>
public class ObjectMoveSystem : JobComponentSystem
{
    // Use the [BurstCompile] attribute to compile a job with Burst. You may see significant speed ups, so try it!
    [BurstCompile]
    struct ObjectMoveJob : IJobForEach<Translation, ObjectMoveData>
    {
        public void Execute(ref Translation _pos, ref ObjectMoveData _moveObject )
        {
            _pos.Value += _moveObject.Direction.Value * _moveObject.Speed;
        }
    }

    // OnUpdate runs on the main thread.
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new ObjectMoveJob
        {
        };

        return job.Schedule(this, inputDependencies);
    }
}
