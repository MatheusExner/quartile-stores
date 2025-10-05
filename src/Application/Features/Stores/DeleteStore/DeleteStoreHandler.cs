using System.Net;
using Application.Common;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Stores.DeleteStore;

public class DeleteStoreHandler(IStoreRepository storeRepository, IUnitOfWork unitOfWork) : IRequestHandler<DeleteStoreCommand>
{
    private readonly IStoreRepository _storeRepository = storeRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <summary>
    /// Handles the deletion of a store by its ID.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="CustomException"></exception>
    public async Task Handle(DeleteStoreCommand request, CancellationToken cancellationToken)
    {
        var existingStore = await _storeRepository.GetByIdAsync(request.Id);

        if (existingStore == null)
        {
            throw new CustomException("Store not found.", HttpStatusCode.NotFound);
        }
        await _storeRepository.DeleteAsync(existingStore);
        await _unitOfWork.SaveChangesAsync();
    }
}
