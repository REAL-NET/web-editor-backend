using Repo;
using System.Collections.Generic;

namespace RepoConstraintsCheck
{
    public interface IConstraintsCheckStrategy
    {
        bool Check(IModel model);
        (bool, IEnumerable<(int, IEnumerable<int>)>) CheckWithErrorInfo(IModel model);
    }
}
