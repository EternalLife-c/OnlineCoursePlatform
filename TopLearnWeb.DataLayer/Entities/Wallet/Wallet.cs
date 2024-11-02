using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearnWeb.DataLayer.Entities.User;

namespace TopLearn.DataLayer.Entities.Wallet
{
    public class Wallet
    {
        [Key]
        public int WalletId { get; set; }

        [Display(Name = "کاربر")]
        public int UserId { get; set; }

        [Display(Name = "نوع تراکنش")]
        public int TransactionTypeId { get; set; }  

        [Display(Name = "مبلغ")]
        public int Amount { get; set; }

        [Display(Name = "شرح")]
        [MaxLength(500, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکتر باشد")]
        public string Description { get; set; }

        [Display(Name = "انجام شده")]
        public bool IsPaid { get; set; }

        [Display(Name = "تاریخ و ساعت")]
        public DateTime TransactionDate { get; set; }

        #region Relations

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("TransactionTypeId")]
        public virtual WalletTransactionType Type { get; set; }

        #endregion

    }
}
