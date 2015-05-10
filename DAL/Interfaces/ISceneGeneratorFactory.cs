namespace DAL.Interfaces
{
    public interface ISceneGeneratorFactory
    {
        ISceneGenerator GetScene(string type);
    }
}
