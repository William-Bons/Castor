using Castor.gui.common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Castor.database.tab_medis;

/// <summary>
/// Диагнозы пациента
/// </summary>
public partial class patdiag : ITableView
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
    /// Пациент
    /// </summary>
    public long patientid { get; set; }

    /// <summary>
    /// Диагноз
    /// </summary>
    public long diagid { get; set; }

    /// <summary>
    /// Визит
    /// </summary>
    public long visitid { get; set; }

    /// <summary>
    /// Дата выставления диагноза
    /// </summary>
    public DateTime? dat { get; set; }

    /// <summary>
    /// Текст диагноза
    /// </summary>
    public string? text { get; set; }

    /// <summary>
    /// Тип диагноза:
    /// LU.TAG=72 (значение CODE)
    /// </summary>
    public int diagtype { get; set; }

    /// <summary>
    /// Код диагноза:
    /// 1-точно; 2-под вопросом;
    /// </summary>
    public int diagcode { get; set; }

    /// <summary>
    /// Первичный диагноз:
    /// 1-впервые; 2-повторно;
    /// </summary>
    public int diagfirst { get; set; }

    /// <summary>
    /// Замечания
    /// </summary>
    public string? note { get; set; }

    /// <summary>
    /// Не используется. Отборочная комиссия
    /// </summary>
    public long? patcommid { get; set; }

    /// <summary>
    /// МЭС
    /// </summary>
    public long? mesid { get; set; }

    /// <summary>
    /// Характер заболевания. LU.TAG=73
    /// </summary>
    public long? illtypeid { get; set; }

    /// <summary>
    /// Отношение к диспансерному учету. LU.TAG=74
    /// </summary>
    public long? dispid { get; set; }

    /// <summary>
    /// Причина снятия с учета. LU.TAG=75
    /// </summary>
    public long? dispoffid { get; set; }

    /// <summary>
    /// Травма. LU.TAG=76
    /// </summary>
    public long? travmaid { get; set; }

    /// <summary>
    /// Обострение. LU.TAG=164
    /// </summary>
    public long? worse_id { get; set; }

    /// <summary>
    /// Противоправные действия. LU.TAG=162
    /// </summary>
    public long? crime_id { get; set; }

    /// <summary>
    /// Стадия заболевания. LU.TAG=163
    /// </summary>
    public long? stage_id { get; set; }

    /// <summary>
    /// Вид диагноза: 0-без указания; 1-поступления; 2-клинический; 3-заключительный; LU.TAG=79 (значение CODE)
    /// </summary>
    public int? diagform { get; set; }

    /// <summary>
    /// Статус внутреннего классификатора. 0 - МКБ; 1 - Внутренний;
    /// </summary>
    public short? status_int { get; set; }

    /// <summary>
    /// T — размер опухоли и степень её врастания в ткани. LU.TAG=265
    /// </summary>
    public long? tnm_t_lu_id { get; set; }

    /// <summary>
    /// N — поражение лимфатических узлов. LU.TAG=266
    /// </summary>
    public long? tnm_n_lu_id { get; set; }

    /// <summary>
    /// M — наличие или отсутствие метастазов. LU.TAG=267
    /// </summary>
    public long? tnm_m_lu_id { get; set; }

    /// <summary>
    /// Стадирование (TNM и стадирование по DUKES). LU.TAG=268
    /// </summary>
    public long? stage_lu_id { get; set; }

    /// <summary>
    /// Стадирование по Dukes. LU.TAG=269
    /// </summary>
    public long? dukes_lu_id { get; set; }

    /// <summary>
    /// Внешняя причина травмы/отравления. DIAGNOS.KEYID
    /// </summary>
    public long? diag_travma_reasonid { get; set; }

    /// <summary>
    /// Ссылка на id вопроса протокола с диагнозом
    /// </summary>
    public long? form_item_id { get; set; }

    /// <summary>
    /// Ссылка на id заполненного протокола
    /// </summary>
    public long? form_result_id { get; set; }

    /// <summary>
    /// Зубная формула. Ссылка на отметку.
    /// </summary>
    public long? epic_picture_element_id { get; set; }

    /// <summary>
    /// Профессиональное заболевание. 1-ДА;
    /// </summary>
    public short? prof_type { get; set; }

    /// <summary>
    /// Орган. LU.TAG=197
    /// </summary>
    public long? organ_id { get; set; }

    public virtual visit? Visits { get; set; }

    [ForeignKey(nameof(diagid))]
    public virtual diagnos? Diagnos { get; set; }
}
