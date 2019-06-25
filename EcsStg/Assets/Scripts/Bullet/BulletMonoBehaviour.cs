using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

using GAME.DATA;


[RequiresEntityConversion]
public class BulletMonoBehaviour : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert( Entity _entity, EntityManager dstManager, GameObjectConversionSystem _conversionSystem )
    {
        Translation t = new Translation();
        t.Value = new float3(0f, 0f, 0f);

        var bulletData = new BulletData()
        {
            Damage = 1,
        };
        var moveData = new ObjectMoveData()
        {
            Speed = 1.0f,
            Direction = t,
        };

        dstManager.AddComponentData(_entity, bulletData);
        dstManager.AddComponentData(_entity, moveData);
    }
}
