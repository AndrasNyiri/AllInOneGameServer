using System.Collections.Generic;
using System.Threading;
using LightEngine.Configuration;

namespace LightEngine.Loop
{
    public class LoopManager
    {
        private readonly List<List<GameLoop>> _loopList = new List<List<GameLoop>>();
        private ulong _loopIdCounter = 1;


        public LoopManager()
        {
            for (int i = 0; i < Settings.SIMULATION_THREADS; i++)
            {
                _loopList.Add(new List<GameLoop>());
                ThreadPool.QueueUserWorkItem(UpdateLoop, i);
            }
        }

        private void UpdateLoop(object context)
        {
            int index = (int)context;
            var myList = _loopList[index];

            while (true)
            {
                lock (myList)
                {
                    for (var i = myList.Count - 1; i >= 0; i--)
                    {
                        myList[i].Update();
                    }
                }

                Thread.Sleep(1);
            }
        }

        public void AddLoop(GameLoop gameLoop)
        {
            gameLoop.gameLoopId = _loopIdCounter++;
            int threadIndex = GetLeastBusyThreadIndex();
            lock (_loopList[threadIndex])
            {
                _loopList[threadIndex].Add(gameLoop);
            }
        }

        public GameLoop StartLoop()
        {
            int threadIndex = GetLeastBusyThreadIndex();
            GameLoop newLoop = new GameLoop();
            newLoop.gameLoopId = _loopIdCounter++;
            lock (_loopList[threadIndex])
            {
                _loopList[threadIndex].Add(newLoop);
            }

            return newLoop;
        }

        private int GetLeastBusyThreadIndex()
        {
            int minValue = int.MaxValue;
            int index = 0;
            for (var i = 0; i < _loopList.Count; i++)
            {
                lock (_loopList[i])
                {
                    if (_loopList[i].Count < minValue)
                    {
                        minValue = _loopList[i].Count;
                        index = i;
                    }
                }
            }

            return index;
        }


        public GameLoop GetLoop(ulong gameloopId)
        {
            foreach (var loops in _loopList)
            {
                lock (loops)
                {
                    var loop = loops.Find(
                        g =>
                        g.gameLoopId == gameloopId);
                    if (loop != null)
                    {
                        return loop;
                    }
                }

            }
            return null;
        }

    }
}
