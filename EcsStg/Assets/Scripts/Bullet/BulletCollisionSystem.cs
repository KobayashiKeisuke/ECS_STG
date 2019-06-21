using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;

using GAME.DATA;

namespace GAME
{
    [UpdateAfter(typeof(BulletComponentSystem))]
    public class BulletCollisionSystem : ComponentSystem
    {

        private Entity m_playerEntity;

        public void Initialize( Entity _playerEntity )
        {
            m_playerEntity = _playerEntity;
        }

        // OnUpdate runs on the main thread.
        protected override void OnUpdate()
        {
            var manager = World.Active.EntityManager;
            var Data = manager.GetComponentData<PlayerData>( m_playerEntity );
            var PlayerPos = manager.GetComponentData<Translation>( m_playerEntity );

            Entities.ForEach( ( ref Translation _pos, ref RenderBounds _bounds, ref BulletData _bullet )=>
            {
                // EnemyBullet 以外は対象外
                if( _bullet.BulletType != 1 )
                {
                    return;
                }
                // 多段ヒット防止
                if( _bullet.IsCollide )
                {
                    return;
                }

                // 範囲内なら特になにもしない
                float3 bound = _bounds.Value.Extents;
                if( _pos.Value.x -bound.x <= PlayerPos.Value.x && PlayerPos.Value.x <= _pos.Value.x +bound.x
                && _pos.Value.y - bound.y <= PlayerPos.Value.y && PlayerPos.Value.y <= _pos.Value.y + bound.y
                && _pos.Value.z - bound.z <= PlayerPos.Value.z && PlayerPos.Value.z <= _pos.Value.z + bound.z)
                {
                    // バレットを消すように見せかける
                    _pos.Value.x = GameConst.SCREEN_WIDTH;
                    _pos.Value.z = GameConst.SCREEN_HEIGHT;
                    Data.Life--;
                    _bullet.IsCollide = true;
                }
            });

            manager.SetComponentData( m_playerEntity, Data );
        }
    }
}
