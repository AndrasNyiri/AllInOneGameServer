using LightEngine.Components;
using LightEngine.Configuration;
using System;
using System.Collections.Generic;
using Vector2 = LightEngine.PhysicsEngine.Primitives.Vector2;
using World = LightEngine.PhysicsEngine.Dynamics.World;

namespace LightEngine.Loop
{
    public class GameLoop
    {
        public ulong gameLoopId;

        public bool shouldDrawToConsole = false;
        public long time;
        public readonly List<GameObject> activeObjects = new List<GameObject>();

        public readonly World physicsWorld;

        private long _deltaTime;
        private readonly long _startTime;

        private readonly List<Invokable> _invokables = new List<Invokable>();
        private readonly List<GameObject> _gameObjectsToRemove = new List<GameObject>();

        private static readonly float MillisPerSecond = 1000f / Settings.targetFrameRate;

        public float DeltaTime => _deltaTime / 1000f;
        public float Time => time / 1000f;

        private int _frames;
        private float _lastTime;

        public int Fps { get; private set; }

        private ulong _goIdCounter = 1;


        public GameLoop()
        {
            _startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            physicsWorld = new World(new Vector2(0.0f, Settings.gravity));
        }

        private void CalculateTime()
        {
            long nowMilis = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            long newTime = nowMilis - _startTime;
            _deltaTime = newTime - time;
            time = newTime;
        }


        public void Update()
        {
            if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - _startTime - time < Math.Floor(MillisPerSecond)) return;
            CalculateTime();

            if (_lastTime < Time)
            {
                _lastTime = Time + 1;
                Fps = _frames;
                _frames = 0;
            }
            else
            {
                _frames++;
            }

            physicsWorld.Step(DeltaTime);
            if (shouldDrawToConsole)
            {
                Console.Clear();

                foreach (var gameObject in activeObjects)
                {
                    gameObject.Draw();
                }
            }


            for (int i = _invokables.Count - 1; i > -1; i--)
            {
                _invokables[i].Update(_deltaTime);
            }

            foreach (var gameObject in activeObjects)
            {
                gameObject.Update();
            }


            if (_gameObjectsToRemove.Count > 0)
            {
                foreach (var gameObject in _gameObjectsToRemove)
                {
                    activeObjects.Remove(gameObject);
                }
                _gameObjectsToRemove.Clear();
            }
        }


        public void RegisterGameObject(GameObject go)
        {
            go.id = _goIdCounter++;
            go.Assign(this);
            activeObjects.Add(go);
        }

        public void Destroy(GameObject go)
        {
            Rigidbody rb = go.GetComponent<Rigidbody>();
            if (rb != null) physicsWorld.RemoveBody(rb.body);
            _gameObjectsToRemove.Add(go);
        }

        public Invokable AddInvokable(Invokable invokable)
        {
            _invokables.Add(invokable);
            invokable.SetGameLoop(this);
            return invokable;
        }

        public void RemoveInvokeable(Invokable invokable)
        {
            _invokables.Remove(invokable);
        }


    }
}
