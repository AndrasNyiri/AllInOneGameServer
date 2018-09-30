using System;
using LightEngineCore.Components;
using LightEngineCore.Configuration;
using LightEngineCore.Loop;
using LightEngineCore.PhysicsEngine.Dynamics;
using LightEngineCore.PhysicsEngine.Primitives;
using Contact = LightEngineCore.PhysicsEngine.Collision.ContactSystem.Contact;

namespace LightGameServer.Game
{
    class Test
    {
        static void Main(string[] args)
        {
            Settings.gravity = 0;
            Settings.targetFrameRate = 500;
            Settings.drawOrigin = new Vector2(50, 10);
            LoopManager loopManager = new LoopManager(1);
            GameLoop gameLoop = new GameLoop();
            GameObject box = new GameObject("My Box", new Rigidbody(gameLoop.physicsWorld, 2f, 1f, 1f, Vector2.Zero, BodyType.Dynamic));
            GameObject ground = new GameObject("Ground", new Rigidbody(gameLoop.physicsWorld, 10f, 2f, 1f, new Vector2(0.0f, 5f), BodyType.Static));
            gameLoop.RegisterGameObject(box);
            gameLoop.RegisterGameObject(ground);
            gameLoop.shouldDrawToConsole = true;

            box.GetComponent<Rigidbody>().body.OnCollision += OnBoxCollision;



            loopManager.AddLoop(gameLoop);
            while (true)
            {
                var keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.W:
                        box.GetComponent<Rigidbody>().body.ApplyLinearImpulse(new Vector2(0, -15));
                        break;
                    case ConsoleKey.S:
                        box.GetComponent<Rigidbody>().body.ApplyLinearImpulse(new Vector2(0, 15));
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

        private static void OnBoxCollision(Fixture self, Fixture hitFixture, Contact contact)
        {
            var normal = contact.Manifold.LocalNormal;
            normal.Y *= -1;
            self.Body.ApplyLinearImpulse(normal * 30f);
        }
    }
}
