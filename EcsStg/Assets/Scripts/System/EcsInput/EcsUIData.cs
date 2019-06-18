using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace GAME.DATA
{
    /// <summary>
    /// 単純な 1:1 の親子構造を持つEntityData
    /// </summary>
    public struct EcsUIData : IComponentData
    {
        public Entity Parent;
        public Entity Child;
    }
}