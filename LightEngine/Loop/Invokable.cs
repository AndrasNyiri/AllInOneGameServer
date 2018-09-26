using System;

namespace LightEngine.Loop
{
    public class Invokable
    {
        private GameLoop _gameLoop;
        private long _deltaSum;
        private readonly Action _action;
        private readonly bool _deleteOnInvoke;
        private float _interval;
        private bool _interrupted;

        public Invokable(Action action, float interval = 1, bool deleteOnInvoke = false)
        {
            _action = action;
            _deleteOnInvoke = deleteOnInvoke;
            _interval = interval;
        }

        public void SetGameLoop(GameLoop gameLoop)
        {
            _gameLoop = gameLoop;
        }

        private void TryToInvoke()
        {
            if (_deltaSum < _interval) return;
            if (_interrupted)
            {
                _gameLoop.RemoveInvokeable(this);
                return;
            }
            _action();
            _deltaSum = 0;
            if (_deleteOnInvoke)
            {
                _gameLoop.RemoveInvokeable(this);
            }
        }

        public void Update(long deltaTime)
        {
            _deltaSum += deltaTime;
            TryToInvoke();
        }

        public void Interrupt()
        {
            _interrupted = true;
        }

        public void AddToInterval(float amount)
        {
            _interval += amount;
        }
    }
}
