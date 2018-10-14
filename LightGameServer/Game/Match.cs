using System;
using LightEngineCore.Components;
using LightEngineCore.Loop;
using LightEngineCore.PhysicsEngine.Dynamics;
using LightEngineCore.PhysicsEngine.Primitives;
using LightEngineSerializeable.Utils;
using LightGameServer.NetworkHandling.Model;
using System.Collections.Generic;
using LightEngineSerializeable.LiteNetLib;
using LightEngineSerializeable.LiteNetLib.Utils;
using LightEngineSerializeable.SerializableClasses.DatabaseModel;
using LightEngineSerializeable.SerializableClasses.Enums;
using LightEngineSerializeable.SerializableClasses.GameModel;
using LightEngineSerializeable.SerializableClasses.GameModel.GameEvents;
using LightEngineSerializeable.SerializableClasses.GameModel.RequestEvents;
using LightEngineSerializeable.Utils.Serializers;
using LightGameServer.Game.Model;
using LightGameServer.Game.Prefabs.Skills;
using LightGameServer.Game.Prefabs.Static;
using LightGameServer.Game.Prefabs.Units;
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
        private bool _timerRunning;


        private PlayerType _playerTurn;
        private Invokable _everytingStopped;

        public Match(GameLoop gameLoop, PeerInfo playerOne, PeerInfo playerTwo)
        {
            this.gameLoop = gameLoop;
            this.playerOne = new PlayerInfo { PeerInfo = playerOne, PlayerType = PlayerType.PlayerOne, DeckIndex = 0 };
            this.playerTwo = playerTwo != null
                ? new PlayerInfo { PeerInfo = playerTwo, PlayerType = PlayerType.PlayerTwo, DeckIndex = 0 }
                : new PlayerInfo
                {
                    PeerInfo = new PeerInfo { PlayerData = new PlayerData { Name = "BOT" } },
                    PlayerType = PlayerType.PlayerTwo,
                    DeckIndex = 0
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
                    case RequestEventType.PlayUnitAbility:
                        if (GetPlayerInTurn().PeerInfo != peerInfo || !GetPlayerInTurn().CanPlay) return;
                        var abilityRequest = (PlayUnitAbilityRequest)request;
                        if (new Vector2(abilityRequest.DirectionX, abilityRequest.DirectionY) == Vector2.Zero) continue;
                        PlayUnitAbility(new Vector2(abilityRequest.DirectionX, abilityRequest.DirectionY));
                        break;
                    case RequestEventType.SetAimDirection:
                        RedirectRequest(SendOptions.Sequenced, request, GetOppositePeer(peerInfo));
                        break;
                }
            }
        }

        private void PlayUnitAbility(Vector2 direction)
        {
            SetAttackingFlag();
            _timerRunning = false;
            _sendPositions = true;
            GetPlayerInTurn().GetSelectedUnit().PlayAbility(direction);
            GetPlayerInTurn().IncrementDeckIndex();
            GetPlayerInTurn().CanPlay = false;
            SwitchTurns();
        }

        private void SetAttackingFlag()
        {
            foreach (var unit in GetPlayerInTurn().Deck)
            {
                unit.IsAttacking = false;
            }

            foreach (var unit in GetPlayerInTurn().Deck)
            {
                unit.IsAttacking = true;
            }
        }

        public PlayerInfo GetPlayerInTurn()
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
            new Wall(gameLoop, "FrontWall",
                10.9986f, 1f, new Vector2(0, 12));
            new Wall(gameLoop, "TopRightWall",
                5.85351f, 1f, new Vector2(6.69f, 10.22f), -45f);
            new Wall(gameLoop, "TopLeftWall",
                 5.85351f, 1f, new Vector2(-6.69f, 10.22f), 45f);
            new Wall(gameLoop, "BottomWall",
                10.9986f, 1f, new Vector2(0, -12));
            new Wall(gameLoop, "BottomRightWall",
                5.85351f, 1f, new Vector2(6.69f, -10.22f), 45f);
            new Wall(gameLoop, "BottomLeftWall",
                5.85351f, 1f, new Vector2(-6.69f, -10.22f), -45f);
            new Wall(gameLoop, "LeftWall",
                1f, 16.7883f, new Vector2(-8.45f, 0f));
            new Wall(gameLoop, "RightWall",
                1f, 16.7883f, new Vector2(8.45f, 0f));
        }

        private void CreateCasters()
        {
            playerOne.Deck = new Unit[]
            {
                new Caster1(this, playerOne, new Vector2(-4.5f, -5f)),
                new Caster2(this, playerOne, new Vector2(-2f, -7f)),
                new Caster3(this, playerOne, new Vector2(2f, -7f)),
                new Caster4(this, playerOne, new Vector2(4.5f, -5f))
            };

            playerTwo.Deck = new Unit[]
            {
                new Caster1(this, playerTwo, new Vector2(-4.5f, 5f)),
                new Caster2(this, playerTwo, new Vector2(-2f, 7f)),
                new Caster3(this, playerTwo, new Vector2(2f, 7f)),
                new Caster4(this, playerTwo, new Vector2(4.5f, 5f))
            };
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
                        ObjectType = (byte)objectType,
                        PositionX = go.Pos.X.ToShort(),
                        PositionY = go.Pos.Y.ToShort()
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
                    PlayerType = (byte)_playerTurn
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
                if (go is Skill)
                {
                    go.Destroy();
                    continue;
                }
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
            SendGameEventToPlayer(GetPlayerInTurn(), new CanPlayEvent { CanPlay = canPlay, SelectedUnitId = GetPlayerInTurn().GetSelectedGoId() });
            GetPlayerNotInTurn().CanPlay = !canPlay;
            SendGameEventToPlayer(GetPlayerNotInTurn(), new CanPlayEvent { CanPlay = !canPlay, SelectedUnitId = GetPlayerInTurn().GetSelectedGoId() });
        }

        private void PlayBotMove()
        {
            var dir = new Vector2(MathHelper.NextFloat(-1, 1f), MathHelper.NextFloat(-1, 1f));
            dir.Normalize();
            RedirectRequest(SendOptions.ReliableOrdered, new SetAimDirectionRequest { Active = true, DirectionX = dir.X, DirectionZ = dir.Y }, GetPlayerNotInTurn().PeerInfo.NetPeer);
            gameLoop.AddInvokable(new Invokable(() =>
            {
                RedirectRequest(SendOptions.ReliableOrdered, new SetAimDirectionRequest { Active = false }, GetPlayerNotInTurn().PeerInfo.NetPeer);
                PlayUnitAbility(dir);
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
                    SendGameEventToPlayer(GetPlayerNotInTurn(), new CanPlayEvent { CanPlay = false, SelectedUnitId = GetPlayerInTurn().GetSelectedGoId() });
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
                            PositionX = pos.X.ToShort(),
                            PositionY = pos.Y.ToShort(),
                        });
                    }
                }

                var sections = posEvents.SplitList();
                foreach (var section in sections)
                {
                    SendGameEventToPlayers(SendOptions.Sequenced, section.ToArray());
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

        public void SendGameEventToPlayers(SendOptions sendOption, params GameEvent[] gameEvents)
        {
            NetDataWriter writer = _gameEventSerializer.Serialize(gameEvents);
            if (playerOne.PeerInfo.IsConnected) DataSender.New(playerOne.PeerInfo.NetPeer).Send(writer, sendOption);
            if (playerTwo.PeerInfo.IsConnected) DataSender.New(playerTwo.PeerInfo.NetPeer).Send(writer, sendOption);
        }

        private void RedirectRequest(SendOptions sendOption, RequestEvent requestEvent, NetPeer peer)
        {
            if (peer == null) return;
            NetDataWriter writer = _requestEventSerializer.Serialize(requestEvent);
            DataSender.New(peer).Send(writer, sendOption);
        }
    }
}