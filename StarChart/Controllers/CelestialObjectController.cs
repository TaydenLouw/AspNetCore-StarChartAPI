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

            celestialObject.ForEach(x => x.Satellites = new List<CelestialObject> {_context.CelestialObjects.FirstOrDefault(k => k.OrbitedObjectId == x.Id)});


            return Ok(celestialObject);
        }
        
        [HttpGet( Name = "GetAll")]
        public IActionResult GetAll()
        {
            var celestialObject = _context.CelestialObjects.ToList();
            celestialObject.ForEach(x => x.Satellites = new List<CelestialObject> {_context.CelestialObjects.FirstOrDefault(k => k.OrbitedObjectId == x.Id)});
            return Ok(celestialObject);
        }
    }
}
