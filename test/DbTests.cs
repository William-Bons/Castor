using Castor.database;
using Castor.database.tab_medis;
using Castor.database.tables;
using Castor.gui.common;
using Castor.gui.dialogs;
using Castor.Properties;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace Castor.test
{

    public class DbTests : IConsoleMessage, IRun
    {
        private object TestNo = null;
        public event ConsoleMessageHandler ConsoleMessage;


        public DbTests(object testNo)
        {
            TestNo = testNo;
        }

        public void Run()
        {
            MethodInfo methodInfo = typeof(DbTests).GetMethod(TestNo.ToString());
            methodInfo?.Invoke(this, null);
        }


        public async Task TTestSelectTable1()
        {
            Func<Task> asyncLambda = async () =>
            {
                IEnumerable<Movebook> movebooks;
                IEnumerable<visit> visits;
                IEnumerable<string> fios;

                using (CastorContext castor = new CastorContext())
                {
                    // select Fio only from movebooks
                    movebooks = castor.Movebooks
                        .Where(m => m.Visitid == null);

                    fios = movebooks.Select(m => m.Fio);

                    using (MedisContext medis = new MedisContext())
                    {
                        // select all visits in DEPARTMENT (dep)
                        visits = medis.dep
                            .Where(d => d.keyid == Settings.Default.LastSelectedDep)
                            .Include(d => d.Visits)
                            .ThenInclude(v => v.Patient)
                            .Select(d => d.Visits).First();

                        // select those visits where fio match movebook and date out empty
                        IEnumerable<visit> vv = visits
                            .Where(m => fios.Contains(m.Patient?.fullname) && m.dat1==null);

                        foreach (var mb in movebooks)
                        {
                            var q = vv.Where(v => v.Patient.fullname == mb.Fio);
                            mb.Visitid = vv.Where(v => v.Patient.fullname == mb.Fio).First().keyid;
                        }

                        int t = 0;
                    }
                }
            };
            await asyncLambda();
        }

        public async Task TTestSelectTable2()
        {
            string[] ds = { "F20", "F21","F32.2","F25","F43.2","F01.0","G20","F70.0","F23.1","20.0","21"  };
            foreach (string s in ds)
            {
                ConsoleMessage?.Invoke($"{s} == {Regex.IsMatch(s, @"^F(21|01|22|23|25|30|31|32)")}");
            }

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
