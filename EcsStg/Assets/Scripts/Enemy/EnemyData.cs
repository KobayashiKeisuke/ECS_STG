using Unity.Entities;

namespace GAME.DATA
{
    public struct EnemyData : IComponentData
    {
        /// <summary> Enemy Instance Id </summary>
        public int Id;
        /// <summary> Hit Point </summary>
        public int HP;
        /// <summary> Score </summary>
        public int Score;
    }
}
