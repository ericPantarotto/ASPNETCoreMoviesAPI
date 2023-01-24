using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Data;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IFileStorageService fileStorageService;
        private const string container = "people";

        public PeopleController(ApplicationDbContext context,
                                IMapper mapper,
                                IFileStorageService fileStorageService)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileStorageService = fileStorageService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonDTO>>> GetPerson()
        {
            List<Person> people = await context.People.ToListAsync();
            return mapper.Map<List<PersonDTO>>(people);
        }

        // GET: api/People/5
        [HttpGet("{id:int}", Name = "getPerson")]
        public async Task<ActionResult<PersonDTO>> GetPerson(int id)
        {
            var person = await context.People.FindAsync(id);

            if (person == null)
            {
                return NotFound();
            }

            return mapper.Map<PersonDTO>(person);
        }

        // POST: api/People
        [HttpPost]
        public async Task<ActionResult<PersonDTO>> PostPerson([FromForm] PersonCreationDTO personCreationDTO)
        {
            Person person = mapper.Map<Person>(personCreationDTO);

            if (personCreationDTO.Picture != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    personCreationDTO.Picture.CopyTo(memoryStream);
                    byte[] content = memoryStream.ToArray();
                    string extension = Path.GetExtension(path: personCreationDTO.Picture.FileName);
                    person.Picture = await fileStorageService.SaveFile(content: content,
                                                                       extension: extension,
                                                                       containerName: container,
                                                                       contentType: personCreationDTO.Picture.ContentType);
                }
                
            }

            context.People.Add(person);
            await context.SaveChangesAsync();

            PersonDTO personDTO = mapper.Map<PersonDTO>(person);
            return CreatedAtAction("getPerson", new { id = person.Id }, personDTO);
        }

        // PUT: api/People/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutPerson(int id, Person person)
        //{
        //    if (id != person.Id)
        //    {
        //        return BadRequest();
        //    }

        //    context.Entry(person).State = EntityState.Modified;

        //    try
        //    {
        //        await context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!PersonExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}


        // DELETE: api/People/5
        [HttpDelete("{id}")]
        //public async Task<ActionResult<Person>> DeletePerson(int id)
        //{
        //    var person = await context.People.FindAsync(id);
        //    if (person == null)
        //    {
        //        return NotFound();
        //    }

        //    context.People.Remove(person);
        //    await context.SaveChangesAsync();

        //    return person;
        //}

        private bool PersonExists(int id)
        {
            return context.People.Any(e => e.Id == id);
        }
    }
}
