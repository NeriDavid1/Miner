using System.Threading.Tasks;

namespace MinerGameMode
{
    public interface IGameMode
    {
        void Initialize();
        Task LoadLevel(int levelIndex);
        void OnGameStart();
        void OnGameEnd(bool success);
        void Cleanup();
    }
}
