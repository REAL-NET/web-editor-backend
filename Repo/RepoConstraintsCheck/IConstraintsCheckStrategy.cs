using Repo;

namespace RepoConstraintsCheck
{
    public interface IConstraintsCheckStrategy
    {
        bool Check(IModel model);
    }
}
