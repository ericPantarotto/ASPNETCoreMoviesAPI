using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Data;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;
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
        public async Task<ActionResult<IEnumerable<PersonDTO>>> Get([FromQuery] PaginationDTO pagination)
        {
            var queryable = context.People.AsQueryable();
            await HttpContext.InsertPaginationParametersInResponse(queryable, pagination.RecordsPerPage);
            //List<Person> people = await context.People.ToListAsync();
            List<Person> people  = await queryable.Paginate(pagination).ToListAsync();
            return mapper.Map<List<PersonDTO>>(people);
        }

        // GET: api/People/5
        [HttpGet("{id:int}", Name = "getPerson")]
        public async Task<ActionResult<PersonDTO>> Get(int id)
        {
            var person = await context.People.FindAsync(id);

            if (person is null) { return NotFound(); }

            return mapper.Map<PersonDTO>(person);
        }

        // POST: api/People
        [HttpPost]
        public async Task<ActionResult<PersonDTO>> PostPerson([FromForm] PersonCreationDTO personCreationDTO)
        {
            Person person = mapper.Map<Person>(personCreationDTO);
            await SetPicture(personCreationDTO, person);

            context.People.Add(person);
            await context.SaveChangesAsync();

            PersonDTO personDTO = mapper.Map<PersonDTO>(person);
            return CreatedAtAction("getPerson", new { id = person.Id }, personDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutPerson(int id, [FromForm] PersonUpdateDTO personUpdateDTO)
        {
            Person personDb = await context.People.FirstOrDefaultAsync(x => x.Id == id);
            if (personDb is null) { return NotFound(); }

            mapper.Map(personUpdateDTO, personDb);
            await SetPicture(personUpdateDTO, personDb);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> PatchPerson(int id, [FromBody] JsonPatchDocument<PersonPatchDTO> patchDocument)
        {
            if (patchDocument is null) { return BadRequest(); }

            Person personDb = await context.People.FirstOrDefaultAsync(x => x.Id == id);
            if (personDb == null) { return NotFound(); }

            var entityDto = mapper.Map<PersonPatchDTO>(personDb);
            patchDocument.ApplyTo(entityDto, ModelState);
            bool isValid = TryValidateModel(entityDto);
            if (!isValid) { return BadRequest(ModelState); }

            mapper.Map(entityDto, personDb);
            await context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/People/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePerson(int id)
        {
            Person person = await context.People.FindAsync(id);
            if (person == null) { return NotFound(); }

            await fileStorageService.DeleteFile(fileRoute: person.Picture, containerName: container);
            context.People.Remove(person);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private async Task SetPicture(PersonDTOBase personSource, Person personToUpdate)
        {
            if (personSource.Picture != null)
            {
                using var memoryStream = new MemoryStream();
                personSource.Picture.CopyTo(memoryStream);
                byte[] content = memoryStream.ToArray();
                string extension = Path.GetExtension(path: personSource.Picture.FileName);
                if (personSource is PersonCreationDTO)
                {
                    personToUpdate.Picture = await fileStorageService.SaveFile(content: content,
                                                                       extension: extension,
                                                                       containerName: container,
                                                                       contentType: personSource.Picture.ContentType);
                }
                else
                {
                    personToUpdate.Picture = await fileStorageService.EditFile(content: content,
                                                                      extension: extension,
                                                                      containerName: container,
                                                                      fileRoute: personToUpdate.Picture,
                                                                      contentType: personSource.Picture.ContentType);
                }
            }
        }
    }
}
