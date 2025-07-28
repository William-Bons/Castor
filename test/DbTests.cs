using Castor.database;
using Castor.database.tab_medis;
using Castor.gui.common;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Castor.test
{

    public class DbTests : IConsoleMessage, IRun
    {
        private object TestNo = null;
        private CastorContext Db;
        public event ConsoleMessageHandler ConsoleMessage;


        public DbTests(CastorContext db, object testNo)
        {
            Db = db;
            TestNo = testNo;
        }

        public void Run()
        {
            MethodInfo methodInfo = typeof(DbTests).GetMethod(TestNo.ToString());
            methodInfo?.Invoke(this, null);
        }


        public async Task TTestSelectTable1()
        {
            ConsoleMessage?.Invoke("running Test selectTable");
            Func<Task> asyncLambda = async () =>
            {
                var plann = await Db.DictPlannings.ToListAsync();
                ConsoleMessage?.Invoke($"Gets {plann.Count} plannings");
                foreach (var user in plann)
                    ConsoleMessage?.Invoke($"{user.keyid}\t\t => {user.description}");
            };
            await asyncLambda();
        }

        public async Task TTestSelectTable2()
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

        public async Task TTestSelectTable3()
        {
            await Task.Run(() =>
            {
                using (MedisContext cc = new MedisContext())
                {
                    //var req = cc.patient
                    //    .Where(pat => pat.keyid == 2315389)
                    //    .Include(pat => pat.Visits)
                    //    .ThenInclude(vis => vis.Dep)
                    //    .ToList();
                    //;

                    var qwe = cc.dep
                        .Where(d => d.keyid == 2704)
                        .Include(d => d.Visits.Where(v => !v.dat1.HasValue))
                        .ThenInclude(v => v.Patient)
                        .ToList();
                    ;
                }
            });
        }


    }
}
