using Repo;
using System.Collections.Generic;

namespace RepoConstraintsCheck
{
    public interface IConstraintsCheckStrategy
    {
        (bool, IEnumerable<(int, IEnumerable<int>)>) Check(IModel model);
    }
}
