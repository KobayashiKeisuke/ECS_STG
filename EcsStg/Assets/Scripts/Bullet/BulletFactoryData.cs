using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace GAME.DATA
{
    public struct BulletFactoryData : IComponentData
    {
        /// <summary> 生成親 </summary>
        public Entity ParentEntity;
        /// <summary> 生成時のPositionOffset </summary>
        public float3 PositionOffset;
        /// <summary> 生成時のRotationOffset </summary>
        public float3 RotationOffset;

        /// <summary> バレットの生成サイクル </summary>
        public float SpawnCycle;
        /// <summary> バレットの生成カウントダウン </summary>
        public float SpawnTimer;
        /// <summary> バレットリストのハンドラ </summary>
        public int BulletListHandler;
        /* バレットパラメーター */
        /// <summary> フレームあたりの移動速度 </summary>
        public float Speed;
        /// <summary> 移動方向 </summary>
        public float3 MoveDirection;
        /// <summary> バレットのダメージ量 </summary>
        public int Damage;
    }
}
