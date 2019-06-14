using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GAME.INPUT
{
    public enum TOUCH_STATE
    {
        /// <summary> タッチなし </summary>
        NONE = 99 ,
        /* TouchPhase 準拠 */
        /// <summary> タッチ開始 </summary>
        BEGIN = 0,
        /// <summary> タッチ中 </summary>
        MOVED,
        /// <summary> 指を離す </summary>
        ENDED,
        /// <summary> タッチ追跡不可能 </summary>
        CANCEL,
    }
    /// <summary>
    /// 入力データ保持Component
    /// </summary>
    public struct InputData : IComponentData
    {
        public float2 ScreenPosition;
        public float CurrentTime;
        public float DeltaTime;
        public bool IsPressed;
    }
}
