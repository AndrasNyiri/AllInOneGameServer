using System;
using LightEngineCore.Components;
using LightEngineCore.Loop;
using LightEngineCore.PhysicsEngine.Dynamics;
using LightEngineCore.PhysicsEngine.Primitives;
using LightEngineSerializeable.LiteNetLib;
using LightEngineSerializeable.LiteNetLib.Utils;
using LightEngineSerializeable.SerializableClasses.DatabaseModel;
using LightEngineSerializeable.SerializableClasses.Enums;
using LightEngineSerializeable.SerializableClasses.GameModel.GameEvents;
using LightEngineSerializeable.Utils;
using LightGameServer.Game.Model;
using LightGameServer.Game.Prefabs.Units;

namespace LightGameServer.Game.Prefabs.Skills
{
    abstract class Skill : GameObject
    {
        private float _drag = 1.2f;
        private float _restitution = 1f;
        private float _firction = 0f;


        public short Damage { get; }
        public PlayerInfo Player { get; }
        public SkillSettings Settings { get; }
        public Match MyMatch { get; }
        public NetworkObjectType Type { get; }

        protected Skill(Match match, PlayerInfo player, NetworkObjectType type, SkillSettings skillSettings, Vector2 pos, Vector2 pushDir, short damage) : base(match.gameLoop, skillSettings.Name, new Rigidbody(match.gameLoop, skillSettings.Radius, skillSettings.Density, pos, BodyType.Dynamic))
        {
            var body = GetComponent<Rigidbody>().body;
            body.LinearDamping = _drag;
            body.Restitution = _restitution;
            body.Friction = _firction;
            body.SleepingAllowed = true;

            Type = type;
            MyMatch = match;
            Player = player;
            Settings = skillSettings;
            Damage = damage;
            this.onCollidedWithGameObject += onCollidedWithGameObject;
            SendSpawnEvent();
        }


        public virtual void OnCollidedWithGameObject(GameObject go)
        {
            if (go is Unit)
            {
                if (Player.IsInDeck((Unit)go)) return;
                Console.WriteLine("I " + name + ", should attack " + go.name);
            }
        }

        private void SendSpawnEvent()
        {
            var spawnEvent = new NetworkObjectSpawnEvent
            {
                Id = (ushort)id,
                ObjectType = (byte)Type,
                PositionX = Pos.X.ToShort(),
                PositionY = Pos.Y.ToShort(),
            };
            MyMatch.SendGameEventToPlayers(SendOptions.ReliableOrdered, spawnEvent);
        }

        public override void Destroy()
        {
            MyMatch.SendGameEventToPlayers(SendOptions.ReliableOrdered, new NetworkObjectDestroyEvent { Id = (ushort)id });
            base.Destroy();
        }
    }

}
