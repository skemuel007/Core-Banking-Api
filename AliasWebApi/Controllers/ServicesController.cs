using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AliasWebApiCore.Models;
using AliasWebApiCore.Models.Identity;
using AliasWebApiCore.Services;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AliasWebApiCore.Controllers
{
    [Produces("application/json")]
    [Route("api/Services/configure")]
    public class ServicesController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IScheduler _scheduler;

        public ServicesController(AppDbContext db,IScheduler scheduler)
        {
            _db = db;
            _scheduler = scheduler;
        }

        [HttpPut]
        public async Task<IActionResult> ConfigureScheduler([FromBody] ServiceConfigModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var service = _db.ServiceConfig.FirstOrDefault(a => a.ServiceType.ToLower().Contains(model.ServiceName.ToLower()));
            if (service == null)
            {
                return BadRequest("Service does not exist");
            }
            service.Status = model.Status;
            service.StartDate = model.Date;
            service.Frequency = model.Frequency;
            service.ModifiedDate=DateTime.Now;
            service.ModifiedUserId = model.UserId;
            string cronexpression = null;
            switch (model.Frequency)
            {
                case "minutely":
                    cronexpression = Cron.MinuteInterval(model.Date.Minute);
                    break;
                case "hourly":
                    cronexpression = Cron.HourInterval(model.Date.Hour);
                    break;
                case "daily":
                    cronexpression = Cron.Daily(model.Date.Day);
                    break;
                case "dailyathourandminute":
                    cronexpression = Cron.Daily(model.Date.Hour, model.Date.Minute);
                    break;
                case "monthly":
                    cronexpression = Cron.MonthInterval(model.Date.Month);
                    break;
                case "monthlyonday":
                    cronexpression = Cron.Monthly(model.Date.Day);
                    break;
                case "monthlyondayathour":
                    cronexpression = Cron.Monthly(model.Date.Day, model.Date.Hour);
                    break;
                case "monthlyondayathourandminute":
                    cronexpression = Cron.Monthly(model.Date.Day, model.Date.Hour,model.Date.Minute);
                    break;
                case "weeklyondayathour":
                    cronexpression = Cron.Weekly(model.Date.DayOfWeek, model.Date.Hour);
                    break;
                case "weeklyondayathourandminute":
                    cronexpression = Cron.Weekly(model.Date.DayOfWeek, model.Date.Hour,model.Date.Minute);
                    break;
            }
            service.CronSchedule = cronexpression;

            service.CreatedUserId = (_db.ServiceConfig.Where(a => a.ServiceConfigId == service.CreatedUserId).Select(a => a.CreatedUserId)).FirstOrDefault();
            service.CreatedDate = (_db.ServiceConfig.Where(a => a.ServiceConfigId == service.CreatedUserId).Select(a => a.CreatedDate)).FirstOrDefault();
            _db.Entry(service).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            BackgroundJob.Enqueue(() => _scheduler.DiscoverServices(null));
            return Ok();
        }


    }

    public class ServiceConfigModel
    {
        public string ServiceName { get; set; }
        public bool Status { get; set; }
        public DateTime Date { get; set; }
        public string Frequency { get; set; }
        public int UserId { get; set; }
    }

}