using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AliasWebApiCore.Services
{
    public interface IScheduler
    {
        Task<bool> DiscoverServices(SavingsInterestModel model = null);
        Task<bool> Sms();
        Task<bool> Sweep();
        Task<bool> SessionManager();
        //Task<bool> SavingsInterest(SavingsInterestModel model = null);
        //Task<bool> Cot(SavingsInterestModel model = null);
        //Task<bool> FixedDeposositInterestAccrued();
        //Task<bool> PlacementInterestAccrued();
        //Task<bool> SavingsInterest(SavingsInterestModel model);
    }
}
