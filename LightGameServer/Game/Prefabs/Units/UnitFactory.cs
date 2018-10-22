using LightEngineCore.PhysicsEngine.Primitives;
using LightGameServer.Game.Model;

namespace LightGameServer.Game.Prefabs.Units
{
    static class UnitFactory
    {
        public static Unit CreateUnit(short unitId, Match match, PlayerInfo playerInfo, Vector2 pos)
        {
            switch (unitId)
            {
                case 1:
                    return new Caster1(match, playerInfo, pos);
                case 2:
                    return new Caster2(match, playerInfo, pos);
                case 3:
                    return new Caster3(match, playerInfo, pos);
                case 4:
                    return new Caster4(match, playerInfo, pos);
            }
            return null;
        }
    }
}
