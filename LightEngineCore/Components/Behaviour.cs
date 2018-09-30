namespace LightEngineCore.Components
{
    public abstract class Behaviour
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
