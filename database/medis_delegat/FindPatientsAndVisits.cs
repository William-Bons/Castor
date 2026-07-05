using Castor.database.tab_medis;
using Castor.gui.common;
using Castor.gui.dialogs;
using Castor.gui.find;
using Castor.Properties;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castor.database.medis_delegat
{
    public class FindPatientsAndVisits
    {

        /// <summary>
        /// поиск всех визитов для patient. Если задан параметр Department - фильтрует по выбранному отделению
        /// </summary>
        public IEnumerable<visit> FindVisitsForPatient(patient Patient, dep? Department)
        {
            IEnumerable<visit> _Visits = new List<visit>();

            try
            {
                using MedisContext medis = new MedisContext();
                _Visits = medis.visit
                    .Where(v => v.patientid == Patient.keyid)
                    .Include(v => v.Dep)
                    .ThenInclude(r => r.Root).ThenInclude(t => t.Root)
                    .Include(v => v.Patient)
                    .ThenInclude(p => p.Diagnoses.Where(g => g.Diagnos.code.StartsWith("F")))
                    .ThenInclude(d => d.Diagnos)
                    .ToList();

                if (Department != null)
                {
                    _Visits = _Visits
                        .Where(v => v.depid == Department.keyid)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Message.ShowPopup(ex.Message);
            }

            return _Visits;
        }

        /// <summary>
        /// поиск пациента в медис по данным указанным в структуре Patientdata. Если заполнена только фамилия, поиск не проводится
        /// возвращает List of patient
        /// </summary>
        public IEnumerable<patient> FindPatient(Patientdata Patientdata)
        {
            IEnumerable<patient> _Patients = new List<patient>();
            try
            {
                using MedisContext medis = new MedisContext();

                // search on SNILS
                if (!string.IsNullOrWhiteSpace(Patientdata.SNILS))
                {
                    _Patients = medis.patient
                        .Where(p => p.snils == Patientdata.SNILS)
                        .Take(20)
                        .ToList();
                }
                // search to 3 names
                else if(!string.IsNullOrWhiteSpace(Patientdata.Lastname))
                {
                    _Patients = medis.patient
                        .Where(p =>
                            p.lastname.Contains(Patientdata.Family.ToLower())
                            && p.firstname.Contains(Patientdata.Firstname.ToLower())
                            && p.secondname.Contains(Patientdata.Lastname.ToLower())
                    ).Take(20)
                    .ToList();
                }
                // search to 2 names: family name and firstname
                else if(!string.IsNullOrWhiteSpace(Patientdata.Firstname))
                {
                    _Patients = medis.patient
                        .Where(p =>
                            p.lastname.Contains(Patientdata.Family.ToLower())
                            && p.firstname.Contains(Patientdata.Firstname.ToLower())
                    ).Take(20)
                    .ToList();
                }
                else
                {
                    Message.ShowPopup("Только фамилии недостаточно для поиска");
                }

                
            }
            catch (Exception ex)
            {
                Message.ShowPopup(ex.Message);
            }

            return _Patients;
        }

    }
}
