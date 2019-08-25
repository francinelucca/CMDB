using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMDB.DATA{
    public partial class configurationItem
    {
        public List<int> dependenciesList;

        public String toString()
        {
            return this.configurationItemId.ToString() + ". " + this.nombre + " " + this.version + " Estatus: " + ((this.deprecated?? false) ? "Deprecado" : "Activo");
        }
    }
}
