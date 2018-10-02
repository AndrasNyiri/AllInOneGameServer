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
using MathHelper = LightEngineCore.Common.MathHelper;

namespace LightGameServer.Game
{
    class Match
    {
        public readonly GameLoop gameLoop;

        private readonly PeerInfo _playerOne;
        private readonly PeerInfo _playerTwo;

        private const double DEGREES_TO_RADIANS = (double)(Math.PI / 180);

        private readonly GameEventSerializer _gameEventSerializer = new GameEventSerializer();

        public Match(GameLoop gameLoop, PeerInfo playerOne, PeerInfo playerTwo)
        {
            this.gameLoop = gameLoop;
            _playerOne = playerOne;
            _playerTwo = playerTwo;
            Setup();
        }

        public Match(GameLoop gameLoop, PeerInfo playerOne)
        {
            this.gameLoop = gameLoop;
            _playerOne = playerOne;
            _playerTwo = new PeerInfo { PlayerData = new PlayerData { Name = "BOT" } };
            gameLoop.AddInvokable(new Invokable(Setup, 2000f, true));
        }

        private void Setup()
        {
            GameObject frontWall = new GameObject("FrontWall", new Rigidbody(gameLoop.physicsWorld, 10.9986f, 1f, 1f, new Vector2(0, 12), BodyType.Static));
            GameObject topRightWall = new GameObject("TopRightWall", new Rigidbody(gameLoop.physicsWorld, 5.85351f, 1f, 1f, new Vector2(6.69f, 10.22f), BodyType.Static, (float)(-45f * DEGREES_TO_RADIANS)));
            GameObject topLeftWall = new GameObject("TopLeftWall", new Rigidbody(gameLoop.physicsWorld, 5.85351f, 1f, 1f, new Vector2(-6.69f, 10.22f), BodyType.Static, (float)(45f * DEGREES_TO_RADIANS)));


            GameObject bottomWall = new GameObject("BottomWall", new Rigidbody(gameLoop.physicsWorld, 10.9986f, 1f, 1f, new Vector2(0, -12), BodyType.Static));
            GameObject bottomRightWall = new GameObject("BottomRightWall", new Rigidbody(gameLoop.physicsWorld, 5.85351f, 1f, 1f, new Vector2(6.69f, -10.22f), BodyType.Static, (float)(45f * DEGREES_TO_RADIANS)));
            GameObject bottomLeftWall = new GameObject("BottomLeftWall", new Rigidbody(gameLoop.physicsWorld, 5.85351f, 1f, 1f, new Vector2(-6.69f, -10.22f), BodyType.Static, (float)(-45f * DEGREES_TO_RADIANS)));

            GameObject leftWall = new GameObject("LeftWall", new Rigidbody(gameLoop.physicsWorld, 1, 16.7883f, 1f, new Vector2(-8.45f, 0f), BodyType.Static));
            GameObject rightWall = new GameObject("RightWall", new Rigidbody(gameLoop.physicsWorld, 1, 16.7883f, 1f, new Vector2(8.45f, 0f), BodyType.Static));

            gameLoop.RegisterGameObject(frontWall);
            gameLoop.RegisterGameObject(topRightWall);
            gameLoop.RegisterGameObject(topLeftWall);

            gameLoop.RegisterGameObject(bottomWall);
            gameLoop.RegisterGameObject(bottomRightWall);
            gameLoop.RegisterGameObject(bottomLeftWall);

            gameLoop.RegisterGameObject(leftWall);
            gameLoop.RegisterGameObject(rightWall);

            gameLoop.AddInvokable(new Invokable(() =>
            {
                float randomX = MathHelper.NextFloat(-6, 6);
                float randomY = MathHelper.NextFloat(-6, 6);
                GameObject newGameObject = new GameObject("Caster",
                    new Rigidbody(gameLoop.physicsWorld, 0.75f, 1f, new Vector2(randomX, randomY), BodyType.Dynamic));

                gameLoop.RegisterGameObject(newGameObject);

                newGameObject.GetComponent<Rigidbody>().body.ApplyLinearImpulse(new Vector2(MathHelper.NextFloat(-30, 30), MathHelper.NextFloat(-30, 30)));
                newGameObject.GetComponent<Rigidbody>().body.LinearDamping = 0.5f;

                SendGameEventToPlayers(SendOptions.ReliableOrdered
                    , new NetworkObjectSpawnEvent { Id = (int)newGameObject.id, ObjectType = NetworkObjectType.Caster1, PositionX = randomX, PositionY = randomY });

            }, 500f));

            gameLoop.AddInvokable(new Invokable(() =>
            {
                List<GameEvent> posEvents = new List<GameEvent>();
                foreach (var go in gameLoop.activeObjects)
                {
                    Rigidbody goRig = go.GetComponent<Rigidbody>();
                    if (goRig != null && goRig.body.BodyType == BodyType.Dynamic)
                    {
                        posEvents.Add(new PositionSyncEvent { Id = (int)go.id, PositionX = go.Pos.X, PositionY = go.Pos.Y });
                    }
                }

                if (posEvents.Count > 0) SendGameEventToPlayers(SendOptions.ReliableUnordered, posEvents.ToArray());

            }, 15));
        }

        private void SendGameEventToPlayers(SendOptions sendOption, params GameEvent[] gameEvents)
        {
            NetDataWriter writer = _gameEventSerializer.Serialize(gameEvents);
            if (_playerOne.Peer != null) DataSender.New(_playerOne.Peer).Send(writer, sendOption);
            if (_playerTwo.Peer != null) DataSender.New(_playerTwo.Peer).Send(writer, sendOption);
        }

    }
}
