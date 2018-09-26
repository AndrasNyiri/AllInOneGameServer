namespace LightEngine.Components
{
    public abstract class Component
    {
        public GameObject gameObject;

        public virtual void Update()
        {

        }

        public void Assign(GameObject go)
        {
            this.gameObject = go;
        }
    }
}
