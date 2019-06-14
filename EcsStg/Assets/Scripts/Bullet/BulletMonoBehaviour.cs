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
        t.Value = new float3(0f, 0f, -0.03f);

        var data = new BulletData(){
            Speed = 1.0f,
            Direction = t,
            Damage = 1,
        };
        dstManager.AddComponentData(_entity, data);
    }
}
