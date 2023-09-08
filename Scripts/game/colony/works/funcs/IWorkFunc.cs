using loader.database;
namespace game.colony.works.funcs
{
    public interface IWorkFunc
    {
        int Uid { get; }
        SkillId SkillId { get; }
        bool CanAssign { get; }
        bool IsAssigned { get; }
        float StateProgress { get; }
        bool IsCompleted { get; }
        bool IsCanceled { get; }

        void DoUpdate(float dt);

        void TryAssign();

        void AddPers(WorkerView worker);

        void UnAssign();

        void DoCancel();
    }
}
