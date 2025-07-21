using Castor.database;
using Castor.database.tab_medis;
using Castor.gui.common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castor.test
{
    public class DbTests : IConsoleMessage
    {
        private CastorCommonContext Db;
        public event ConsoleMessageHandler ConsoleMessage;

        public DbTests(CastorCommonContext db, object testNo) 
        {
            Db=db;
            switch (testNo)
            {
                case 1:
                    _ = TTestSelectTable1();
                    break;
                case 2:
                    _ = TTestSelectTable2();
                    break;
                case 3:
                    _ = TTestSelectTable3();
                    break;
            }
        }

        

        private async Task TTestSelectTable1()
        {
            var users = await Db.Users.ToListAsync();
            ;
        }

        private async Task TTestSelectTable2()
        {
            await Task.Run(() =>
            {
                using (MedisContext cc = new MedisContext())
                {
                    var ptn = cc.patient
                    .Where(p => p.keyid == 2315389)
                    .ToList();

                    var patsr = cc.patserv
                        .Where(b => b.patientid == 2315389)
                        .ToList();
                    ;
                }
            });
        }

        private async Task TTestSelectTable3()
        {
            await Task.Run(() =>
            {
                using(MedisContext cc = new MedisContext())
                {
                    //var req = cc.patient
                    //    .Where(pat => pat.keyid == 2315389)
                    //    .Include(pat => pat.Visits)
                    //    .ThenInclude(vis => vis.Dep)
                    //    .ToList();
                    //;

                    var qwe = cc.dep
                        .Where(d => d.keyid==2704)
                        .Include(d => d.Visits.Where(v => !v.dat1.HasValue))
                        .ThenInclude(v => v.Patient)
                        .ToList();
                    ;
                }
            });
        }
    }
}
