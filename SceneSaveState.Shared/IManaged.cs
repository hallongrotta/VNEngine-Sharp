using System;
using System.Collections.Generic;
using System.Text;

namespace SceneSaveState
{
    public interface IManaged<out T>
    {
        string Name { get; set; }
        string TypeName { get;}
        T Copy();
    }
}
