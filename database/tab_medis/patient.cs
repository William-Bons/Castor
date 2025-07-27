using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Castor.database.tab_medis;

/// <summary>
/// Пациент
/// </summary>
public partial class patient
{
    /// <summary>
    /// Пациент
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
    /// Номер компьютерной истории.
    /// сквозная нумерация по всей БД
    /// </summary>
    public long num { get; set; }

    /// <summary>
    /// (не используется) Номер стационарной истории болезни,
    /// если пациент в данный момент лежит на стационарном отделении
    /// </summary>
    public string? stnum { get; set; }

    /// <summary>
    /// Активный (основной) шифр пациента. AGR.TYP
    /// </summary>
    public string? typ { get; set; }

    /// <summary>
    /// Активный (основной) шифр пациента. AGR.KEYID
    /// </summary>
    public long? agrid { get; set; }

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
    /// Пол: 0 - мужской
    /// 1 - женский;
    /// </summary>
    public short sex { get; set; }

    /// <summary>
    /// Документ, удостоверяющий личность (основной). Серия
    /// </summary>
    public string? passser { get; set; }

    /// <summary>
    /// Документ, удостоверяющий личность (основной). Номер
    /// </summary>
    public string? passnumb { get; set; }

    /// <summary>
    /// Документ, удостоверяющий личность (основной). Кем выдан
    /// </summary>
    public string? passissue { get; set; }

    /// <summary>
    /// Документ, удостоверяющий личность (основной). Дата выдачи
    /// </summary>
    public DateTime? passdate { get; set; }

    /// <summary>
    /// (не используется) Проживание в СПб или нет.
    /// 0 - другой город (иногородний); 1 - этот город (местный). заменено на ROOT_REGION_ID;
    /// </summary>
    public long? thistown { get; set; }

    /// <summary>
    /// Адрес ИНФИС/МОКБ. Регистрация. Район (регион)
    /// </summary>
    public long? regid { get; set; }

    /// <summary>
    /// Адрес ИНФИС/МОКБ. Проживание. Район (регион)
    /// </summary>
    public long? reg1id { get; set; }

    /// <summary>
    /// Адрес ИНФИС/МОКБ. Регистрация. Улица
    /// </summary>
    public string? street { get; set; }

    /// <summary>
    /// Адрес ИНФИС/МОКБ. Проживание. Улица
    /// </summary>
    public string? street1 { get; set; }

    /// <summary>
    /// Адрес ИНФИС/МОКБ. Регистрация. Дом
    /// </summary>
    public string? house { get; set; }

    /// <summary>
    /// Адрес ИНФИС/МОКБ. Проживание. Дом
    /// </summary>
    public string? house1 { get; set; }

    /// <summary>
    /// Адрес ИНФИС/МОКБ. Регистрация. Корпус дома
    /// </summary>
    public string? corp { get; set; }

    /// <summary>
    /// Адрес ИНФИС/МОКБ. Проживание. Корпус дома
    /// </summary>
    public string? corp1 { get; set; }

    /// <summary>
    /// Адрес ИНФИС/МОКБ. Регистрация. Квартира
    /// </summary>
    public string? flat { get; set; }

    /// <summary>
    /// Адрес ИНФИС/МОКБ. Проживание. Квартира
    /// </summary>
    public string? flat1 { get; set; }

    /// <summary>
    /// Последнее место работы пациента
    /// </summary>
    public string? lastwrkplace { get; set; }

    /// <summary>
    /// Номер телефона
    /// </summary>
    public string? phone { get; set; }

    /// <summary>
    /// Дата рождения
    /// </summary>
    public DateTime? birthdate { get; set; }

    /// <summary>
    /// Дата выдачи карточки на руки пациенту
    /// </summary>
    public DateTime? carddate { get; set; }

    /// <summary>
    /// Отметка о прикреплении
    /// </summary>
    public long? attachlabel { get; set; }

    /// <summary>
    /// Местоположение карты LU.TAG=32
    /// </summary>
    public long? cardplace { get; set; }

    /// <summary>
    /// E-mail/факс
    /// </summary>
    public string? note { get; set; }

    /// <summary>
    /// (не используется) Данные паспорта
    /// </summary>
    public string? passport { get; set; }

    /// <summary>
    /// (не используется) Свидетельство о рождении
    /// </summary>
    public string? birthpaper { get; set; }

    /// <summary>
    /// (не используется) Мать
    /// </summary>
    public string? mather { get; set; }

    /// <summary>
    /// (не используется) Детский сад
    /// </summary>
    public string? detsad { get; set; }

    /// <summary>
    /// (не используется) Школа
    /// </summary>
    public string? school { get; set; }

    /// <summary>
    /// Направление
    /// </summary>
    public string? direct { get; set; }

    /// <summary>
    /// (не используется) Отец
    /// </summary>
    public string? father { get; set; }

    /// <summary>
    /// Документ, удостоверяющий личность (основной). Вид документа. LU.TAG=4
    /// </summary>
    public long? doc_id { get; set; }

    /// <summary>
    /// Должность
    /// </summary>
    public string? position { get; set; }

    /// <summary>
    /// Адрес ИНФИС/МОКБ. Регистрация. Город
    /// </summary>
    public string? city { get; set; }

    /// <summary>
    /// Адрес ИНФИС/МОКБ. Проживание. Город
    /// </summary>
    public string? city1 { get; set; }

    /// <summary>
    /// (не используется) Проживание в СПб или нет.
    /// 0 - другой город (иногородний); 1 - этот город (местный). заменено на ROOT_REGION1_ID;
    /// </summary>
    public long? thistown1 { get; set; }

    /// <summary>
    /// Социальный статус. LU.TAG=42
    /// </summary>
    public long? social_status_id { get; set; }

    /// <summary>
    /// Сотовый телефон
    /// </summary>
    public string? cellular { get; set; }

    /// <summary>
    /// Служебное поле для идентификации пациента
    /// </summary>
    public string? pin { get; set; }

    /// <summary>
    /// СНИЛС\ИИН
    /// </summary>
    public string? snils { get; set; }

    /// <summary>
    /// Адрес ИНФИС/МОКБ. Регистрация. Регион/страна
    /// </summary>
    public string? region { get; set; }

    /// <summary>
    /// Адрес ИНФИС/МОКБ. Проживание. Регион/страна
    /// </summary>
    public string? region1 { get; set; }

    /// <summary>
    /// Активный (основной) полис пациента
    /// </summary>
    public long? policeid { get; set; }

    /// <summary>
    /// Телефон родственников
    /// </summary>
    public string? relphone { get; set; }

    /// <summary>
    /// Социальный статус. текст
    /// </summary>
    public string? socialstatus { get; set; }

    /// <summary>
    /// Адрес ИНФИС/МОКБ. Регистрация. Регион
    /// </summary>
    public long? region_id { get; set; }

    /// <summary>
    /// Адрес ИНФИС/МОКБ. Проживание. Регион
    /// </summary>
    public long? region1_id { get; set; }

    /// <summary>
    /// Адрес ИНФИС/МОКБ. Регистрация. Тип адреса. LU.TAG=67. сохраняется LU.CODE
    /// </summary>
    public long? root_region_id { get; set; }

    /// <summary>
    /// Адрес ИНФИС/МОКБ. Проживание. Тип адреса. LU.TAG=67. сохраняется LU.CODE
    /// </summary>
    public long? root_region1_id { get; set; }

    /// <summary>
    /// Адрес ИНФИС/МОКБ. Регистрация. Городской/сельский. LU.TAG=102. сохраняется LU.CODE
    /// </summary>
    public long? lives { get; set; }

    /// <summary>
    /// Адрес ИНФИС/МОКБ. Проживание. Городской/сельский. LU.TAG=102. сохраняется LU.CODE
    /// </summary>
    public long? lives1 { get; set; }

    /// <summary>
    /// Адрес ИНФИС/МОКБ. Регистрация. Город
    /// </summary>
    public long? city_id { get; set; }

    /// <summary>
    /// Адрес ИНФИС/МОКБ. Проживание. Город
    /// </summary>
    public long? city1_id { get; set; }

    /// <summary>
    /// Адрес ИНФИС/МОКБ. Регистрация. Улица
    /// </summary>
    public long? street_id { get; set; }

    /// <summary>
    /// Адрес ИНФИС/МОКБ. Проживание. Улица
    /// </summary>
    public long? street1_id { get; set; }

    /// <summary>
    /// (не используется) Свидетельство о рождении. Серия
    /// </summary>
    public string? borndocser { get; set; }

    /// <summary>
    /// (не используется) Свидетельство о рождении. Номер
    /// </summary>
    public string? borndocnum { get; set; }

    /// <summary>
    /// (не используется) Свидетельство о рождении. Кем выдано
    /// </summary>
    public string? borndocissue { get; set; }

    /// <summary>
    /// (не используется) Свидетельство о рождении. Дата выдачи
    /// </summary>
    public DateTime? borndocdat { get; set; }

    /// <summary>
    /// Адрес ИНФИС/МОКБ. Регистрация. Комментарий
    /// </summary>
    public string? addrnote { get; set; }

    /// <summary>
    /// Адрес ИНФИС/МОКБ. Проживание. Комментарий
    /// </summary>
    public string? addrnote1 { get; set; }

    public short? status { get; set; }

    /// <summary>
    /// Адрес ИНФИС/МОКБ. Регистрация. Район
    /// </summary>
    public string? area { get; set; }

    /// <summary>
    /// Адрес ИНФИС/МОКБ. Проживание. Район
    /// </summary>
    public string? area1 { get; set; }

    /// <summary>
    /// Откуда пациент узнал о ЛПУ. LU.TAG=84
    /// </summary>
    public long? knows_from_luid { get; set; }

    /// <summary>
    /// Комментарий
    /// </summary>
    public string? knows_from_note { get; set; }

    /// <summary>
    /// Место рождения
    /// </summary>
    public string? born_place { get; set; }

    /// <summary>
    /// Ссылка на направление
    /// </summary>
    public long? reg_direction_id { get; set; }

    /// <summary>
    /// Категория пациента. LU.TAG=19
    /// </summary>
    public long? categ_lu_id { get; set; }

    /// <summary>
    /// Номер участка LU.TAG=39
    /// </summary>
    public long? areanum_lu_id { get; set; }

    /// <summary>
    /// ПИН по БД терфонда
    /// </summary>
    public string? pin_oms { get; set; }

    /// <summary>
    /// Адрес ИНФИС/МОКБ. Регистрация. Тип улицы
    /// </summary>
    public string? street_type { get; set; }

    /// <summary>
    /// Адрес ИНФИС/МОКБ. Проживание. Тип улицы
    /// </summary>
    public string? street_type1 { get; set; }

    /// <summary>
    /// Коэффициент удаленности
    /// </summary>
    public long? dist_status { get; set; }

    /// <summary>
    /// Внешний регистрационный номер пациента (внешний номер карты пациента), используется для ЛИС &quot;Ариадна&quot;. Лучше использовать PATIENT_EXT.
    /// </summary>
    public string? ext_num { get; set; }

    /// <summary>
    /// Группа крови LU.TAG=191
    /// </summary>
    public long? group_lu_id { get; set; }

    /// <summary>
    /// Резус-фактор LU.TAG=192
    /// </summary>
    public long? rh_lu_id { get; set; }

    /// <summary>
    /// Дата смерти
    /// </summary>
    public DateTime? death_dat { get; set; }

    /// <summary>
    /// Ссылка на актуальную запись в истории личных данных пациента.
    /// </summary>
    public long? pa_history_id { get; set; }

    /// <summary>
    /// Гражданство. LU.TAG=245
    /// </summary>
    public long? citizenship_lu_id { get; set; }

    /// <summary>
    /// Документ, удостоверяющий личность (основной). Код подразделения, выдавшего документ
    /// </summary>
    public string? passdep_code { get; set; }
    public virtual ICollection<visit> Visits { get; set; }
    public virtual ICollection<patserv> Patservs { get; set; }
    public virtual string fullname => Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase($"{lastname} {firstname} {secondname}");
}
