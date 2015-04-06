namespace DAL.Interfaces
{
    public interface ISceneFactory
    {
        IScene GetScene(string type);
    }
}
