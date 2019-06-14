using System;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

namespace GAME.DATA
{
    /// <summary>
    /// 毎フレーム回転するオブジェクトのデータ
    /// </summary>
    [Serializable]
    public struct ObjectRotateData : IComponentData
    {
        /// <summary> 旋回時間 </summary>
        public float LerpTime;
        /// <summary> 現在旋回時間 </summary>
        public float CurrentLerpTime;
        /// <summary> 回転方向 </summary>
        public Rotation Rot;
    }
}
