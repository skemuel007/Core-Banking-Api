using AliasWebApiCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AliasWebApiCore.Models.Identity;
using Microsoft.AspNetCore.Authorization;

namespace AliasWebApiCore.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class RatingsController : Controller
    {
        private readonly AppDbContext _db;

        public RatingsController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/Ratings
        [HttpGet]
        public IQueryable<Rating> GetRatings()
        {
            return _db.Ratings;
        }

        // GET: api/Ratings/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetRating(int id)
        {
            Rating rating = await _db.Ratings.FindAsync(id);
            if (rating == null)
            {
                return NotFound();
            }

            return Ok(rating);
        }

        // PUT: api/Ratings/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutRating(int id,[FromBody] Rating rating)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != rating.RatingId)
            {
                return BadRequest();
            }

            rating.CreatedUserId = (_db.Ratings.Where(a => a.RatingId == id).Select(a => a.CreatedUserId)).FirstOrDefault();
            rating.CreatedDate = (_db.Ratings.Where(a => a.RatingId == id).Select(a => a.CreatedDate)).FirstOrDefault();
            _db.Entry(rating).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RatingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Ratings
        [HttpPost]
        public async Task<IActionResult> PostRating([FromBody] Rating rating)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.Ratings.Add(rating);
            await _db.SaveChangesAsync();

            return Ok(new { Status = "OK", Message = "Successfully Added", Output = "Rating has been added" });
        }

        // DELETE: api/Ratings/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteRating(int id)
        {
            Rating rating = await _db.Ratings.FindAsync(id);
            if (rating == null)
            {
                return NotFound();
            }

            _db.Ratings.Remove(rating);
            await _db.SaveChangesAsync();

            return Ok(rating);
        }

        [NonAction]
        private bool RatingExists(int id)
        {
            return _db.Ratings.Count(e => e.RatingId == id) > 0;
        }
    }
}