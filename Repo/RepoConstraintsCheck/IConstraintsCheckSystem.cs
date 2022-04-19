using System.Collections.Generic;

namespace RepoConstraintsCheck
{
    public interface IConstraintsCheckSystem
    {
        bool Check();
        (bool, IEnumerable<(int, IEnumerable<int>)>) CheckWithErrorInfo();
    }
}
