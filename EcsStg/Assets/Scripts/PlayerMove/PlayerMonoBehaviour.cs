
using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

using GAME.DATA;


[RequiresEntityConversion]
public class PlayerMonoBehaviour : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert( Entity _entity, EntityManager dstManager, GameObjectConversionSystem _conversionSystem )
    {
        Translation t = new Translation();
        t.Value = new float3(0f, 0f, 0.0f);

        var moveData = new ObjectMoveData()
        {
            Speed = 1.0f,
            Direction = t,
        };

        var playerData = new PlayerData();

        dstManager.AddComponentData(_entity, moveData);
        dstManager.AddComponentData(_entity, playerData);
    }
}
