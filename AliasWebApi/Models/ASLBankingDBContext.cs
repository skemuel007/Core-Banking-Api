using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AliasWebApiCore.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace AliasWebApiCore.Models
{
    public class AslBankingDbContext:DbContext
    {

        public AslBankingDbContext(DbContextOptions<AslBankingDbContext> options) : base(options)
        {
           
        }

        public DbSet<AccountPopupMsg> AccountPopupMsg { get; set; }
        public DbSet<FixedDeposit> FixedDeposit { get; set; }
        public DbSet<ServiceConfig> ServiceConfig { get; set; }
        public DbSet<SessionManager> SessionManager { get; set; }
        public DbSet<Target> Targets { get; set; }
        public DbSet<RelatedDocument> RelatedDocuments { get; set; }
        public DbSet<CustomerType> CustomerTypes { get; set; }
        public DbSet<Corporate> Corporates { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<LoanServicing> LoanServicings { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Sector> Sectors { get; set; }
        public DbSet<CountryList> CountryLists { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<FileUpload> FileUploads { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Banktiers> Banktiers { get; set; }
        public DbSet<AccountTypes> AccountTypes { get; set; }
        public DbSet<Ledgers> Ledgers { get; set; }
        public DbSet<JointCustomer> JointCustomers { get; set; }
        public DbSet<JointCustomersKeys> JointCustomersKeys { get; set; }
        public DbSet<CompanySignatory> CompanySignatories { get; set; }
        public DbSet<CompanyDirectors> CompanyDirectorses { get; set; }
        public DbSet<BranchDetails> BranchDetails { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Teller> Teller { get; set; }
        public DbSet<Cheques> Cheques { get; set; }
        public DbSet<IssuedChequeBooks> IssuedChequeBooks { get; set; }
        public DbSet<GeneralLedgerCode> GeneralLedgerCodes { get; set; }
        public DbSet<MainGeneralLedgerCodes> MainGeneralLedgerCodes { get; set; }
        public DbSet<Liens> Liens { get; set; }
        public DbSet<Overdrafts> Overdrafts { get; set; }
        public DbSet<Sweep> Sweeps { get; set; }
        public DbSet<Individual> Individuals { get; set; }
        public DbSet<BankDetails> BankDetails { get; set; }
        public DbSet<CommonSequence> CommonSequences { get; set; }
        public DbSet<TransCodeItems> TransCodeItems { get; set; }
        public DbSet<ApprovalRules> ApprovalRules { get; set; }
        public DbSet<SmsLog> SmsLog { get; set; }
        public DbSet<SmsConfig> SmsConfig { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
            builder.Entity<UserGroup>().HasKey(table => new
            {
                table.CreatedUserId,
                table.GroupId
            });
        }
        
    }
}