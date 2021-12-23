using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace afad
{
    public abstract class IDataSource

    {
        public abstract string Name { get; }

        public abstract NetcadData ReadData();

    }
}
