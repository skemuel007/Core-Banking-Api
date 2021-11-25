using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AliasWebApiCore.Models;
using AliasWebApiCore.Models.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AliasWebApiCore.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    public class DashBoardController : Controller
    {
        public readonly AppDbContext _db;
        public DashBoardController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("SetupStatus")]
        public IActionResult GetSetupStatus()
        {
            var setupstatus = new List<SetupModel>
            {
                new SetupModel
                {
                    Name = "Target", Count = _db.Targets.Count()
                },
                new SetupModel
                {
                    Name = "Sector", Count = _db.Sectors.Count()
                },
                new SetupModel
                {
                    Name = "Rating", Count = _db.Ratings.Count()
                },
                new SetupModel
                {
                    Name = "Country", Count = _db.CountryLists.Count()
                },
                new SetupModel
                {
                    Name = "Ledger", Count = _db.Ledgers.Count()
                },
                new SetupModel
                {
                    Name = "CustomerType", Count = _db.CustomerTypes.Count()
                },
                new SetupModel
                {
                    Name = "CommonSequence", Count = _db.CommonSequences.Count()
                },
                new SetupModel
                {
                    Name = "AccountType", Count = _db.AccountTypes.Count()
                },
                new SetupModel
                {
                    Name = "BranchDetails", Count = _db.BranchDetails.Count()
                },
                new SetupModel
                {
                    Name = "Banktiers", Count = _db.Banktiers.Count()
                },
                new SetupModel
                {
                    Name = "CommonSequences", Count = _db.CommonSequences.Count()
                },
                new SetupModel
                {
                    Name = "Session", Count = _db.SessionManager.Count()
                },
                new SetupModel
                {
                    Name = "GeneralLedgerCodes", Count = _db.GeneralLedgerCodes.Count()
                },
                new SetupModel
                {
                    Name = "MainGeneralLedgerCode", Count = _db.MainGeneralLedgerCodes.Count()
                },
                new SetupModel
                {
                    Name = "Services", Count = _db.ServiceConfig.Count()
                }
            };
            return Ok(setupstatus);

        }

    }

    public class SetupModel
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }
}