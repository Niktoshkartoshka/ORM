using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM.Migrations.Stubs
{
    internal class Migration : Model
    {
        public int Id;
        public string NameMigration;
        public int Batch;
    }
}
