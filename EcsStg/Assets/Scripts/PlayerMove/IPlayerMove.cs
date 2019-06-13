using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GAME
{
    public interface IPlayerMove
    {
        /// <summary>
        /// 極座標パラメータで入力を受け取る
        /// </summary>
        /// <param name="_range"></param>
        /// <param name="_theta"></param>
        void OnMove( float _range, float _theta );
    }
}