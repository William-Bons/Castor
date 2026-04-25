using Castor.database.tables;
using Castor.gui.common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Castor.database.tab_medis;

/// <summary>
/// Класс таблицы базы данных Медис
/// 
/// Справочник подразделений
/// 1480 - ДПБ
/// 1482 - стационарные подразделения ДПБ
/// 2742 - 6 отд
/// 2709 - 12 отд
/// </summary>
public partial class dep : ITableView
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
    /// Не используется. Тип отделения:
    /// LU.TAG=10(CODE).
    /// 100 - все мед.учреждение
    /// 101 - амбулаторные
    /// 102 - стационарные
    /// 103 - диагностические
    /// 104 - остальные
    /// 105 - специальные;
    /// </summary>
    public int typeid { get; set; }

    /// <summary>
    /// Ссылка на вышестоящее отделение. DEP.KEYID. 0 для отделения верхнего уровня
    /// </summary>
    public long rootid { get; set; }

    /// <summary>
    /// Статус отделения:
    /// 0 - архивный
    /// 1 - активный;
    /// </summary>
    public short status { get; set; }

    /// <summary>
    /// Отделение с койко-фондом:
    /// 0 - без койко-фонда
    /// 1 - с койко-фондом;
    /// </summary>
    public short? typ { get; set; }

    /// <summary>
    /// Уровень вложенности:
    /// 1 - все мед. учреждения (структуры)
    /// 2 - тип отделения
    /// 3 -.. прочие отделения;
    /// </summary>
    public int? lev { get; set; }

    /// <summary>
    /// Уровень, сортировка на каждом уровне
    /// Для каждого уровня выделено 3 символа для порядкового номера.
    /// Значение &apos;&apos;&apos;&apos;002005&apos;&apos;&apos;&apos; означает что отделение находится на втором уровне 5-м по порядку, а вышестоящий уровень 2-й по порядку.
    /// Классификатор отделений допускает 8 уровней.
    /// </summary>
    public string sortcode { get; set; } = null!;

    /// <summary>
    /// Код
    /// </summary>
    public string? code { get; set; }

    /// <summary>
    /// Название 
    /// </summary>
    public string? text { get; set; }

    /// <summary>
    /// Короткое название
    /// </summary>
    public string? stext { get; set; }

    /// <summary>
    /// Количество коек - для стационарных отделений
    /// </summary>
    public int? bed { get; set; }

    /// <summary>
    /// Номер отделения
    /// </summary>
    public int? num { get; set; }

    /// <summary>
    /// Примечания
    /// </summary>
    public string? note { get; set; }

    /// <summary>
    /// Отделение выбытия. 1-ДА
    /// </summary>
    public short? out_status { get; set; }

    /// <summary>
    /// Отделение поступления. 1-ДА
    /// </summary>
    public short? in_status { get; set; }

    /// <summary>
    /// Отделение местоположения. 1-ДА
    /// </summary>
    public short? place_status { get; set; }

    /// <summary>
    /// Код местоположения
    /// </summary>
    public string? p_code { get; set; }

    /// <summary>
    /// Группа для городской сводки. LU.TAG=48. Для объединения в отчетные группы для города
    /// </summary>
    public long? luid { get; set; }

    /// <summary>
    /// Внутренний код
    /// </summary>
    public string? internal_code { get; set; }

    /// <summary>
    /// Тип отделения. LU.TAG=202, хранится CODE
    /// </summary>
    public long? status_dep { get; set; }

    /// <summary>
    /// Номер кабинета для отделений типа КАБИНЕТ (DEP.STATUS_DEP=400)
    /// </summary>
    public string? room { get; set; }

    /// <summary>
    /// Оперирующее отделение. 1-ДА
    /// </summary>
    public short? oper_status { get; set; }

    /// <summary>
    /// Профиль патологии. LU.TAG=283
    /// </summary>
    public long? profpat_lu_id { get; set; }

    /// <summary>
    /// Профиль лечения на отделении по справочнику ОМС (Поля нет в интерфейсе)
    /// </summary>
    public short? oms_prof_code { get; set; }

    /// <summary>
    /// Консультирующее отделение. 1-ДА
    /// </summary>
    public short? is_consulting { get; set; }

    /// <summary>
    /// Группа, к которой относится отделение, имеет общие коечный фонд (палаты и койки)
    /// </summary>
    public long? chamber_group_status { get; set; }

    /// <summary>
    /// ЛПУ, к которому относится отделение (LPU.KEYID)
    /// </summary>
    public long? lpu_id { get; set; }

    /// <summary>
    /// Может ли быть профильным. 0 - не может; 1 - может;
    /// </summary>
    public short? can_prof_status { get; set; }

    /// <summary>
    /// Порядковый номер. Поле для сортировки при отображении отделений в одной ветке
    /// </summary>
    public string? sort { get; set; }

    /// <summary>
    /// Предельно сокращенное название отделения
    /// </summary>
    public string? abbr_text { get; set; }

    /// <summary>
    /// Федеральный реестр медицинских организаций. Подразделение. INETUSER.FRMO_DEP.ID
    /// </summary>
    public long? frmo_dep_id { get; set; }

    /// <summary>
    /// Структура. 1-ДА.
    /// </summary>
    public short? is_struct { get; set; }

    /// <summary>
    /// Идентификатор в сервисе самозаписи
    /// </summary>
    public long? nsi_id { get; set; }

    public virtual ICollection<visit>? Visits { get; set; }
    public virtual ICollection<docdep>? Docdeps { get; set; }
    public virtual dep? Root { get; set; }
}
