using LightEngine.Configuration;
using LightEngine.Loop;
using System;
using System.Collections.Generic;
using Vector2 = LightEngine.PhysicsEngine.Primitives.Vector2;

namespace LightEngine.Components
{
    public class GameObject
    {
        public delegate void OnCollidedWithGameObjectDelegate(GameObject go);
        public delegate void OnBelowGroundDelegate();

        public OnCollidedWithGameObjectDelegate onCollidedWithGameObject;
        public OnBelowGroundDelegate onBelowGround;

        public GameLoop gameLoop;
        public List<Component> components = new List<Component>();

        public string name;
        public ulong id;

        public GameObject(string name = "", params Component[] initComponents)
        {
            this.name = name;
            if (string.IsNullOrEmpty(name))
            {
                this.name = GetType().ToString();
            }
            foreach (var component in initComponents)
            {
                AddComponent(component);
            }
        }

        public void Assign(GameLoop gl)
        {
            this.gameLoop = gl;
        }

        public virtual void Update()
        {
            foreach (var component in components)
            {
                component.Update();
            }
        }

        public GameObject AddComponent(Component component)
        {
            component.Assign(this);
            this.components.Add(component);
            return this;
        }

        public GameObject AddComponent<T>() where T : Component, new()
        {
            T newComponent = new T();
            newComponent.Assign(this);
            this.components.Add(newComponent);
            return this;
        }

        public T GetComponent<T>() where T : Component
        {
            foreach (var x in this.components)
            {
                if (!(x is T)) continue;
                return (T)x;
            }
            return default(T);
        }

        public Vector2 Pos
        {
            get
            {
                var rb = GetComponent<Rigidbody>();
                return rb != null ? rb.body.Position : default(Vector2);
            }
        }

        public float Rot
        {
            get
            {
                var rb = GetComponent<Rigidbody>();
                return rb != null ? rb.body.Rotation : 0f;
            }
        }


        public virtual void Draw()
        {
            try
            {
                Vector2 topLeft = Settings.drawOrigin + new Vector2((float)Math.Floor(Pos.X), (float)Math.Ceiling(Pos.Y));
                Console.SetCursorPosition((int)topLeft.X, (int)topLeft.Y);
                Console.Write("B");
            }
            catch (ArgumentOutOfRangeException)
            {

            }
        }

        public void InvokeCollidedDelegate(GameObject go)
        {
            if (onCollidedWithGameObject != null) onCollidedWithGameObject.Invoke(go);
        }

        public void InvokeOnBelowGroundDelegate()
        {
            if (onBelowGround != null) onBelowGround.Invoke();
        }

        public virtual void Destory()
        {
            gameLoop.Destroy(this);
        }

    }
}
