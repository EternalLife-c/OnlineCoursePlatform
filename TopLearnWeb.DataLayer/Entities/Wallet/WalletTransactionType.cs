using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopLearn.DataLayer.Entities.Wallet
{
    public class WalletTransactionType
    {

        [Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TransactionTypeId { get; set; }

        [Required]
        [MaxLength(150)]
        public string TypeTitle { get; set; }

        #region Relations

        public virtual List<Wallet> Wallets { get; set; }

        #endregion
    }
}
