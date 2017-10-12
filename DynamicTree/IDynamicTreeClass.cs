using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainProject {
    public interface IDynamicTreeClass {

        string Uid { get; }

        string Header { get; }

        bool HasChilds { get; }

        List<IDynamicTreeClass> GetChilds (string parentId);

    }
}
