using Castor.database;
using Castor.database.tab_medis;
using Castor.gui.common;
using Castor.gui.dialogs;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Reflection.Metadata;

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
                    ICollection<dep> deps = cc.dep
                    .Where(d => d.keyid == SelectUser.SelectedDep.keyid)
                    .Include(d => d.Visits.Where(v => !v.dat1.HasValue))
                    .ThenInclude(v => v.Patient)
                    .ThenInclude(p => p.Diagnoses)
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

        public void TCreateHtml()
        {
            var html =
                @"<body>
                    <h1>This is <b>bold</b> heading</h1>
                    <P>This is <u>underlined</u> paragraph</P>
                </body>";

            var htmlDoc = new HtmlDocument();
            htmlDoc.OptionOutputOriginalCase = true;
            htmlDoc.OptionDefaultUseOriginalName = true;
            htmlDoc.OptionPreserveXmlNamespaces = true;
            htmlDoc.OptionOutputAsXml = true;
            htmlDoc.OptionOutputUpperCase = true;


            htmlDoc.LoadHtml(html);

            var htmlBody = htmlDoc.DocumentNode.SelectSingleNode("//body");
            HtmlNode newPara = HtmlNode.CreateNode("<P>This a new paragraph cap</P>");
            htmlBody.ChildNodes.Add(newPara);
            ;
        }

    }
}
