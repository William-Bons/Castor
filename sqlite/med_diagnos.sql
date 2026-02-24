-- Table: solution_med.diagnos

-- DROP TABLE IF EXISTS solution_med.diagnos;

CREATE TABLE IF NOT EXISTS solution_med.diagnos
(
    keyid bigint NOT NULL,
    createby bigint,
    createdate timestamp without time zone DEFAULT LOCALTIMESTAMP,
    updateby bigint,
    updatedate timestamp without time zone DEFAULT LOCALTIMESTAMP,
    rootid bigint NOT NULL,
    lev character varying(2) COLLATE pg_catalog."default",
    code character varying(12) COLLATE pg_catalog."default",
    text character varying(512) COLLATE pg_catalog."default" NOT NULL,
    note character varying(4000) COLLATE pg_catalog."default",
    code_group1 character varying(12) COLLATE pg_catalog."default",
    code_group2 character varying(12) COLLATE pg_catalog."default",
    code_group1_id bigint,
    code_group2_id bigint,
    block character varying(128) COLLATE pg_catalog."default",
    block_name character varying(256) COLLATE pg_catalog."default",
    oms_status smallint,
    depid bigint,
    docdepid bigint,
    diag_ref_id bigint,
    mes_id bigint,
    lu_id bigint,
    age_from smallint,
    age_to smallint,
    chronic_status smallint,
    sex smallint,
    days smallint,
    report_group_id bigint,
    status smallint NOT NULL DEFAULT 1,
    not_main_status smallint,
    bgndat timestamp without time zone,
    enddat timestamp without time zone,
    CONSTRAINT pk_diagnos PRIMARY KEY (keyid),
    CONSTRAINT fk_diagnos_main_diagnos FOREIGN KEY (code_group1_id)
        REFERENCES solution_med.diagnos (keyid) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS solution_med.diagnos
    OWNER to "SOLUTION_MED";

REVOKE ALL ON TABLE solution_med.diagnos FROM "INETUSER";
REVOKE ALL ON TABLE solution_med.diagnos FROM "INFIS";
REVOKE ALL ON TABLE solution_med.diagnos FROM "SOLUTION_DBF";
REVOKE ALL ON TABLE solution_med.diagnos FROM "SOLUTION_EPID";
REVOKE ALL ON TABLE solution_med.diagnos FROM "SOLUTION_LAB";
REVOKE ALL ON TABLE solution_med.diagnos FROM "SOLUTION_LOG";
REVOKE ALL ON TABLE solution_med.diagnos FROM "SOLUTION_OMS";
REVOKE ALL ON TABLE solution_med.diagnos FROM "SOLUTION_TMP";
REVOKE ALL ON TABLE solution_med.diagnos FROM "U_SOLUTION_MED";
REVOKE ALL ON TABLE solution_med.diagnos FROM "U_SOLUTION_OMS";
REVOKE ALL ON TABLE solution_med.diagnos FROM dli;

GRANT SELECT ON TABLE solution_med.diagnos TO "INETUSER" WITH GRANT OPTION;

GRANT SELECT ON TABLE solution_med.diagnos TO "INFIS";

GRANT SELECT ON TABLE solution_med.diagnos TO "SOLUTION_DBF";

GRANT REFERENCES ON TABLE solution_med.diagnos TO "SOLUTION_EPID";

GRANT REFERENCES ON TABLE solution_med.diagnos TO "SOLUTION_LAB";

GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE solution_med.diagnos TO "SOLUTION_LOG";

GRANT ALL ON TABLE solution_med.diagnos TO "SOLUTION_MED";

GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE solution_med.diagnos TO "SOLUTION_OMS";

GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE ON TABLE solution_med.diagnos TO "SOLUTION_TMP";

GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE solution_med.diagnos TO "U_SOLUTION_MED";

GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE solution_med.diagnos TO "U_SOLUTION_OMS";

GRANT SELECT ON TABLE solution_med.diagnos TO dli;

GRANT ALL ON TABLE solution_med.diagnos TO nlab;

COMMENT ON TABLE solution_med.diagnos
    IS 'Диагнозы';

COMMENT ON COLUMN solution_med.diagnos.createby
    IS 'Кто создал';

COMMENT ON COLUMN solution_med.diagnos.createdate
    IS 'Дата создания';

COMMENT ON COLUMN solution_med.diagnos.updateby
    IS 'Кто последний изменил';

COMMENT ON COLUMN solution_med.diagnos.updatedate
    IS 'Дата последнего изменения';

COMMENT ON COLUMN solution_med.diagnos.rootid
    IS 'Ссылка на запись предыдущего уровня.
0 для классификаторов';

COMMENT ON COLUMN solution_med.diagnos.lev
    IS 'Уровень вложенности';

COMMENT ON COLUMN solution_med.diagnos.code
    IS 'Код диагноза';

COMMENT ON COLUMN solution_med.diagnos.text
    IS 'Наименование диагноза';

COMMENT ON COLUMN solution_med.diagnos.note
    IS 'Замечания';

COMMENT ON COLUMN solution_med.diagnos.code_group1
    IS 'Используется для сортировки первого уровня МКБ-10';

COMMENT ON COLUMN solution_med.diagnos.code_group1_id
    IS 'KEYID классификатора (ссылка на diagnos с rootid = 0)';

COMMENT ON COLUMN solution_med.diagnos.oms_status
    IS 'Можно выставлять в ОМС';

COMMENT ON COLUMN solution_med.diagnos.depid
    IS 'Ссылка на отделение';

COMMENT ON COLUMN solution_med.diagnos.docdepid
    IS 'Ссылка на врача';

COMMENT ON COLUMN solution_med.diagnos.diag_ref_id
    IS 'Ссылка на диагноз из МКБ';

COMMENT ON COLUMN solution_med.diagnos.mes_id
    IS 'Ссылка на МЭС (таблица MES)';

COMMENT ON COLUMN solution_med.diagnos.lu_id
    IS 'Тип диагноза. Ссылка на справочник LU.TAG=72';

COMMENT ON COLUMN solution_med.diagnos.age_from
    IS 'Возраст от которого можно выставлять диагноз';

COMMENT ON COLUMN solution_med.diagnos.age_to
    IS 'Возраст до которого можно выставлять диагноз';

COMMENT ON COLUMN solution_med.diagnos.chronic_status
    IS 'Тип диагноза: 1-хронический; 2-острый (определяет правила заполнения patdiag.illtypeid, который заполняется по LU.TAG=73)';

COMMENT ON COLUMN solution_med.diagnos.sex
    IS '0 - диагноз работает и для мужчин и для женщин; 1 - только для мужчин; 2 - только для женщин;';

COMMENT ON COLUMN solution_med.diagnos.days
    IS 'Для установки дней, в течение которых диагноз считается 1 раз';

COMMENT ON COLUMN solution_med.diagnos.report_group_id
    IS 'Группа для учета диагноза 1 раз по группе';

COMMENT ON COLUMN solution_med.diagnos.status
    IS 'Статус: 1-активный; 0-архивный;';

COMMENT ON COLUMN solution_med.diagnos.not_main_status
    IS 'Статус запрета на основной диагноз (0 или NULL - может быть основным, 1 - не может быть)';

COMMENT ON COLUMN solution_med.diagnos.bgndat
    IS 'Начало действия записи';

COMMENT ON COLUMN solution_med.diagnos.enddat
    IS 'Окончание действия записи';
-- Index: i_diagnos_case_report_group_id

-- DROP INDEX IF EXISTS solution_med.i_diagnos_case_report_group_id;

CREATE INDEX IF NOT EXISTS i_diagnos_case_report_group_id
    ON solution_med.diagnos USING btree
    ((
CASE COALESCE(report_group_id, 0::bigint)
    WHEN 0 THEN keyid * 1000 + 999
    ELSE report_group_id
END) ASC NULLS LAST)
    TABLESPACE pg_default;
-- Index: i_diagnos_code

-- DROP INDEX IF EXISTS solution_med.i_diagnos_code;

CREATE INDEX IF NOT EXISTS i_diagnos_code
    ON solution_med.diagnos USING btree
    (code COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;
-- Index: i_diagnos_code_group1_id

-- DROP INDEX IF EXISTS solution_med.i_diagnos_code_group1_id;

CREATE INDEX IF NOT EXISTS i_diagnos_code_group1_id
    ON solution_med.diagnos USING btree
    (code_group1_id ASC NULLS LAST)
    TABLESPACE pg_default;
-- Index: i_diagnos_code_varchar_pattern_ops

-- DROP INDEX IF EXISTS solution_med.i_diagnos_code_varchar_pattern_ops;

CREATE INDEX IF NOT EXISTS i_diagnos_code_varchar_pattern_ops
    ON solution_med.diagnos USING btree
    (code COLLATE pg_catalog."default" varchar_pattern_ops ASC NULLS LAST)
    TABLESPACE pg_default;
-- Index: i_diagnos_lev

-- DROP INDEX IF EXISTS solution_med.i_diagnos_lev;

CREATE INDEX IF NOT EXISTS i_diagnos_lev
    ON solution_med.diagnos USING btree
    (lev COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;
-- Index: i_diagnos_report_group_id

-- DROP INDEX IF EXISTS solution_med.i_diagnos_report_group_id;

CREATE INDEX IF NOT EXISTS i_diagnos_report_group_id
    ON solution_med.diagnos USING btree
    (report_group_id ASC NULLS LAST)
    TABLESPACE pg_default;
-- Index: i_diagnos_rootid

-- DROP INDEX IF EXISTS solution_med.i_diagnos_rootid;

CREATE INDEX IF NOT EXISTS i_diagnos_rootid
    ON solution_med.diagnos USING btree
    (rootid ASC NULLS LAST)
    TABLESPACE pg_default;
-- Index: i_diagnos_text

-- DROP INDEX IF EXISTS solution_med.i_diagnos_text;

CREATE INDEX IF NOT EXISTS i_diagnos_text
    ON solution_med.diagnos USING btree
    (text COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;

-- Trigger: trg_diagnos

-- DROP TRIGGER IF EXISTS trg_diagnos ON solution_med.diagnos;

CREATE OR REPLACE TRIGGER trg_diagnos
    BEFORE INSERT OR UPDATE 
    ON solution_med.diagnos
    FOR EACH ROW
    EXECUTE FUNCTION solution_med.trigger_fct_trg_diagnos();