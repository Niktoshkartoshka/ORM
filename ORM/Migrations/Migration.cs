using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM.Migrations
{
    public class Migration
    {
        protected virtual string Connection { get; set; }

        protected Schema Schema;

        public void Init()
        {
            Connection ??= NktOrm.GetDefaultConnectionName();
            Schema = new(Connection);
        }
    }
}
