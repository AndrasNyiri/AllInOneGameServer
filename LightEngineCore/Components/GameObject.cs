using System;
using System.Collections.Generic;
using LightEngineCore.Configuration;
using LightEngineCore.Loop;
using LightEngineCore.PhysicsEngine.Collision.ContactSystem;
using LightEngineCore.PhysicsEngine.Dynamics;
using Vector2 = LightEngineCore.PhysicsEngine.Primitives.Vector2;

namespace LightEngineCore.Components
{
    public class GameObject
    {
        public delegate void OnCollidedWithGameObjectDelegate(GameObject go);
        public delegate void OnBelowGroundDelegate();

        public OnCollidedWithGameObjectDelegate onCollidedWithGameObject;

        public GameLoop match;
        public List<Behaviour> components = new List<Behaviour>();

        public string name;
        public ushort id;

        public GameObject(GameLoop match, string name = "", params Behaviour[] initBehaviours)
        {
            this.match = match;
            match.RegisterGameObject(this);
            this.name = name;
            if (string.IsNullOrEmpty(name))
            {
                this.name = GetType().ToString();
            }
            foreach (var component in initBehaviours)
            {
                AddComponent(component);
            }
        }


        public void Assign(GameLoop gl)
        {
            this.match = gl;
        }

        public virtual void Update()
        {
            foreach (var component in components)
            {
                component.Update();
            }
        }

        public GameObject AddComponent(Behaviour behaviour)
        {
            behaviour.Assign(this);
            this.components.Add(behaviour);
            return this;
        }

        public GameObject AddComponent<T>() where T : Behaviour, new()
        {
            T newComponent = new T();
            newComponent.Assign(this);
            this.components.Add(newComponent);
            return this;
        }

        public T GetComponent<T>() where T : Behaviour
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


        public virtual void Destroy()
        {
            match.Destroy(this);
        }

    }
}
