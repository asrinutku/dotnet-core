using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace afad
{
    public class NetcadData

    {
        public string Name { get; set; }
        public List<NetcadDataRowCollection> Rows = new List<NetcadDataRowCollection>();
    }

    public class NetcadDataRowCollection : List<NetcadDataRow>
    {
    }

    public class NetcadDataRow
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public bool IsGeometry { get; set; }
    }
}
