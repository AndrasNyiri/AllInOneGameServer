using System.Collections.Generic;
using System.Threading;
using LightEngine.Configuration;

namespace LightEngine.Loop
{
    public class LoopManager
    {
        private readonly List<List<GameLoop>> _loopList = new List<List<GameLoop>>();


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
            int threadIndex = GetLeastBusyThreadIndex();
            gameLoop.threadIndex = threadIndex;
            lock (_loopList[threadIndex])
            {
                _loopList[threadIndex].Add(gameLoop);
            }
        }

        public GameLoop StartLoop()
        {
            int threadIndex = GetLeastBusyThreadIndex();
            GameLoop newLoop = new GameLoop();
            newLoop.threadIndex = threadIndex;
            lock (_loopList[threadIndex])
            {
                _loopList[threadIndex].Add(newLoop);
            }
            return newLoop;
        }


        public void StopLoop(GameLoop gameLoop)
        {
            if (gameLoop == null) return;
            lock (_loopList[gameLoop.threadIndex])
            {
                _loopList[gameLoop.threadIndex].Remove(gameLoop);
            }
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

    }
}
