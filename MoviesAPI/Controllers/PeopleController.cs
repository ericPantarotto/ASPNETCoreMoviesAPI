using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
    public class PeopleController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IFileStorageService fileStorageService;
        private const string container = "people";

        public PeopleController(ApplicationDbContext context,
                                IMapper mapper,
                                IFileStorageService fileStorageService)
        : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileStorageService = fileStorageService;
        }
        /// <summary>
        /// Get People list 
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(IEnumerable<PersonDTO>), 200)]
        [HttpGet(Name = "getPeople")]
        [ServiceFilter(typeof(PersonHATEOASAttribute))]
        public async Task<ActionResult<IEnumerable<PersonDTO>>> GetPerson([FromQuery] PaginationDTO pagination)
        {
            return await Get<Person, PersonDTO>();
        }
        
        /// <summary>
        /// get single person based on the id provided
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(PersonDTO), 200)]
        [ProducesResponseType(404)]
        // GET: api/People/5
        [HttpGet("{id:int}", Name = "getPerson")]
        [ServiceFilter(typeof(PersonHATEOASAttribute))]
        public async Task<ActionResult<PersonDTO>> GetPerson(int id)
        {
            return await Get<Person, PersonDTO>(id);
        }

        /// <summary>
        /// Create a new person in the dB
        /// </summary>
        /// <param name="personCreationDTO"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(PersonDTO), 200)]
        // POST: api/People
        [HttpPost(Name = "createPerson")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<PersonDTO>> PostPerson([FromForm] PersonCreationDTO personCreationDTO)
        {
            return await Post<PersonCreationDTO, Person, PersonDTO>(personCreationDTO, "getPerson", fileStorageService, container);            
        }

        /// <summary>
        /// Update an existing person
        /// </summary>
        /// <param name="id"></param>
        /// <param name="personUpdateDTO"></param>
        /// <returns></returns>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [HttpPut("{id:int}", Name = "putPerson")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> PutPerson(int id, [FromForm] PersonUpdateDTO personUpdateDTO)
        {
            Person personDb = await context.People.FirstOrDefaultAsync(x => x.Id == id);
            if (personDb is null) { return NotFound(); }

            mapper.Map(personUpdateDTO, personDb);
            await SetPicture(personUpdateDTO, personDb);
            await context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Patch an existing person based on JsonPatchDocument
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patchDocument"></param>
        /// <returns></returns>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [HttpPatch("{id:int}", Name = "patchPerson")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> PatchPerson(int id, [FromBody] JsonPatchDocument<PersonPatchDTO> patchDocument)
        {
            return await Patch<Person, PersonPatchDTO>(id, patchDocument);
        }

        /// <summary>
        /// delete an existing person in the dB
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        // DELETE: api/People/5
        [HttpDelete("{id}", Name = "deletePerson")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> DeletePerson(int id)
        {
            // Person person = await context.People.FindAsync(id);
            // if (person == null) { return NotFound(); }

            // await fileStorageService.DeleteFile(fileRoute: person.Picture, containerName: container);
            // context.People.Remove(person);
            // await context.SaveChangesAsync();

            // return NoContent();
            return await Delete<Person>(id, fileStorageService, container);
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
