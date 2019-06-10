using System;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

namespace GAME.Entity
{

    [Serializable]
    public struct BulletData : IComponentData
    {
        /// <summary> 弾の移動速度 </summary>
        public float Speed;

        /// <summary> 弾の移動方向 </summary>
        public Translation Direction;

        /// <summary> 弾のダメージ値 </summary>
        public int Damage;
    }
}
