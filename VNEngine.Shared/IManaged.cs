using System;
using System.Collections.Generic;
using System.Text;

namespace VNEngine
{
    public interface IManaged<out T>
    {
        string Name { get; set; }
        string TypeName { get;}
        T Copy();
    }
}
