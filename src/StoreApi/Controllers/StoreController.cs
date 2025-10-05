using Application.Dtos;
using Application.Features.Stores.CreateStore;
using Application.Features.Stores.DeleteStore;
using Application.Features.Stores.GetStore;
using Application.Features.Stores.GetStores;
using Application.Features.Stores.UpdateStore;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace StoreApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class StoreController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// Creates a new store.
        /// </summary>
        /// <param name="name">The name of the store.</param>
        /// <param name="address">The address of the store.</param>
        /// <param name="city">The city where the store is located.</param>
        /// <param name="country">The country where the store is located.</param>
        /// <param name="companyId">The ID of the company that owns the store.</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(StoreDto), 200)]
        public async Task<IActionResult> CreateStore([FromBody] CreateStoreCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(CreateStore), result);
        }

        /// <summary>
        /// Updates an existing store.
        /// </summary>
        /// <param name="id">The ID of the store.</param>
        /// <param name="name">The name of the store.</param>
        /// <param name="address">The address of the store.</param>
        /// <param name="city">The city where the store is located.</param>
        /// <param name="country">The country where the store is located.</param>
        /// <param name="companyId">The ID of the company that owns the store.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(StoreDto), 200)]
        public async Task<IActionResult> UpdateStore(Guid id, [FromBody] UpdateStoreCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Gets a store by its ID.
        /// </summary>
        /// <param name="id">The ID of the store.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DetailedStoreDto), 200)]
        public async Task<IActionResult> GetStore(Guid id)
        {
            var result = await _mediator.Send(new GetStoreQuery { Id = id });
            return Ok(result);
        }


        /// <summary>
        /// Get all stores.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StoreDto>), 200)]
        public async Task<IActionResult> GetAllStores()
        {
            var result = await _mediator.Send(new GetStoresQuery());
            return Ok(result);
        }


        /// <summary>
        /// Deletes a store by its ID.
        /// </summary>
        /// <param name="id">The ID of the store.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStore(Guid id)
        {
            await _mediator.Send(new DeleteStoreCommand { Id = id });
            return NoContent();
        }
    }
}