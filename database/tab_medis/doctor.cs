using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Castor.database.tab_medis;

/// <summary>
/// Класс таблицы базы данных Медис
/// 
/// Сотрудник
/// </summary>
public partial class doctor
{
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
    /// Категория сотрудника на основном отделении (LU.TAG=26)
    /// </summary>
    public long? ucategid { get; set; }

    /// <summary>
    /// Специальность сотрудника на основном отделении (LU.TAG=9)
    /// </summary>
    public long? specid { get; set; }

    /// <summary>
    /// Отделение, на котором числится сотрудник. Если сотрудник работает на нескольких отделениях, то это отделение его основного места работы.
    /// </summary>
    public long? depid { get; set; }

    /// <summary>
    /// Должность (LU.TAG=22)
    /// </summary>
    public long? positionid { get; set; }

    /// <summary>
    /// Тип  персонала (LU.TAG=21)
    /// Врач/Средний мед. персонал/Младший мед. персонал/...
    /// </summary>
    public long staffid { get; set; }

    /// <summary>
    /// Статус сотрудника:
    /// 0 - архивный
    /// 1 - активный;
    /// </summary>
    public short status { get; set; }

    /// <summary>
    /// Код сотрудника
    /// </summary>
    public string? code { get; set; }

    /// <summary>
    /// ФИО. По умолчанию формируется из полей Фамилия Имя Отчество
    /// </summary>
    public string? text { get; set; }

    /// <summary>
    /// (не используется)
    /// </summary>
    public int? num { get; set; }

    /// <summary>
    /// Дата начала работы в учреждении
    /// </summary>
    public DateTime? bgndat { get; set; }

    /// <summary>
    /// Дата окончания работы в учреждении
    /// </summary>
    public DateTime? enddat { get; set; }

    /// <summary>
    /// Замечания
    /// </summary>
    public string? note { get; set; }

    /// <summary>
    /// (не используется)
    /// </summary>
    public long? spec_d { get; set; }

    /// <summary>
    /// (не используется) сертификат
    /// </summary>
    public string? sertificate { get; set; }

    /// <summary>
    /// Степень
    /// </summary>
    public string? degree { get; set; }

    /// <summary>
    /// Дата рождения
    /// </summary>
    public DateTime? birthdate { get; set; }

    /// <summary>
    /// Табельный номер
    /// </summary>
    public string? tabnum { get; set; }

    /// <summary>
    /// Фамилия
    /// </summary>
    public string? lastname { get; set; }

    /// <summary>
    /// Имя
    /// </summary>
    public string? firstname { get; set; }

    /// <summary>
    /// Отчество
    /// </summary>
    public string? secondname { get; set; }

    /// <summary>
    /// Контакты. Контактный телефон
    /// </summary>
    public string? phone { get; set; }

    /// <summary>
    /// Ссылка на пользователя системы (привязка персонала к пользователям системы)
    /// </summary>
    public long? man_id { get; set; }

    /// <summary>
    /// Контакты. Рабочий телефон
    /// </summary>
    public string? workphone { get; set; }

    /// <summary>
    /// Контакты. Мобильный телефон
    /// </summary>
    public string? cellular { get; set; }

    /// <summary>
    /// Контакты. Адрес
    /// </summary>
    public string? address { get; set; }

    /// <summary>
    /// Контакты. Адрес электронной почты
    /// </summary>
    public string? email { get; set; }

    /// <summary>
    /// СНИЛС. Страховой номер индивидуального лицевого счёта
    /// </summary>
    public string? snils { get; set; }

    /// <summary>
    /// (не используется) Дата окончания действия сертификата
    /// </summary>
    public DateTime? sertificate_enddat { get; set; }

    /// <summary>
    /// Пол (мужской/женский)
    /// 0 - мужской; 1 - женский; NULL - без указания;
    /// </summary>
    public short? sex { get; set; }

    /// <summary>
    /// ИНН. Индивидуальный номер налогоплательщика. Используетя для кассиров.
    /// </summary>
    public string? inn { get; set; }

    /// <summary>
    /// Пациент. Если сотрудник обслуживается в ЛПУ
    /// </summary>
    public long? patient_id { get; set; }

    /// <summary>
    /// Федеральный реестр медицинских работников. Основная карта. INETUSER.FRMR_GENERAL.ID
    /// </summary>
    public long? frmr_id { get; set; }

    /// <summary>
    /// Медицинская организация, которой принадлежит запись
    /// </summary>
    public long? mo_id { get; set; }

    public virtual ICollection<visit>? Visits { get; set; } 
}
