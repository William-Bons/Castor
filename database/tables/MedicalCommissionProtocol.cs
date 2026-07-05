using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Castor.database.tables
{


    [Table("MedicalCommissionProtocols")]
    public class MedicalCommissionProtocol
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(20)]
        public string ProtocolNumber { get; set; } = string.Empty; // № протокола

        [Required]
        public DateTime ProtocolDate { get; set; }  // дата

        [StringLength(255)]
        public string? Location { get; set; }        // место проведения

        [StringLength(500)]
        public string CommissionType { get; set; } = "100"; // «плановое», «внеплановое», «по жалобе» и т.п.

        // Состав комиссии (навигационные свойства)
        public virtual ICollection<CommissionMember> Members { get; set; } = new List<CommissionMember>();
        public virtual CommissionMember? Chairman { get; set; }      // председатель (можно вынести отдельно или брать из Members по роли)
        public virtual CommissionMember? Secretary { get; set; }     // секретарь

        // Повестка и вопросы
        public virtual ICollection<ProtocolAgendaItem> AgendaItems { get; set; } = new List<ProtocolAgendaItem>();

        // Голосование (суммарно по протоколу или по каждому вопросу — здесь суммарно)
        public int VotesFor { get; set; }
        public int VotesAgainst { get; set; }
        public int VotesAbstained { get; set; }

        // Решение
        [Column(TypeName = "nvarchar(max)")]
        public string? DecisionText { get; set; }

        // Особое мнение
        [Column(TypeName = "nvarchar(max)")]
        public string? SpecialOpinionText { get; set; }
        public string? SpecialOpinionAuthorName { get; set; }   // Ф.И.О. автора особого мнения
        public string? SpecialOpinionAuthorPosition { get; set; } // должность

        // --- Поля для ЭЦП (новые) ---

        /// <summary>
        /// Статус подписания протокола: Не подписан / Подписан / Ошибка / Отозван
        /// </summary>
        public ProtocolSignStatus SignStatus { get; set; } = ProtocolSignStatus.NotSigned;

        /// <summary>
        /// Дата/время последнего успешного подписания протокола (не путать с датой заседания)
        /// </summary>
        public DateTime? SignedAt { get; set; }

        /// <summary>
        /// Ссылка на ответственного за подписание (если есть выделенная роль)
        /// </summary>
        public int? SignedByMemberId { get; set; }
        [ForeignKey(nameof(SignedByMemberId))]
        public CommissionMember? SignedByMember { get; set; }

        /// <summary>
        /// Коллекция подписей (каждый член комиссии может подписать отдельно)
        /// </summary>
        public virtual ICollection<ProtocolSignature> Signatures { get; set; } = new List<ProtocolSignature>();


        // Метаданные
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public enum ProtocolSignStatus
    {
        NotSigned = 0,
        Signed = 1,
        Error = 2,
        Revoked = 3
    }


    [Table("CommissionMembers")]
    public class CommissionMember
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string FullName { get; set; } = string.Empty;      // Ф.И.О.

        [Required, StringLength(150)]
        public string? Position { get; set; }       // должность

        [Required]
        public CommissionMemberRole Role { get; set; } // роль: Председатель, Член, Секретарь

        [ForeignKey(nameof(ProtocolId))]
        public int ProtocolId { get; set; }
        public virtual MedicalCommissionProtocol? Protocol { get; set; }
    }

    public enum CommissionMemberRole
    {
        Chairman = 1,
        Member = 2,
        Secretary = 3
    }


    [Table("ProtocolAgendaItems")]
    public class ProtocolAgendaItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProtocolId { get; set; }
        public virtual MedicalCommissionProtocol? Protocol { get; set; }

        public int OrderNumber { get; set; } // порядок в повестке: 1, 2, 3…

        [Required, Column(TypeName = "nvarchar(max)")]
        public string? Topic { get; set; }    // формулировка вопроса

        [Column(TypeName = "nvarchar(max)")]
        public string? HeardSummary { get; set; }   // «Слушали»

        [Column(TypeName = "nvarchar(max)")]
        public string? ReviewedDocuments { get; set; } // «Рассмотрели» (перечень документов с номерами/датами)

        [Column(TypeName = "nvarchar(max)")]
        public string? DecisionPerItem { get; set; }  // решение именно по этому вопросу (если нужно хранить отдельно от общего решения)
    }

    [Table("ProtocolSignatures")]
    public class ProtocolSignature
    {
        [Key]
        public int Id { get; set; }

        public int ProtocolId { get; set; }
        [ForeignKey(nameof(ProtocolId))]
        public MedicalCommissionProtocol Protocol { get; set; }

        // Кто подписал
        public int MemberId { get; set; }
        [ForeignKey(nameof(MemberId))]
        public CommissionMember Member { get; set; }

        // Сертификат
        [Required, StringLength(64)] // SHA-1 отпечаток сертификата обычно 40 hex = 20 байт → 40 символов; SHA-256 — 64 hex
        public string CertificateThumbprint { get; set; }

        [StringLength(500)]
        public string CertificateSubject { get; set; } // «CN=Иванов И.И., O=Больница…»

        [StringLength(100)]
        public string Issuer { get; set; }             // Кто выдал сертификат

        public DateTime NotBefore { get; set; }        // Действителен с
        public DateTime NotAfter { get; set; }         // Действителен до

        // Носитель (Rutoken)
        [StringLength(100)]
        public string TokenModel { get; set; }        // «Rutoken EDS», «Rutoken Lite»
        [StringLength(100)]
        public string TokenSerialNumber { get; set; }  // Серийный номер токена

        // Данные подписи
        [Column(TypeName = "varbinary(max)")] // SQL: varbinary(max); PostgreSQL: bytea
        public byte[] SignatureData { get; set; }     // Сама подпись (PKCS#7/CMS)

        [StringLength(50)]
        public string SignatureFormat { get; set; }   // «PKCS7», «CAdES-BES», «CAdES-T» и т.д.

        public DateTime SignedAt { get; set; }

        [StringLength(500)]
        public string ValidationResult { get; set; }  // Результат проверки подписи (для аудита)
        public bool IsValid { get; set; }              // Быстрый флаг валидности

        // Метаданные
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
