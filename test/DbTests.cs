using Castor.database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castor.test
{
    public class DbTests
    {
        private CastorCommonContext Db;
        public DbTests(CastorCommonContext db, object testNo) 
        {
            Db=db;
            TTestSelectTable();
        }

        private async Task TTestSelectTable()
        {
            var users = await Db.Users.ToListAsync();
            ;
        }
    }
}
