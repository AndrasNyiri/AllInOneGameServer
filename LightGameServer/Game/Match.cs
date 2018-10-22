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
using LightEngineSerializeable.SerializableClasses;
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
                    PeerInfo = new PeerInfo
                    {
                        PlayerData = new PlayerData
                        {
                            Name = "BOT",
                            DeviceId = "",
                            OwnedUnits = new PlayerUnit[0],
                            Deck = playerOne.PlayerData.Deck
                        }
                    },
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
            GetPlayerInTurn().CanPlay = false;
            SwitchTurns();
        }

        private void SetAttackingFlag()
        {
            foreach (var unit in GetPlayerNotInTurn().Deck)
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

        private PlayerInfo GetOppositePlayer(PlayerInfo player)
        {
            return player == playerOne ? playerTwo : playerOne;
        }

        private void BuildWalls()
        {
            new Wall(gameLoop, "FrontWall",
                12.03f, 1f, new Vector2(0, 12));
            new Wall(gameLoop, "TopRightWall",
                5.85351f, 1f, new Vector2(8f, 10.22f), -45f);
            new Wall(gameLoop, "TopLeftWall",
                5.85351f, 1f, new Vector2(-8f, 10.22f), 45f);
            new Wall(gameLoop, "BottomWall",
                12.03f, 1f, new Vector2(0, -12));
            new Wall(gameLoop, "BottomRightWall",
                5.85351f, 1f, new Vector2(8f, -10.22f), 45f);
            new Wall(gameLoop, "BottomLeftWall",
                5.85351f, 1f, new Vector2(-8f, -10.22f), -45f);
            new Wall(gameLoop, "LeftWall",
                1f, 16.7883f, new Vector2(-10f, 0f));
            new Wall(gameLoop, "RightWall",
                1f, 16.7883f, new Vector2(10f, 0f));
        }

        private void CreateCasters()
        {
            playerOne.Deck = new[]
            {
                UnitFactory.CreateUnit(playerOne.PeerInfo.PlayerData.Deck.UnitOne,this, playerOne, new Vector2(-4.5f, -5f)),
                UnitFactory.CreateUnit(playerOne.PeerInfo.PlayerData.Deck.UnitTwo,this, playerOne, new Vector2(-2f, -7f)),
                UnitFactory.CreateUnit(playerOne.PeerInfo.PlayerData.Deck.UnitThree,this, playerOne, new Vector2(2f, -7f)),
                UnitFactory.CreateUnit(playerOne.PeerInfo.PlayerData.Deck.UnitFour,this, playerOne, new Vector2(4.5f, -5f))
            };

            playerTwo.Deck = new[]
            {
                UnitFactory.CreateUnit(playerTwo.PeerInfo.PlayerData.Deck.UnitOne,this, playerTwo, new Vector2(-4.5f, 5f)),
                UnitFactory.CreateUnit(playerTwo.PeerInfo.PlayerData.Deck.UnitTwo,this, playerTwo, new Vector2(-2f, 7f)),
                UnitFactory.CreateUnit(playerTwo.PeerInfo.PlayerData.Deck.UnitThree,this, playerTwo, new Vector2(2f, 7f)),
                UnitFactory.CreateUnit(playerTwo.PeerInfo.PlayerData.Deck.UnitFour,this, playerTwo, new Vector2(4.5f, 5f))
            };
        }

        private void ResetTimer()
        {
            _turnTimeLeft = TURN_TIME;
            _timerRunning = true;
        }

        private void SendGameStartEvent(PlayerInfo playerInfo)
        {
            List<NetworkObjectSpawnEvent> spawnEvents = new List<NetworkObjectSpawnEvent>();
            List<GameEvent> otherEvents = new List<GameEvent>();
            foreach (var go in gameLoop.activeObjects)
            {
                if (go is Unit)
                {
                    Unit unit = (Unit)go;
                    if (!unit.IsAlive) continue;
                    NetworkObjectType objectType = (NetworkObjectType)Enum.Parse(typeof(NetworkObjectType), go.name);
                    spawnEvents.Add(new NetworkObjectSpawnEvent
                    {
                        Id = go.id,
                        ObjectType = (byte)objectType,
                        PositionX = go.Pos.X.ToShort(),
                        PositionY = go.Pos.Y.ToShort(),
                        Owner = (byte)unit.Player.PlayerType
                    });
                    otherEvents.Add(new UnitHealthSyncEvent
                    {
                        Id = go.id,
                        CurrentHealth = unit.Hp
                    });
                }
            }

            SendGameEventToPlayer(playerInfo,
                new GameStartEvent
                {
                    LevelId = 1,
                    PlayerType = playerInfo.PlayerType,
                    SpawnEvents = spawnEvents.ToArray(),
                    EnemyPlayerData = GetOppositePlayer(playerInfo).PeerInfo.PlayerData,
                    MyDeckBind = new[]
                    {
                        new DeckGameObjectBind{DeckIndex = 0, GameObjectId = playerInfo.Deck[0].id},
                        new DeckGameObjectBind{DeckIndex = 1, GameObjectId = playerInfo.Deck[1].id},
                        new DeckGameObjectBind{DeckIndex = 2, GameObjectId = playerInfo.Deck[2].id},
                        new DeckGameObjectBind{DeckIndex = 3, GameObjectId = playerInfo.Deck[3].id}
                    },
                    EnemyDeckBind = new[]
                    {
                        new DeckGameObjectBind{DeckIndex = 0, GameObjectId = GetOppositePlayer(playerInfo).Deck[0].id},
                        new DeckGameObjectBind{DeckIndex = 1, GameObjectId = GetOppositePlayer(playerInfo).Deck[1].id},
                        new DeckGameObjectBind{DeckIndex = 2, GameObjectId = GetOppositePlayer(playerInfo).Deck[2].id},
                        new DeckGameObjectBind{DeckIndex = 3, GameObjectId = GetOppositePlayer(playerInfo).Deck[3].id}
                    }
                });
            SendGameEventToPlayer(playerInfo, otherEvents.ToArray());
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
                StartCheckEverytingStopped();
            }
            else
            {
                if (!GetPlayerNotInTurn().IncrementDeckIndex())
                {
                    EndGame();
                }
            }
        }

        private void StartCheckEverytingStopped()
        {
            _everytingStopped = gameLoop.AddInvokable(new Invokable(() =>
            {
                if (IsEverytingStopped())
                {
                    OnEverytingStopped();
                }
            }));
        }

        private bool IsEverytingStopped()
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

            return stopped;
        }

        private void OnEverytingStopped()
        {

            foreach (var go in gameLoop.activeObjects)
            {
                var rb = go.GetComponent<Rigidbody>();
                if (rb != null) rb.body.LinearVelocity = Vector2.Zero;
            }

            foreach (var go in gameLoop.activeObjects)
            {
                if (go is Skill)
                {
                    go.Destroy();
                }
            }

            if (!IsEverytingStopped())
            {
                return;
            }


            _everytingStopped.Interrupt();

            foreach (var go in gameLoop.activeObjects)
            {
                if (go is Unit)
                {
                    var unit = (Unit)go;
                    if (!unit.IsAlive && !unit.Destroyed)
                    {
                        unit.Destroy();
                        if (GetPlayerInTurn().GetSelectedUnit() == unit)
                        {
                            if (!GetPlayerInTurn().IncrementDeckIndex())
                            {
                                EndGame();
                                return;
                            }
                        }
                    }
                }
            }

            if (!GetPlayerNotInTurn().IncrementDeckIndex())
            {
                EndGame();
                return;
            }
            ResetTimer();
            _sendPositions = false;

            SendCanPlay();
            if (!GetPlayerInTurn().PeerInfo.IsConnected)
            {
                PlayBotMove();
            }
        }

        private void SendCanPlay()
        {
            GetPlayerInTurn().CanPlay = true;
            SendGameEventToPlayer(GetPlayerInTurn(), new CanPlayEvent { CanPlay = true, SelectedUnitId = GetPlayerInTurn().GetSelectedGoId() });
            GetPlayerNotInTurn().CanPlay = false;
            SendGameEventToPlayer(GetPlayerNotInTurn(), new CanPlayEvent { CanPlay = false, SelectedUnitId = GetPlayerInTurn().GetSelectedGoId() });
        }

        private void PlayBotMove()
        {
            var dir = new Vector2(MathHelper.NextFloat(-1, 1f), MathHelper.NextFloat(-1, 1f));
            dir.Normalize();
            gameLoop.AddInvokable(new Invokable(() =>
            {
                RedirectRequest(SendOptions.ReliableOrdered, new SetAimDirectionRequest { Active = true, DirectionX = dir.X, DirectionZ = dir.Y }, GetPlayerNotInTurn().PeerInfo.NetPeer);
            }, 100f, true));

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
            SendGameStartEvent(playerOne);
            SendGameStartEvent(playerTwo);
            SendTurnEvent();
            SendCanPlay();
            ResetTimer();
            gameLoop.AddInvokable(new Invokable(() =>
            {
                if (!_timerRunning) return;
                _turnTimeLeft -= gameLoop.DeltaTime;
                if (_turnTimeLeft <= 0)
                {
                    GetPlayerInTurn().CanPlay = false;
                    SwitchTurns(skipStopped: true);
                    GetPlayerInTurn().CanPlay = true;
                    SendGameEventToPlayer(GetPlayerNotInTurn(), new CanPlayEvent { CanPlay = false, SelectedUnitId = GetPlayerInTurn().GetSelectedGoId() });
                    ResetTimer();
                    if (!GetPlayerInTurn().PeerInfo.IsConnected)
                    {
                        PlayBotMove();
                    }
                }
            }));


            gameLoop.AddInvokable(new Invokable(() =>
            {
                if (!_sendPositions) return;

                List<PositionSyncEvent> posEvents = new List<PositionSyncEvent>();
                foreach (var go in gameLoop.activeObjects)
                {
                    Rigidbody goRig = go.GetComponent<Rigidbody>();
                    if (goRig != null && goRig.body.BodyType == BodyType.Dynamic)
                    {
                        var pos = go.Pos;
                        posEvents.Add(new PositionSyncEvent
                        {
                            Id = go.id,
                            PositionX = pos.X.ToShort(),
                            PositionY = pos.Y.ToShort(),
                        });
                    }
                }

                var sections = posEvents.SplitList();

                foreach (var section in sections)
                {
                    GameEvent positionGroupEvent = new PositionGroupSyncEvent
                    {
                        PositionSyncs = section
                    };
                    SendGameEventToPlayers(SendOptions.Sequenced, positionGroupEvent);
                }

            }, Server.UPDATE_TIME));


            if (!GetPlayerInTurn().PeerInfo.IsConnected)
            {
                PlayBotMove();
            }
        }

        private PlayerInfo GetWinnner()
        {
            if (playerOne.AreAllUnitsDead()) return playerTwo;
            if (playerTwo.AreAllUnitsDead()) return playerOne;
            return null;
        }

        private void EndGame()
        {
            var winner = GetWinnner();
            if (winner != null)
            {
                SendGameEventToPlayers(SendOptions.ReliableOrdered, new EndGameEvent { WinnerPlayerType = (byte)winner.PlayerType });
                Server.Get().GameManager.StopMatch(this);
            }
        }

        private void SendGameEventToPlayer(PlayerInfo player, params GameEvent[] gameEvents)
        {
            foreach (var gameEvent in gameEvents)
            {
                gameEvent.TimeStamp = gameLoop.Time;
            }
            NetDataWriter writer = _gameEventSerializer.Serialize(gameEvents);
            if (player.PeerInfo.IsConnected) DataSender.New(player.PeerInfo.NetPeer).Send(writer, SendOptions.ReliableOrdered);
        }

        public void SendGameEventToPlayers(SendOptions sendOption, params GameEvent[] gameEvents)
        {
            foreach (var gameEvent in gameEvents)
            {
                gameEvent.TimeStamp = gameLoop.Time;
            }
            NetDataWriter writer = _gameEventSerializer.Serialize(gameEvents);
            if (playerOne.PeerInfo.IsConnected) DataSender.New(playerOne.PeerInfo.NetPeer).Send(writer, sendOption);
            if (playerTwo.PeerInfo.IsConnected) DataSender.New(playerTwo.PeerInfo.NetPeer).Send(writer, sendOption);
        }

        private void RedirectRequest(SendOptions sendOption, RequestEvent requestEvent, NetPeer peer)
        {
            if (peer == null) return;
            requestEvent.TimeStamp = gameLoop.Time;
            NetDataWriter writer = _requestEventSerializer.Serialize(requestEvent);
            DataSender.New(peer).Send(writer, sendOption);
        }
    }
}