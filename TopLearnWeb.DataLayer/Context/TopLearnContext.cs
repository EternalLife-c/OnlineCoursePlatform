using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TopLearn.DataLayer.Entities.Wallet;
using TopLearnWeb.DataLayer.Entities.User;

namespace TopLearnWeb.DataLayer.Context
{
    public class TopLearnContext : DbContext
    {
        public TopLearnContext(DbContextOptions<TopLearnContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>().HasQueryFilter(u => !u.Deleted);
            modelBuilder.Entity<Role>().HasQueryFilter(u => !u.Deleted);

            #region Seed Data

            #region WalletTransactionType Seed Data

            modelBuilder.Entity<WalletTransactionType>().HasData(
                            new WalletTransactionType
                            {
                                TransactionTypeId = 1,
                                TypeTitle = "واریز"
                            },
                            new WalletTransactionType
                            {
                                TransactionTypeId = 2,
                                TypeTitle = "برداشت"
                            }
                            );

            #endregion

            #endregion

        }

        #region User

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        #endregion

        #region Wallet

        public DbSet<WalletTransactionType> WalletTransactionTypes { get; set; }

        public DbSet<Wallet> Wallets { get; set; }

        #endregion
    }
}
