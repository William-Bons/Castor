using Castor.database.tab_medis;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Castor;

/// <summary>
/// Услуги пациента
/// </summary>
public partial class patserv
{
    /// <summary>
    /// Услуга, оказанная пациенту
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
    /// Пациент
    /// </summary>
    public long patientid { get; set; }

    /// <summary>
    /// Услуга
    /// </summary>
    public long srvdepid { get; set; }

    /// <summary>
    /// Визит
    /// </summary>
    public long? visitid { get; set; }

    /// <summary>
    /// Шифр
    /// </summary>
    public long? agrid { get; set; }

    /// <summary>
    /// Статус стационарности:
    /// 1 - стационарная услуга
    /// 0 - амбулаторная;
    /// </summary>
    public short? status { get; set; }

    /// <summary>
    /// Статус выставления ОМС:
    /// 0 - не выставлять ОМС
    /// 1 - выставлять в ОМС
    /// 2 - служебная услуга ОМС;
    /// </summary>
    public short? omsstatus { get; set; }

    /// <summary>
    /// Количество
    /// </summary>
    public double qty { get; set; }

    /// <summary>
    /// Дата оказания услуги
    /// </summary>
    public DateTime? dat { get; set; }

    /// <summary>
    /// Количество процедурных единиц
    /// </summary>
    public double? procqty { get; set; }

    /// <summary>
    /// Замечания
    /// </summary>
    public string? note { get; set; }

    /// <summary>
    /// Статус выставления в оплату:
    /// 0 - услуга не выставлена в оплату
    /// 1 - услуга выставлена в оплату;
    /// </summary>
    public short? inv_status { get; set; }

    /// <summary>
    /// Направивший врач. DOCDEP.KEYID
    /// </summary>
    public long? docid_send { get; set; }

    /// <summary>
    /// Исполнители. Врач-исполнитель. DOCDEP.KEYID
    /// </summary>
    public long? docid_exec { get; set; }

    /// <summary>
    /// Исполнители. Медсестра. DOCDEP.KEYID
    /// </summary>
    public long? docid_nurse { get; set; }

    /// <summary>
    /// Ссылка на услугу в платеже
    /// </summary>
    public long? payservid { get; set; }

    /// <summary>
    /// 1 - услуга была добавлена из платежа.
    /// </summary>
    public short? from_cash_status { get; set; }

    /// <summary>
    /// Исполнители. Доктор, оказывающий услугу
    /// </summary>
    public long? docdepid { get; set; }

    /// <summary>
    /// Отмена услуги. Дата отказа от услуги
    /// </summary>
    public DateTime? refuse_dat { get; set; }

    /// <summary>
    /// Отмена услуги. Причина отказа
    /// </summary>
    public string? refuse_reason { get; set; }

    /// <summary>
    /// Отмена услуги. ID оператора, сделавшего отмену. Зависит от того кто вошел в систему.
    /// </summary>
    public long? refuse_dispid { get; set; }

    /// <summary>
    /// Статус услуги:
    /// 0 - назначена
    /// 1 - выполнена
    /// 2 - отменена
    /// 3 - планируется, но не обработана;
    /// </summary>
    public short srv_status { get; set; }

    /// <summary>
    /// Транспортабельность: 1-да; 0-нет;
    /// </summary>
    public short? transp_status { get; set; }

    /// <summary>
    /// Полис, по которому оказана услуга
    /// </summary>
    public long? policeid { get; set; }

    /// <summary>
    /// Ссылка на диагностическое исследование. SOLUTION_DIAGNOSTIC.RESEARCH.ID
    /// </summary>
    public long? research_id { get; set; }

    /// <summary>
    /// Используется в ангиографии. Обозначает основную
    /// услугу для исследования. Для основной услуги будут считаться уеты
    /// 100%, для остальных - 50%.
    /// </summary>
    public short? main_status { get; set; }

    /// <summary>
    /// Местоположение. DEP.KEYID (DEP.PLACE_STATUS=1)
    /// </summary>
    public long? placeid { get; set; }

    /// <summary>
    /// Коэффициент скидки/наценки на услугу
    /// </summary>
    public double servc { get; set; }

    /// <summary>
    /// Не используется
    /// </summary>
    public string? cabinet { get; set; }

    /// <summary>
    /// Согласование. Дата согласования
    /// </summary>
    public DateTime? confirm_dat { get; set; }

    /// <summary>
    /// Согласование. Диспетчер страховой компании
    /// </summary>
    public string? disp { get; set; }

    /// <summary>
    /// (не используется) ссылка на PATSERV_ADDINFO
    /// </summary>
    public long? addit_info { get; set; }

    /// <summary>
    /// Фиксированный тип цены для услуги. LU.TAG=149
    /// </summary>
    public long? price_lu_id { get; set; }

    /// <summary>
    /// Срочность: 1-да; 0-нет;
    /// </summary>
    public short? cito_status { get; set; }

    /// <summary>
    /// Дата направления
    /// </summary>
    public DateTime? dir_dat { get; set; }

    /// <summary>
    /// Направление
    /// </summary>
    public long? reg_direction_id { get; set; }

    /// <summary>
    /// Ссылка на услугу в направлении
    /// </summary>
    public long? reg_direction_serv_id { get; set; }

    /// <summary>
    /// Ссылка  на номерок, в общем случае не используется
    /// </summary>
    public long? rnumb_id { get; set; }

    /// <summary>
    /// Ссылка на абонемент (если услуга оказана по абонементу)
    /// </summary>
    public long? abonement_id { get; set; }

    /// <summary>
    /// Местоположение услуги для услуг, оказанных в других учреждениях (не в сети учреждений). Ссылка на LU.TAG=256
    /// </summary>
    public long? place_ext_id { get; set; }

    /// <summary>
    /// Ссылка на карту
    /// </summary>
    public long? card_id { get; set; }

    /// <summary>
    /// УЕТы врача
    /// </summary>
    public double? uet_d { get; set; }

    /// <summary>
    /// УЕТы медсестры
    /// </summary>
    public double? uet_n { get; set; }

    /// <summary>
    /// Согласование. Регистратор, последний менявший согласование
    /// </summary>
    public long? confirmby { get; set; }

    /// <summary>
    /// Согласование. Дата подтверждения выполнения услуги
    /// </summary>
    public DateTime? confirmdate { get; set; }

    /// <summary>
    /// Ссылка на основную услугу, если эта услуга является доплатой
    /// </summary>
    public long? extr_rootid { get; set; }

    /// <summary>
    /// Ссылка на ID составной услуги, назначенной пациенту (PATSERV.KEYID)
    /// </summary>
    public long? root_id { get; set; }

    /// <summary>
    /// 1-составная/комплексная услуга, NULL или 0 - обычная
    /// </summary>
    public short? complex_status { get; set; }

    /// <summary>
    /// Согласование. Комментарий к согласованию услуги
    /// </summary>
    public string? confirm_note { get; set; }

    /// <summary>
    /// Ссылка на актуальную на момент услуги &quot;архивную карту&quot; пациента
    /// </summary>
    public long? pa_history_id { get; set; }

    /// <summary>
    /// Исполнители. Ссылка на санитара (младший персонал)
    /// </summary>
    public long? docid_paramedic { get; set; }

    /// <summary>
    /// Код структуры ЛПУ, в которой выполнена услуга. DEP.SORTCODE
    /// </summary>
    public string? struct_code { get; set; }

    /// <summary>
    /// Ссылка на операцию. (OPER_OPERS.KEYID)
    /// </summary>
    public long? oper_oper_id { get; set; }

    /// <summary>
    /// Код сущности, к которой прикреплена услуга:
    /// Полный и актуальный перечень смотреть в функциях pkg_services.c_link_tag%
    /// ...
    /// 1 - манипуляции (PATMANIP.KEYID)
    /// 2 - вакцинация
    /// 3 - консультация (SOLUTION_EPIC.CONSULT.ID)
    /// 4 - стом.наряды (DENTAL_ARRAY)
    /// 5 - назначение(стационар) (SOLUTION_EPIC.CHART_UNIT)
    /// ...
    /// </summary>
    public short? link_tag { get; set; }

    /// <summary>
    /// Ссылка на ID сущности, к которой привязана услуга (в какой именно таблице эта запись, определяется по PATSERV.LINK_TAG)
    /// </summary>
    public long? link_id { get; set; }

    /// <summary>
    /// Статус редактирования:
    /// 0 - можно всем
    /// 1 - можно только мне (я редактирую сейчас, больше никому нельзя)
    /// 2 - нельзя никому (запись закрыта для изменений);
    /// </summary>
    public short? edit_now_status { get; set; }

    /// <summary>
    /// ID внешнего направившего доктора из справочника LU.TAG=800
    /// </summary>
    public long? docid_send_ext { get; set; }

    /// <summary>
    /// Ссылка на рутовую комплексную(составную) услугу в таблице SRVDEP
    /// </summary>
    public long? complex_srvdep_id { get; set; }

    /// <summary>
    /// Дата окончания оказания услуги
    /// </summary>
    public DateTime? dat2 { get; set; }

    /// <summary>
    /// ID материала для лаб.направления
    /// </summary>
    public long? material_id { get; set; }

    /// <summary>
    /// Место работы пациента
    /// </summary>
    public long? pat_works_id { get; set; }

    /// <summary>
    /// Тип ОМС случая лечения/услуги, который(ая) подлежит к оплате согласно текущей региональной интеграции с ТФОМС. 
    /// Возможные значения можно посмотреть в пакете текущей региональной интеграции. 
    /// Например: &quot;55_OMS54_AMB_VISIT&quot; означает - Законченный случай амбулаторного посещения по 54 приказу ОМС от 23.03.2018 в 55 регионе (Омск). 
    /// Реализацию можно посмотреть в пакете solution_oms.pkg_oms54_55.
    /// </summary>
    public string? oms_type { get; set; }

    /// <summary>
    /// Для выставления флага - прикреплен; не прикреплен; никуда не прикреплен; неидентифицирован;
    /// </summary>
    public short? patattach_status { get; set; }

    /// <summary>
    /// Номер КСГ/КПГ/СТГ по ОМС
    /// </summary>
    public long? ksg_num { get; set; }

    /// <summary>
    /// Место оказания услуги. ROOM.KEYID
    /// </summary>
    public long? room_id { get; set; }

    /// <summary>
    /// Оборудование, на котором выполнялась услуга
    /// </summary>
    public long? equipment_id { get; set; }

    /// <summary>
    /// Медицинская организация, которой принадлежит запись
    /// </summary>
    public long? mo_id { get; set; }

    public virtual ICollection<patserv> Inverseroot { get; set; } = new List<patserv>();

    public virtual patserv? root { get; set; }
    public virtual patient? Patient { get; set; }
}
