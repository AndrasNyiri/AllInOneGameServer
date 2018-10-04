using System;
using LightEngineCore.Components;
using LightEngineCore.Loop;
using LightEngineCore.PhysicsEngine.Dynamics;
using LightEngineCore.PhysicsEngine.Primitives;
using LightEngineSerializeable.Utils;
using LightGameServer.NetworkHandling.Model;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections.Generic;
using System.Linq;
using LightGameServer.Game.Components.Scripts;
using MathHelper = LightEngineCore.Common.MathHelper;

namespace LightGameServer.Game
{
    class Match
    {
        public readonly GameLoop gameLoop;

        public readonly PeerInfo playerOne;
        public readonly PeerInfo playerTwo;

        private readonly GameEventSerializer _gameEventSerializer = new GameEventSerializer();

        private GameObject[] _deckPlayerOne;
        private GameObject[] _deckPlayerTwo;


        private const float START_TIME = 2f;

        private bool _sendPositions = true;

        public Match(GameLoop gameLoop, PeerInfo playerOne, PeerInfo playerTwo)
        {
            this.gameLoop = gameLoop;
            this.playerOne = playerOne;
            this.playerTwo = playerTwo != null ? playerTwo : new PeerInfo { PlayerData = new PlayerData { Name = "BOT" } }; ;
            gameLoop.AddInvokable(new Invokable(Setup, START_TIME * 1000f, true));
        }

        public void AddRequests(params RequestEvent[] requests)
        {
            foreach (var request in requests)
            {
                switch (request.Type)
                {
                    case RequestEventType.PushGameObject:
                        var pushRequest = (PushGameObjectRequest)request;
                        var gameObjectToPush = GetGameObject(pushRequest.GameObjectId);
                        Vector2 pushForce = new Vector2(pushRequest.DirectionX, pushRequest.DirectionY);
                        pushForce.Normalize();
                        gameObjectToPush.GetComponent<Rigidbody>().body.ApplyLinearImpulse(pushForce * 100f);
                        break;
                }
            }
        }

        private GameObject GetGameObject(ulong id)
        {
            foreach (var go in gameLoop.activeObjects)
            {
                if (go.id == id) return go;
            }

            return null;
        }

        private void BuildWalls()
        {
            GameObject frontWall = new GameObject("FrontWall",
                new Rigidbody(gameLoop.physicsWorld, 10.9986f, 1f, 1f, new Vector2(0, 12), BodyType.Static));
            GameObject topRightWall = new GameObject("TopRightWall",
                new Rigidbody(gameLoop.physicsWorld, 5.85351f, 1f, 1f, new Vector2(6.69f, 10.22f), BodyType.Static,
                    -45f.ToRadians()));
            GameObject topLeftWall = new GameObject("TopLeftWall",
                new Rigidbody(gameLoop.physicsWorld, 5.85351f, 1f, 1f, new Vector2(-6.69f, 10.22f), BodyType.Static,
                    45f.ToRadians()));


            GameObject bottomWall = new GameObject("BottomWall",
                new Rigidbody(gameLoop.physicsWorld, 10.9986f, 1f, 1f, new Vector2(0, -12), BodyType.Static));
            GameObject bottomRightWall = new GameObject("BottomRightWall",
                new Rigidbody(gameLoop.physicsWorld, 5.85351f, 1f, 1f, new Vector2(6.69f, -10.22f), BodyType.Static,
                    45f.ToRadians()));
            GameObject bottomLeftWall = new GameObject("BottomLeftWall",
                new Rigidbody(gameLoop.physicsWorld, 5.85351f, 1f, 1f, new Vector2(-6.69f, -10.22f), BodyType.Static,
                    -45f.ToRadians()));

            GameObject leftWall = new GameObject("LeftWall",
                new Rigidbody(gameLoop.physicsWorld, 1, 16.7883f, 1f, new Vector2(-8.45f, 0f), BodyType.Static));
            GameObject rightWall = new GameObject("RightWall",
                new Rigidbody(gameLoop.physicsWorld, 1, 16.7883f, 1f, new Vector2(8.45f, 0f), BodyType.Static));

            gameLoop.RegisterGameObject(frontWall);
            gameLoop.RegisterGameObject(topRightWall);
            gameLoop.RegisterGameObject(topLeftWall);

            gameLoop.RegisterGameObject(bottomWall);
            gameLoop.RegisterGameObject(bottomRightWall);
            gameLoop.RegisterGameObject(bottomLeftWall);

            gameLoop.RegisterGameObject(leftWall);
            gameLoop.RegisterGameObject(rightWall);
        }

        private void CreateCasters()
        {
            _deckPlayerOne = new[]
            {
                new GameObject("Caster1",
                    new Rigidbody(gameLoop.physicsWorld, 0.75f, 1f, new Vector2(-4.5f, -5f), BodyType.Dynamic)),
                new GameObject("Caster2",
                    new Rigidbody(gameLoop.physicsWorld, 0.75f, 1f, new Vector2(-2f, -7f), BodyType.Dynamic)),
                new GameObject("Caster3",
                    new Rigidbody(gameLoop.physicsWorld, 0.75f, 1f, new Vector2(2f, -7f), BodyType.Dynamic)),
                new GameObject("Caster4",
                    new Rigidbody(gameLoop.physicsWorld, 0.75f, 1f, new Vector2(4.5f, -5f), BodyType.Dynamic))
            };

            _deckPlayerTwo = new[]
            {
                new GameObject("Caster1",
                    new Rigidbody(gameLoop.physicsWorld, 0.75f, 1f, new Vector2(-4.5f, 5f), BodyType.Dynamic)),
                new GameObject("Caster2",
                    new Rigidbody(gameLoop.physicsWorld, 0.75f, 1f, new Vector2(-2f, 7f), BodyType.Dynamic)),
                new GameObject("Caster3",
                    new Rigidbody(gameLoop.physicsWorld, 0.75f, 1f, new Vector2(2f, 7f), BodyType.Dynamic)),
                new GameObject("Caster4",
                    new Rigidbody(gameLoop.physicsWorld, 0.75f, 1f, new Vector2(4.5f, 5f), BodyType.Dynamic))
            };

            foreach (var caster in _deckPlayerOne)
            {
                var body = caster.GetComponent<Rigidbody>().body;
                body.LinearDamping = 1.5f;
                body.Restitution = 0.8f;
                body.Friction = 0.3f;
                gameLoop.RegisterGameObject(caster);
            }

            foreach (var caster in _deckPlayerTwo)
            {
                var body = caster.GetComponent<Rigidbody>().body;
                body.LinearDamping = 1.5f;
                body.Restitution = 0.8f;
                body.Friction = 0.3f;
                gameLoop.RegisterGameObject(caster);
            }

            SendCasterSpawnEvents();
        }

        private void SendCasterSpawnEvents()
        {

            List<GameEvent> spawnEvents = new List<GameEvent>();
            foreach (var go in gameLoop.activeObjects)
            {
                Rigidbody goRig = go.GetComponent<Rigidbody>();
                if (goRig != null && goRig.body.BodyType == BodyType.Dynamic)
                {
                    NetworkObjectType objectType = (NetworkObjectType)Enum.Parse(typeof(NetworkObjectType), go.name);
                    spawnEvents.Add(new NetworkObjectSpawnEvent
                    {
                        Id = (ushort)go.id,
                        ObjectType = objectType,
                        PositionX = go.Pos.X,
                        PositionY = go.Pos.Y
                    });
                }
            }

            SendGameEventToPlayers(SendOptions.ReliableOrdered, spawnEvents.ToArray());
        }

        private void Setup()
        {
            BuildWalls();
            CreateCasters();

            gameLoop.AddInvokable(new Invokable(() =>
            {
                if (!_sendPositions) return;

                List<GameEvent> posEvents = new List<GameEvent>();
                foreach (var go in gameLoop.activeObjects)
                {
                    Rigidbody goRig = go.GetComponent<Rigidbody>();
                    if (goRig != null && goRig.body.BodyType == BodyType.Dynamic)
                    {
                        posEvents.Add(new PositionSyncEvent
                        {
                            Id = (ushort)go.id,
                            PositionX = go.Pos.X,
                            PositionY = go.Pos.Y,
                            TimeStamp = gameLoop.Time
                        });
                    }
                }

                var sections = posEvents.SplitList();
                foreach (var section in sections)
                {
                    SendGameEventToPlayers(SendOptions.Unreliable, section.ToArray());
                }
            }, 15));
        }

        private void SendGameEventToPlayers(SendOptions sendOption, params GameEvent[] gameEvents)
        {
            NetDataWriter writer = _gameEventSerializer.Serialize(gameEvents);
            if (playerOne.Peer != null) DataSender.New(playerOne.Peer).Send(writer, sendOption);
            if (playerTwo.Peer != null) DataSender.New(playerTwo.Peer).Send(writer, sendOption);
        }
    }
}