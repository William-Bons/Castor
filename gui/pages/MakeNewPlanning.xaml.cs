using Castor.database;
using Castor.database.tab_medis;
using Castor.database.tables;
using Castor.gui.common;
using System.Windows;

namespace Castor.gui.pages
{
    /// <summary>
    /// Логика взаимодействия для MakeNewPlanning.xaml
    /// </summary>
    public partial class MakeNewPlanning : Window, IDialog
    {

        public MakeNewPlanning(CastorCommonContext castorCommonContext, object _visit)
        {
            Visit = (visit?)_visit;
            InitializeComponent();
            DataContext = this;

            Planns = castorCommonContext.DictPlannings.ToList();

            Planning = new Planning();
            Planning.patientid = Visit.patientid.Value;
            Planning.docdepid = Visit.doctorid.Value;
            Planning.depid = Visit.depid.Value;
            Planning.visitid = Visit.keyid;
            Planning.start_date = Visit.dat.Value;
        }

        public visit? Visit { get; set; }
        public ICollection<DictPlannings>? Planns { get; set; }
        public Planning Planning { get; set; }
    }
}
