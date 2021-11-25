using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace AliasWebApiCore.Models
{
    public class MiscModels
    {
    }

    public class AccountDetails
    {
        public int AccountId { get; set; }
        public decimal? DailyInterestSum { get; set; }
    }

    public class TransactionAccept
    {
        public Transaction transaction { get; set; }
        public Cheques Cheque { get; set; }
        public string ClearingType { get; set; }
        public Sweep Sweep { get; set; }
        public int TellerId { get; set; }
    }

    //public class SmsContent
    //{
    //    public string source { get; set; }
    //    public string destination { get; set; }
    //    public string message { get; set; }
    //}
    public class FdModel
    {
        public Transaction Transaction { get; set; }
        public string Key { get; set; }
        public int FixedDepositAccountId { get; set; }
        public decimal Amount { get; set; }
        public decimal NewInterestRate { get; set; }
        public bool RolloverInterest { get; set; }
        public bool RolloverPrincipal { get; set; }
        public FixedDeposit FixedDeposit { get; set; }
        public int BranchId { get; set; }
        public int CreatedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int NewFdInterestRate { get; set; }
        public int NewPeriod { get; set; }
    }

    //public class SavingsInterestModel
    //{
    //    public DateTime StartDate { get; set; }
    //    public DateTime EndDate { get; set; }
    //    public int[] InterestTypes { get; set; }
    //}

    //public class AccountFormat
    //{
    //    public int AccountId { get; set; }
    //    public int AccountTypeId { get; set; }
    //    public string SavingsInterestCalculationMethod { get; set; }
    //    public int? SavingsInterestTypeId { get; set; }
    //    public double PercentageValue { get; set; }
    //    public List<decimal> Amount { get; set; }
    //    public List<decimal> Balances { get; set; }
    //}

    //public class BalanceCalcModel
    //{
    //    public decimal Balance { get; set; }
    //    public double PercentageValue { get; set; }
    //}
    //public class DomainProfile : Profile
    //{
    //    public DomainProfile()
    //    {
    //        CreateMap<DomainUser, UserViewModel>();
    //    }
    //}
}
