using System;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

namespace GAME.DATA
{
    /// <summary>
    /// 毎フレーム移動するオブジェクトのデータ
    /// </summary>
    [Serializable]
    public struct ObjectMoveData : IComponentData
    {
        /// <summary> 移動速度 </summary>
        public float Speed;
        /// <summary> 移動方向 </summary>
        public Translation Direction;
    }
}
