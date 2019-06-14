using System;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

namespace GAME.DATA
{

    [Serializable]
    public struct BulletData : IComponentData
    {
        /// <summary> 弾の移動速度 </summary>
        public bool IsInitialized;
        /// <summary> 弾のダメージ値 </summary>
        public int Damage;
    }
}
