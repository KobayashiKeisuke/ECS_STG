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
    public struct PlayerData : IComponentData
    {
        /// <summary> 残機 </summary>
        public int Life;
        /// <summary> ボムの数 </summary>
        public int BombCount;
        /// <summary> アタックレベル </summary>
        public int AtkLevel;
        /// <summary> 移動速度レベル </summary>
        public int SpeedLevel;
        /// <summary> ゲームオーバー判定 </summary>
        public bool IsGameOver;
    }
}
