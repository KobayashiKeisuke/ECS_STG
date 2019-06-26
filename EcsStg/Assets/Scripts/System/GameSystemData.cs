using UnityEngine;
using Unity.Entities;

namespace GAME.DATA
{
    public struct GameSystemData : IComponentData
    {
        /// <summary> GameOverフラグ </summary>
        public bool IsGameOver;
        /// <summary> GameOverフラグ </summary>
        public int Score;
    }
}
