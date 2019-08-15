using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [ApiController]
    [Route("")]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (celestialObject == null)
            {
                return NotFound();
            }

            var orbitingObject = _context.CelestialObjects.FirstOrDefault(x => x.OrbitedObjectId == celestialObject.Id);
            if (orbitingObject != null)
            {
                celestialObject.Satellites = new List<CelestialObject> {orbitingObject};
            }

            return Ok(celestialObject);
        }

        [HttpGet("{name}", Name = "GetByName")]
        public IActionResult GetByName(string name)
        {
            var celestialObject = _context.CelestialObjects.Where(x => x.Name == name).ToList();
            if (celestialObject.Count == 0)
            {
                return NotFound();
            }

            celestialObject.ForEach(x =>
                    x.Satellites = new List<CelestialObject>
                    {
                            _context.CelestialObjects.FirstOrDefault(k => k.OrbitedObjectId == x.Id)
                    });


            return Ok(celestialObject);
        }

        [HttpGet(Name = "GetAll")]
        public IActionResult GetAll()
        {
            var celestialObject = _context.CelestialObjects.ToList();
            celestialObject.ForEach(x =>
                    x.Satellites = new List<CelestialObject>
                    {
                            _context.CelestialObjects.FirstOrDefault(k => k.OrbitedObjectId == x.Id)
                    });
            return Ok(celestialObject);
        }

        [HttpPost("", Name = "Create")]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new {id = celestialObject.Id}, celestialObject);
        }
        
        [HttpPut("{id}", Name = "Update")]
        public IActionResult Update(int id, [FromBody] CelestialObject celestialObject)
        {
            var toUpdate = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (toUpdate == null)
            {
                return NotFound();
            }

            toUpdate.Name = celestialObject.Name;
            toUpdate.OrbitalPeriod = celestialObject.OrbitalPeriod;
            toUpdate.OrbitedObjectId = celestialObject.OrbitedObjectId;
            _context.CelestialObjects.Update(toUpdate);
            _context.SaveChanges();
            return NoContent();
        }
        
        [HttpPatch("{id}/{name}", Name = "RenameObject")]
        public IActionResult RenameObject(int id, string name)
        {
            var toUpdate = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (toUpdate == null)
            {
                return NotFound();
            }

            toUpdate.Name = name;
            _context.CelestialObjects.Update(toUpdate);
            _context.SaveChanges();
            return NoContent();
        }
        
        [HttpDelete("{id}", Name = "Delete")]
        public IActionResult Delete(int id)
        {
            var toDelete = _context.CelestialObjects.Where(x => x.Id == id || x.OrbitedObjectId == id).ToList();
            if (toDelete.Count == 0)
            {
                return NotFound();
            }
            _context.CelestialObjects.RemoveRange(toDelete);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
