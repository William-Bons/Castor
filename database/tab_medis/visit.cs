using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Castor.database.tab_medis;

/// <summary>
///  Класс таблицы базы данных Медис
///  
/// Визиты пациентов; Периоды госпитализации;
/// </summary>
public partial class visit
{
    /// <summary>
    /// визит
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
    /// Номер ИБ/Случая лечения. 
    /// В стационаре - номер истории болезни. Заполняется для всех стационарных визитов. Нумерация начинается с 1 каждый год
    /// В поликлинике - номер талона. Заполняется на всех посещениях случая.
    /// </summary>
    public long num { get; set; }

    /// <summary>
    /// Дата посещения/Дата начала события
    /// </summary>
    public DateTime? dat { get; set; }

    /// <summary>
    /// Дата конца события
    /// </summary>
    public DateTime? dat1 { get; set; }

    /// <summary>
    /// Тип события
    /// &lt;100: LU.TAG=20
    /// 100-очередь на госпитализацию
    /// 101-регистрация в ПО
    /// 102-поступл. на отд.
    /// 103-перевод
    /// 104-смена койки
    /// 105-операция
    /// 106-прерывание КД
    /// 107-смена врача
    /// 108-бронь койки
    /// 150-скорая помощь;
    /// </summary>
    public int vistype { get; set; }

    /// <summary>
    /// 0 - обычные; 1-дети; 2-ожидаемые(?);
    /// </summary>
    public int code { get; set; }

    /// <summary>
    /// Пациент
    /// </summary>
    public long? patientid { get; set; }

    /// <summary>
    /// Отделение поступления
    /// </summary>
    public long? depid { get; set; }

    /// <summary>
    /// Ссылка на KEYID - визита регистрации (VISTYPE=101)
    /// </summary>
    public long? rootid { get; set; }

    /// <summary>
    /// Отделение выбытия
    /// </summary>
    public long? dep1id { get; set; }

    /// <summary>
    /// Профильное отделение
    /// </summary>
    public long? depprofid { get; set; }

    /// <summary>
    /// Шифр
    /// </summary>
    public long? agrid { get; set; }

    /// <summary>
    /// Доктор (DOCDEP.KEYID)
    /// </summary>
    public long? doctorid { get; set; }

    /// <summary>
    /// Койка из BED Если -2=&lt;без указания&gt;
    /// </summary>
    public long? bedid { get; set; }

    /// <summary>
    /// Стационар: профиль койки (FCATEG.TAG=2); Амбулатория: профиль патологии LU.TAG=283 Если -2=&lt;без указания&gt;
    /// </summary>
    public long? profid { get; set; }

    /// <summary>
    /// Категория койки (FCATEG.TAG=3) Если -2=&lt;без указания&gt;
    /// </summary>
    public long? categid { get; set; }

    /// <summary>
    /// Медсестра (DOCDEP.KEYID)
    /// </summary>
    public long? docid { get; set; }

    /// <summary>
    /// Выставлять в ОМС (Эпикриз)
    /// </summary>
    public short? omsstatus { get; set; }

    /// <summary>
    /// Для стационарных визитов 0-не обработан; 1-обработан.(ИБ закрыта); 2-ИБ закрыта во ВрачеСтационара;
    /// </summary>
    public short? status { get; set; }

    /// <summary>
    /// Комментарий
    /// </summary>
    public string? note { get; set; }

    /// <summary>
    /// Исход госпитализации LU.TAG=345
    /// </summary>
    public long? resultid { get; set; }

    /// <summary>
    /// Случай. (первичный, повторный) LU.TAG=52
    /// </summary>
    public long? caseid { get; set; }

    /// <summary>
    /// Законченность LU.TAG=53 (закончен или нет)
    /// </summary>
    public long? casetypeid { get; set; }

    /// <summary>
    /// Исход заболевания LU.TAG=54
    /// </summary>
    public long? caseresultid { get; set; }

    /// <summary>
    /// Место обслуживания LU.TAG=57
    /// </summary>
    public long? placeid { get; set; }

    /// <summary>
    /// Полис (договор)
    /// </summary>
    public long? policeid { get; set; }

    /// <summary>
    /// Доп. поле под служебные отделения (за кем числить смерть, отделение обращения при отк. случае) и т.д.
    /// - для выписки &quot;Перевод в другие стационары&quot; - стационар, в который выбытие. Заполняется Из справочника lu.tag=25
    /// - для выписки &quot;Смерть&quot; - отделение, за которым числить. Заполняется из dep
    /// </summary>
    public long? dep2id { get; set; }

    /// <summary>
    /// Текстовое поле &quot;Профиль оплаты ОМС&quot; в выписном экипризе.
    /// </summary>
    public string? omspayprof { get; set; }

    /// <summary>
    /// Направление
    /// </summary>
    public long? reg_direction_id { get; set; }

    /// <summary>
    /// Учитывать посещение в статистике:  0 - да; 1 - нет;
    /// </summary>
    public short? calc_status { get; set; }

    /// <summary>
    /// Для амб. визитов. (1 - распечатан стат. талон).
    /// </summary>
    public short? print_status { get; set; }

    /// <summary>
    /// Результат лечения LU.TAG=188
    /// </summary>
    public long? caseresult2id { get; set; }

    /// <summary>
    /// Кем направлен. LU.TAG=92
    /// </summary>
    public long? sendby_id { get; set; }

    /// <summary>
    /// Стационар: Профиль заболевания LU.TAG=292; Амбулатория: Профиль ДС (FCATEG.KEYID)
    /// </summary>
    public long? profillid { get; set; }

    /// <summary>
    /// Статус наличия родственника на стационарном промежутке. 1-есть;
    /// </summary>
    public short? relative_status { get; set; }

    /// <summary>
    /// Имя родственника
    /// </summary>
    public string? relative_name { get; set; }

    /// <summary>
    /// Ссылка на актуальную на момент посещения &quot;архивную карту&quot; пациента
    /// </summary>
    public long? pa_history_id { get; set; }

    /// <summary>
    /// Ссылка на тип профосмотра. LU.TAG=334
    /// </summary>
    public long? examlu_id { get; set; }

    /// <summary>
    /// Ссылка на МЭС
    /// </summary>
    public long? mes_id { get; set; }

    /// <summary>
    /// Ссылка на основное посещение по МЭС
    /// </summary>
    public long? mesrootid { get; set; }

    /// <summary>
    /// Степень сложности койки. LU.TAG=337
    /// </summary>
    public long? complexlu_id { get; set; }

    /// <summary>
    /// Дата обработки (в выписном эпикризе при закрытии, в абм.истории при сохранении талона)
    /// </summary>
    public DateTime? process_dat { get; set; }

    /// <summary>
    /// Доп.отметка визита. Заполняется при записи на прием. LU.TAG=341
    /// </summary>
    public long? option_id { get; set; }

    /// <summary>
    /// Тип визита. LU.TAG=249 случай или разовое
    /// </summary>
    public long? visit_type_id { get; set; }

    /// <summary>
    /// Ссылка на 101 визит матери. Ведется у новорожденных, чтобы определить ИБ матери
    /// </summary>
    public long? rel_visit_id { get; set; }

    /// <summary>
    /// Статус оповещения (смс, голос). подробнее в коментарии к полю informer.status
    /// </summary>
    public short? notif_status { get; set; }

    /// <summary>
    /// Статус редактирования: 0 - можно всем; 1 - можно только мне (я редактирую сейчас, больше никому нельзя); 2 - нельзя никому (запись закрыта для изменений);
    /// </summary>
    public short? edit_now_status { get; set; }

    /// <summary>
    /// Выставлять ВМП (Эпикриз)
    /// </summary>
    public short? vmpstatus { get; set; }

    /// <summary>
    /// Дата планируемой выписки
    /// </summary>
    public DateTime? plan_dat { get; set; }

    /// <summary>
    /// 0 = свободен; 1 = частично свободен; 2 = занят;
    /// </summary>
    public short? status_occupied { get; set; }

    /// <summary>
    /// Тип госпитализации (Эпикриз) LU.TAG=134
    /// </summary>
    public long? hosp_type_luid { get; set; }

    /// <summary>
    /// Ссылка на абонемент
    /// </summary>
    public long? abonement_id { get; set; }

    /// <summary>
    /// Упрощенное ведение ВМП. Тип ВМП
    /// </summary>
    public long? vmp_type_id { get; set; }

    /// <summary>
    /// Упрощенное ведение ВМП. Метод ВМП
    /// </summary>
    public long? vmp_method_id { get; set; }

    /// <summary>
    /// Бронирование
    /// </summary>
    public long? booking_id { get; set; }

    public virtual ICollection<patserv>? Patservs { get; set; }
    public virtual visit? root { get; set; }
    public virtual patient? Patient { get; set; }
    public virtual dep? Dep { get; set; }
    public virtual docdep? Doctor { get; set; }
    public virtual int DaysInDep => dat != null ? (DateTime.Now - dat).Value.Days : 0;
    public virtual string Fullname => Patient?.fullname ?? string.Empty;
    public virtual int Age => Patient?.age ?? 0;
}
