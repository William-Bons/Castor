using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Castor.database.tab_medis;

/// <summary>
/// Универсальный справочник (простой классификатор)
/// </summary>
public partial class lu
{
    [Key]
    public long keyid { get; set; }

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
    /// № справочника. Для заголовка справочника: TAG=0, CODE=№справочника
    /// </summary>
    public long tag { get; set; }

    /// <summary>
    /// Сокращенное наименование
    /// </summary>
    public string? shorttext { get; set; }

    /// <summary>
    /// Наименование
    /// </summary>
    public string? text { get; set; }

    /// <summary>
    /// Системный код значения
    /// </summary>
    public string? lcode { get; set; }

    /// <summary>
    /// Код значения
    /// </summary>
    public long? code { get; set; }

    /// <summary>
    /// Дата начала действия
    /// </summary>
    public DateTime? bgndat { get; set; }

    /// <summary>
    /// Дата окончания действия
    /// </summary>
    public DateTime? enddat { get; set; }

    /// <summary>
    /// Статус значения: 1 - активный
    /// 0 - архивный;
    /// </summary>
    public short status { get; set; }

    /// <summary>
    /// Код ОМС
    /// </summary>
    public string? oms_code { get; set; }

    /// <summary>
    /// Комментарий
    /// </summary>
    public string? note { get; set; }

    /// <summary>
    /// Порядок сортировки
    /// </summary>
    public string? sortcode { get; set; }

    /// <summary>
    /// Тип содержимого (используется при работе инсталлера): 0-содержимое не обязательно; 1-произвольное; 2-фиксированное;
    /// </summary>
    public short? content_type { get; set; }

    /// <summary>
    /// Цвет для выделения записи, связанной с этим значением справочника
    /// </summary>
    public string? color { get; set; }

    /// <summary>
    /// (не используется) Ссылка на раздел в иерархическом типе справочника
    /// </summary>
    public long? rootid { get; set; }

    /// <summary>
    /// Тип справочника: 0-плоский; 1-иерархический (устанавливается на заголовок справочника: запись с TAG=0)
    /// </summary>
    public long? typeid { get; set; }

    /// <summary>
    /// Код родительской записи для содержимого иерархического справочника.
    /// </summary>
    public long? rootcode { get; set; }

    /// <summary>
    /// SQL-запрос
    /// </summary>
    public string? query { get; set; }
}
