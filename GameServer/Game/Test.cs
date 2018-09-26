using System;
using AllInOneGameServer.Game.Components.Scripts;
using LightEngine.Components;
using LightEngine.Configuration;
using LightEngine.Loop;
using LightEngine.PhysicsEngine.Dynamics;
using LightEngine.PhysicsEngine.Primitives;
namespace AllInOneGameServer.Game
{
    class Test
    {
        static void Main(string[] args)
        {
            Settings.targetFrameRate = 500;
            Settings.drawOrigin = new Vector2(50, 10);
            LoopManager loopManager = new LoopManager();
            GameLoop gameLoop = new GameLoop();
            GameObject box = new GameObject("My Box", new Rigidbody(gameLoop.physicsWorld, 2f, 1f, 1f, Vector2.Zero, BodyType.Dynamic));
            GameObject ground = new GameObject("Ground", new Rigidbody(gameLoop.physicsWorld, 10f, 2f, 1f, new Vector2(0.0f, 5f), BodyType.Static));
            gameLoop.RegisterGameObject(box);
            gameLoop.RegisterGameObject(ground);
            gameLoop.shouldDrawToConsole = true;
            loopManager.AddLoop(gameLoop);

            while (true)
            {
                var keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.Spacebar:
                        box.GetComponent<Rigidbody>().body.ApplyLinearImpulse(new Vector2(0, -15));
                        break;
                    case ConsoleKey.A:
                        box.GetComponent<Rigidbody>().body.ApplyLinearImpulse(new Vector2(-15, 0f));
                        break;
                    case ConsoleKey.D:
                        box.GetComponent<Rigidbody>().body.ApplyLinearImpulse(new Vector2(15, 0f));
                        break;

                }
            }


        }
    }
}
