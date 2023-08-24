using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestRESTAPI.Data;
using TestRESTAPI.Data.Models;

namespace TestRESTAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        public CategoriesController(AppDbContext db)
        {
            _db = db;
        }
        private readonly AppDbContext _db;

        // first endpoint
        [HttpGet]
        public async Task<IActionResult> GetCategory()
        {
            var cats = await _db.Categories.ToListAsync();
            return Ok(cats);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(string category)
        {
            Category c = new() { Name= category };
            await _db.Categories.AddAsync(c);
            _db.SaveChanges();
            return Ok(c);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCategory(Category category)
        {
            var c = await _db.Categories.SingleOrDefaultAsync(x => x.Id == category.Id);
            if(c==null)
            {
                return NotFound($"Category id {category.Id} not exists");
            }
            c.Name = category.Name;
            _db.SaveChanges();
            return Ok(c);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult>    //  FromRoute means take the id from the route (from the url)
            UpdateCategoryPatch([FromBody] JsonPatchDocument<Category> category, [FromRoute] int id)
        {
            var c = await _db.Categories.SingleOrDefaultAsync(x => x.Id == id);
            if(c==null)
            {
                return NotFound($"Category id {id} not exists");
            }
            category.ApplyTo(c);
            await _db.SaveChangesAsync();
            return Ok(c);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveCategory(int id)
        {
            var c = await _db.Categories.SingleOrDefaultAsync(x => x.Id == id);
            if (c == null)
            {
                return NotFound($"Category id {id} not exists");
            }
            _db.Categories.Remove(c);
            _db.SaveChanges();
            return Ok(c);
        }
    }
}
