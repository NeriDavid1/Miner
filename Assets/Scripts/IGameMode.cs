using System.Threading.Tasks;

public interface IGameMode
{
    void Initialize();
    Task LoadLevel(int levelIndex);
    void OnGameStart();
    void OnGameEnd(bool success);
    void Cleanup();
}
