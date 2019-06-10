using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 何らかのオブジェクト生成スポナーインターフェース
/// </summary>
public interface ISpawner
{
    /// <summary>
    /// スポーン
    /// </summary>
    void Spawn();
}