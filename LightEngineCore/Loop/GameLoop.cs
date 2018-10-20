using System;
using System.Collections.Generic;
using LightEngineCore.Components;
using LightEngineCore.Configuration;
using LightEngineCore.PhysicsEngine.Dynamics;
using Vector2 = LightEngineCore.PhysicsEngine.Primitives.Vector2;
using World = LightEngineCore.PhysicsEngine.Dynamics.World;

namespace LightEngineCore.Loop
{
    public class GameLoop
    {
        public int threadIndex;

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

        private ushort _goIdCounter = 1;


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

            physicsWorld.Step(DeltaTime * Settings.timeScale);
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

        public GameObject GetGameObjectByBody(Body body)
        {
            foreach (var go in activeObjects)
            {
                Rigidbody rb = go.GetComponent<Rigidbody>();
                if (rb != null && rb.body == body) return go;
            }

            return null;
        }

        public List<T> GetInOverlapCircle<T>(Vector2 pos, float radius) where T : GameObject
        {
            List<T> affectedGameObjects = new List<T>();
            foreach (var go in activeObjects)
            {
                if (go is T && Vector2.Distance(go.Pos, pos) <= radius)
                {
                    affectedGameObjects.Add((T)go);
                }
            }
            return affectedGameObjects;
        }

    }
}
