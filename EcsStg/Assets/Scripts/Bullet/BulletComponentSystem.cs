using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

using GAME.Entity;

public class BulletComponentSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        // Entities.ForEach( this.UpdateTransform );
        Entities.ForEach( (ref BulletData _bullet, ref Translation _pos)=>
        {
            _pos.Value += _bullet.Direction.Value * _bullet.Speed;
        });
    }

    void UpdateTransform( ref BulletData _bullet, ref Translation _pos)
    {
        _pos.Value += _bullet.Direction.Value * _bullet.Speed;
    }
}
