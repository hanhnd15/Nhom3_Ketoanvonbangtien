using System;
using System.ComponentModel.DataAnnotations;

namespace MoneyManagementApp.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        [Required]
        public decimal Amount { get; set; }  // Số tiền

        [Required]
        [StringLength(50)]
        public string TransactionType { get; set; } = string.Empty; // Giá trị mặc định // 'Thu' hoặc 'Chi'

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime TransactionDate { get; set; }  // Ngày giao dịch

        [Required]
        [StringLength(50)]
        public string DebitAccount { get; set; } = string.Empty; // Tài khoản Nợ

        [Required]
        [StringLength(50)]
        public string CreditAccount { get; set; } = string.Empty; // Tài khoản Có
    }
}
