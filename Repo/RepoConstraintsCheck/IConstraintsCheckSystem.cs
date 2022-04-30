using System.Collections.Generic;

namespace RepoConstraintsCheck
{
    public interface IConstraintsCheckSystem
    {
        (bool, IEnumerable<(int, IEnumerable<int>)>) Check();
    }
}
