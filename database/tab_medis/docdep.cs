using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Castor.database.tab_medis;

/// <summary>
/// Сотрудник на отделении
/// </summary>
public partial class docdep
{
    /// <summary>
    /// Сотрудник на отделении
    /// </summary>
    [Key] public long keyid { get; set; }

    /// <summary>
    /// Кто создал
    /// </summary>
    public long? createby { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime? createdate { get; set; }

    /// <summary>
    /// Кто последний изменил
    /// </summary>
    public long? updateby { get; set; }

    /// <summary>
    /// Дата последнего изменения
    /// </summary>
    public DateTime? updatedate { get; set; }

    /// <summary>
    /// Категория (LU.TAG=26)
    /// </summary>
    public long? ucategid { get; set; }

    /// <summary>
    /// Специальность (LU.TAG=9)
    /// </summary>
    public long? specid { get; set; }

    /// <summary>
    /// Сотрудник
    /// </summary>
    public long docid { get; set; }

    /// <summary>
    /// Отделение
    /// </summary>
    public long depid { get; set; }

    /// <summary>
    /// Должность
    /// </summary>
    public long? positionid { get; set; }

    /// <summary>
    /// Специализация
    /// </summary>
    public long? specialid { get; set; }

    /// <summary>
    /// Статус записи:
    /// 0 - архивный
    /// 1 - активный;
    /// </summary>
    public short status { get; set; }

    /// <summary>
    /// Код сотрудника на отделении
    /// </summary>
    public string? code { get; set; }

    /// <summary>
    /// Отделение, ФИО сотрудника. По умолчанию дублирует ФИО с doctor, но может быть вручную изменено.
    /// </summary>
    public string? text { get; set; }

    /// <summary>
    /// Номер кабинета (при возможности синхронен с ROOM_ID)
    /// </summary>
    public int? num { get; set; }

    /// <summary>
    /// Дата начала работы на отделении
    /// </summary>
    public DateTime? bgndat { get; set; }

    /// <summary>
    /// Дата окончания работы на отделении
    /// </summary>
    public DateTime? enddat { get; set; }

    /// <summary>
    /// Замечания
    /// </summary>
    public string? note { get; set; }

    /// <summary>
    /// (не используется) Исследование (LU.TAG=28)
    /// </summary>
    public long? examid { get; set; }

    /// <summary>
    /// Статус фиктивности: 0 - реальный сотрудник; 1 - фиктивный;
    /// </summary>
    public short? fict_status { get; set; }

    /// <summary>
    /// Тип начисления зарплаты для сотрудника (LU.TAG=303)
    /// </summary>
    public long? salary_typ_id { get; set; }

    /// <summary>
    /// Тип персонала (LU.TAG=311)
    /// </summary>
    public long? staff_typ_id { get; set; }

    /// <summary>
    /// Номер участка (LU.TAG=39)
    /// </summary>
    public long? areanumid { get; set; }

    /// <summary>
    /// Кол-во доп. номерков, разрешенных для выдачи другими врачами
    /// </summary>
    public long? add_numb_qty { get; set; }

    /// <summary>
    /// Тип врача (LU.TAG=386)
    /// </summary>
    public long? doc_type_id { get; set; }

    /// <summary>
    /// Тип  персонала (LU.TAG=21)
    /// </summary>
    public long? staff_docdep_id { get; set; }

    /// <summary>
    /// Идентификатор кабинета (ROOM.KEYID)
    /// </summary>
    public long? room_id { get; set; }

    /// <summary>
    /// Предыдущее значение статуса. Необходимо для восстановления правильного статуса у записи, при изменении статуса у сотрудника.
    /// </summary>
    public short? prev_status { get; set; }

    /// <summary>
    /// Ограничения на обслуживание пациентов. Пол: NULL - для всех (значение по умолчанию); 0 - мужской; 1 - женский;
    /// </summary>
    public short? pat_sex { get; set; }

    /// <summary>
    /// Ограничения на обслуживание пациентов. Минимальный возраст, полных лет.
    /// </summary>
    public short? pat_age_from { get; set; }

    /// <summary>
    /// Ограничения на обслуживание пациентов. Максимальный возраст, полных лет.
    /// </summary>
    public short? pat_age_to { get; set; }

    public virtual ICollection<visit>? Visits { get; set; }

}
