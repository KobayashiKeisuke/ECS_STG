﻿using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

using GAME;
using GAME.DATA;


[RequiresEntityConversion]
public class PlayerMonoBehaviour : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField]
    private GameObject m_bulletObjectPrefab = null;
    [SerializeField, Range(0.1f, 5.0f)]
    private float m_spawnCycle = 1.0f;

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
        var bulletFactoryData = new BulletFactoryData()
        {
            ParentEntity = _entity,
            PositionOffset = float3.zero,
            RotationOffset = float3.zero,

            SpawnCycle = m_spawnCycle,
            SpawnTimer = m_spawnCycle,

            /* バレットパラメーター */
            Speed = 0.1f,
            MoveDirection = new float3(0f, 0f, 1f ),
            Damage = 1,
        };


        var bulletFactorySys = World.Active.GetOrCreateSystem<BulletFactorySystem>();
        if( bulletFactorySys != null )
        {
            Camera cam = Camera.main;
            float frustumHeight = cam.transform.position.y * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float frustumWidth = frustumHeight / Screen.height * Screen.width;

            float maxSize = math.max(frustumHeight, frustumWidth);
            int count = BulletFactorySystem.CalcPreloadObjectCount( maxSize, bulletFactoryData.Speed, 1.0f, bulletFactoryData.SpawnCycle);

            bulletFactoryData.BulletListHandler = bulletFactorySys.CreateBulletObject(count, m_bulletObjectPrefab, "MyBullet");
            Debug.Log($"Handler:{bulletFactoryData.BulletListHandler}");
        }

        dstManager.AddComponentData(_entity, moveData);
        dstManager.AddComponentData(_entity, playerData);
        dstManager.AddComponentData(_entity, bulletFactoryData);
    }
}
