using System;
using LightEngineCore.Components;
using LightEngineCore.PhysicsEngine.Dynamics;
using LightEngineCore.PhysicsEngine.Primitives;
using LightEngineSerializeable.LiteNetLib;
using LightEngineSerializeable.SerializableClasses.DatabaseModel;
using LightEngineSerializeable.SerializableClasses.GameModel.GameEvents;
using LightGameServer.Game.Model;

namespace LightGameServer.Game.Prefabs.Units
{
    class Unit : GameObject
    {
        private float _drag = 1.2f;
        private float _restitution = 1f;
        private float _firction = 0f;

        public Match MyMatch { get; }

        public UnitSettings Settings { get; }
        public PlayerInfo Player { get; }
        public bool IsAttacking { get; set; }
        public short Hp { get; set; }
        public short MeeleDamage
        {
            get { return Settings.Damage; }
        }

        public short ProjectileDamage
        {
            get { return Settings.ProjectileDamage; }
        }

        public short PushForce
        {
            get { return Settings.PushForce; }
        }

        public bool IsAlive
        {
            get { return Hp > 0; }
        }


        public Unit(Match myMatch, PlayerInfo playerInfo, UnitSettings settings, Vector2 pos) : base(myMatch.gameLoop, settings.Name, new Rigidbody(myMatch.gameLoop, settings.Radius, settings.Density, pos, BodyType.Dynamic))
        {
            Hp = settings.Hp;
            MyMatch = myMatch;
            Player = playerInfo;
            Settings = settings;
            var body = GetComponent<Rigidbody>().body;
            body.LinearDamping = _drag;
            body.Restitution = _restitution;
            body.Friction = _firction;
            body.SleepingAllowed = true;
            this.onCollidedWithGameObject += OnCollidedWithGameObject;
        }

        public virtual void OnCollidedWithGameObject(GameObject go)
        {
            if (!IsAttacking) return;
            if (go is Unit)
            {
                if (Player.IsInDeck((Unit)go)) return;
                Console.WriteLine("I " + name + ", should attack " + go.name);
            }

        }

        public virtual void PlayAbility(Vector2 direction)
        {

        }

        public override void Destroy()
        {
            MyMatch.SendGameEventToPlayers(SendOptions.ReliableOrdered, new NetworkObjectDestroyEvent { Id = (ushort)id });
            base.Destroy();
        }
    }
}
