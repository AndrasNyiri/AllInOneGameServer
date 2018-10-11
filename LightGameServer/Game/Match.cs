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
using LightGameServer.Game.Model;
using LightGameServer.NetworkHandling;
using MathHelper = LightEngineCore.Common.MathHelper;

namespace LightGameServer.Game
{
    class Match
    {


        public readonly GameLoop gameLoop;

        public readonly PlayerInfo playerOne;
        public readonly PlayerInfo playerTwo;

        private readonly GameEventSerializer _gameEventSerializer = new GameEventSerializer();
        private readonly RequestEventSerializer _requestEventSerializer = new RequestEventSerializer();

        private const float START_TIME = 2f;
        private const float TURN_TIME = 20f;

        private bool _sendPositions = true;
        private float _turnTimeLeft;
        private bool _timerRunning = false;


        private PlayerType _playerTurn;
        private Invokable _everytingStopped;

        public Match(GameLoop gameLoop, PeerInfo playerOne, PeerInfo playerTwo)
        {
            this.gameLoop = gameLoop;
            this.playerOne = new PlayerInfo { PeerInfo = playerOne, PlayerType = PlayerType.PlayerOne, SelectedGameObjectIndex = 0 };
            this.playerTwo = playerTwo != null
                ? new PlayerInfo { PeerInfo = playerTwo, PlayerType = PlayerType.PlayerTwo, SelectedGameObjectIndex = 0 }
                : new PlayerInfo
                {
                    PeerInfo = new PeerInfo { PlayerData = new PlayerData { Name = "BOT" } },
                    PlayerType = PlayerType.PlayerTwo,
                    SelectedGameObjectIndex = 0
                };

            _playerTurn = MathHelper.rng.Next(0, 2) == 1 ? PlayerType.PlayerOne : PlayerType.PlayerTwo;
            gameLoop.AddInvokable(new Invokable(Setup, START_TIME * 1000f, true));
        }

        public void ProcessRequests(PeerInfo peerInfo, params RequestEvent[] requests)
        {
            foreach (var request in requests)
            {
                switch (request.Type)
                {
                    case RequestEventType.PushGameObject:
                        if (GetPlayerInTurn().PeerInfo != peerInfo || !GetPlayerInTurn().CanPlay) return;
                        var pushRequest = (PushGameObjectRequest)request;
                        if (new Vector2(pushRequest.DirectionX, pushRequest.DirectionY) == Vector2.Zero) continue;
                        PushGameObject(GetPlayerInTurn().GetSelectedGoId(), new Vector2(pushRequest.DirectionX, pushRequest.DirectionY));
                        break;
                    case RequestEventType.SetAimDirection:
                        RedirectRequest(request, GetOppositePeer(peerInfo));
                        break;
                }
            }
        }

        private void PushGameObject(ushort id, Vector2 direction)
        {
            _timerRunning = false;
            _sendPositions = true;
            var gameObjectToPush = GetGameObject(id);
            Vector2 pushForce = direction;
            pushForce.Normalize();
            float pushPower = 200f;
            gameObjectToPush.GetComponent<Rigidbody>().body.ApplyLinearImpulse(pushForce * pushPower);
            GetPlayerInTurn().IncrementSelectedIndex();
            GetPlayerInTurn().CanPlay = false;
            SwitchTurns();
        }

        private PlayerInfo GetPlayerInTurn()
        {
            return _playerTurn == PlayerType.PlayerOne ? playerOne : playerTwo;
        }

        private PlayerInfo GetPlayerNotInTurn()
        {
            return _playerTurn == PlayerType.PlayerOne ? playerTwo : playerOne;
        }

        private NetPeer GetOppositePeer(PeerInfo peerInfo)
        {
            return playerOne.PeerInfo.PlayerData.PlayerId == peerInfo.PlayerData.PlayerId ? playerTwo.PeerInfo.NetPeer : playerOne.PeerInfo.NetPeer;
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
            playerOne.Deck = new[]
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

            playerTwo.Deck = new[]
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

            foreach (var caster in playerOne.Deck)
            {
                var body = caster.GetComponent<Rigidbody>().body;
                body.LinearDamping = 1.2f;
                body.Restitution = 1f;
                body.Friction = 0f;
                body.SleepingAllowed = true;
                gameLoop.RegisterGameObject(caster);
            }

            foreach (var caster in playerTwo.Deck)
            {
                var body = caster.GetComponent<Rigidbody>().body;
                body.LinearDamping = 1.2f;
                body.Restitution = 1f;
                body.Friction = 0f;
                body.SleepingAllowed = true;
                gameLoop.RegisterGameObject(caster);
            }




        }

        private void StartTimer()
        {
            _turnTimeLeft = TURN_TIME;
            _timerRunning = true;
        }

        private void SendGameStartEvents()
        {

            List<NetworkObjectSpawnEvent> spawnEvents = new List<NetworkObjectSpawnEvent>();
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
            SendGameEventToPlayer(playerOne, new GameStartEvent { LevelId = 1, NetworkTime = gameLoop.Time, PlayerType = playerOne.PlayerType, SpawnEvents = spawnEvents.ToArray(), CanPlay = GetPlayerInTurn() == playerOne });
            SendGameEventToPlayer(playerTwo, new GameStartEvent { LevelId = 1, NetworkTime = gameLoop.Time, PlayerType = playerTwo.PlayerType, SpawnEvents = spawnEvents.ToArray(), CanPlay = GetPlayerInTurn() == playerTwo });
        }

        private void SendTurnEvent()
        {

            SendGameEventToPlayers(SendOptions.ReliableOrdered,
                new TurnSyncEvent
                {
                    PlayerType = _playerTurn
                });
        }

        private void SwitchTurns(bool skipStopped = false)
        {
            _playerTurn = _playerTurn == PlayerType.PlayerOne ? PlayerType.PlayerTwo : PlayerType.PlayerOne;
            SendTurnEvent();

            if (!skipStopped)
            {
                _everytingStopped = gameLoop.AddInvokable(new Invokable(() =>
                {
                    bool stopped = true;
                    foreach (var go in gameLoop.activeObjects)
                    {
                        var rb = go.GetComponent<Rigidbody>();
                        if (rb != null && rb.body.LinearVelocity.Length() > 0.1f)
                        {
                            stopped = false;
                        }
                    }

                    if (stopped)
                    {
                        OnEverytingStopped();
                    }
                }));
            }
        }

        private void OnEverytingStopped()
        {
            foreach (var go in gameLoop.activeObjects)
            {
                var rb = go.GetComponent<Rigidbody>();
                if (rb != null) rb.body.LinearVelocity = Vector2.Zero;
            }

            StartTimer();
            _everytingStopped.Interrupt();
            _sendPositions = false;

            SendCanPlay(true);
            if (!GetPlayerInTurn().PeerInfo.IsConnected)
            {
                PlayBotMove();
            }
        }

        private void SendCanPlay(bool canPlay)
        {
            GetPlayerInTurn().CanPlay = canPlay;
            SendGameEventToPlayer(GetPlayerInTurn(), new CanPlayEvent { CanPlay = canPlay });
            GetPlayerNotInTurn().CanPlay = !canPlay;
            SendGameEventToPlayer(GetPlayerNotInTurn(), new CanPlayEvent { CanPlay = !canPlay });
        }

        private void PlayBotMove()
        {
            var dir = new Vector2(MathHelper.NextFloat(-1, 1f), MathHelper.NextFloat(-1, 1f));
            RedirectRequest(new SetAimDirectionRequest { Active = true, DirectionX = dir.X, DirectionZ = dir.Y }, GetPlayerNotInTurn().PeerInfo.NetPeer);
            gameLoop.AddInvokable(new Invokable(() =>
            {
                RedirectRequest(new SetAimDirectionRequest { Active = false }, GetPlayerNotInTurn().PeerInfo.NetPeer);
                PushGameObject(GetPlayerInTurn().GetSelectedGoId(), dir);
            }, 2000f, true));
        }

        private void Setup()
        {
            BuildWalls();
            CreateCasters();
            SendGameStartEvents();
            SendTurnEvent();
            SendCanPlay(true);
            StartTimer();
            gameLoop.AddInvokable(new Invokable(() =>
            {
                if (!_timerRunning) return;
                _turnTimeLeft -= gameLoop.DeltaTime;
                if (_turnTimeLeft <= 0)
                {
                    GetPlayerInTurn().CanPlay = false;
                    SwitchTurns(true);
                    GetPlayerInTurn().CanPlay = true;
                    SendGameEventToPlayer(GetPlayerNotInTurn(), new CanPlayEvent { CanPlay = false });
                    StartTimer();
                    if (!GetPlayerInTurn().PeerInfo.IsConnected)
                    {
                        PlayBotMove();
                    }
                }
            }));


            gameLoop.AddInvokable(new Invokable(() =>
            {
                if (!_sendPositions) return;

                List<GameEvent> posEvents = new List<GameEvent>();
                foreach (var go in gameLoop.activeObjects)
                {
                    Rigidbody goRig = go.GetComponent<Rigidbody>();
                    if (goRig != null && goRig.body.BodyType == BodyType.Dynamic)
                    {
                        var pos = go.Pos;
                        posEvents.Add(new PositionSyncEvent
                        {
                            Id = (ushort)go.id,
                            PositionX = pos.X,
                            PositionY = pos.Y,
                        });
                    }
                }

                var sections = posEvents.SplitList();
                foreach (var section in sections)
                {
                    SendGameEventToPlayers(SendOptions.Unreliable, section.ToArray());
                }
            }, Server.UPDATE_TIME));


            if (!GetPlayerInTurn().PeerInfo.IsConnected)
            {
                PlayBotMove();
            }
        }

        private void SendGameEventToPlayer(PlayerInfo player, GameEvent gameEvent)
        {
            NetDataWriter writer = _gameEventSerializer.Serialize(gameEvent);
            if (player.PeerInfo.IsConnected) DataSender.New(player.PeerInfo.NetPeer).Send(writer, SendOptions.ReliableOrdered);
        }

        private void SendGameEventToPlayers(SendOptions sendOption, params GameEvent[] gameEvents)
        {
            NetDataWriter writer = _gameEventSerializer.Serialize(gameEvents);
            if (playerOne.PeerInfo.IsConnected) DataSender.New(playerOne.PeerInfo.NetPeer).Send(writer, sendOption);
            if (playerTwo.PeerInfo.IsConnected) DataSender.New(playerTwo.PeerInfo.NetPeer).Send(writer, sendOption);
        }

        private void RedirectRequest(RequestEvent requestEvent, NetPeer peer)
        {
            if (peer == null) return;
            NetDataWriter writer = _requestEventSerializer.Serialize(requestEvent);
            DataSender.New(peer).Send(writer, SendOptions.ReliableOrdered);
        }
    }
}