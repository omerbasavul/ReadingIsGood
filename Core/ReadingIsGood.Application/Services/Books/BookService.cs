using System.Collections.Immutable;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ReadingIsGood.Application.Models;
using ReadingIsGood.BuildingBlocks.Common.CustomException;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;
using ReadingIsGood.BuildingBlocks.EntityFrameworkCore.Infrastructure.Repositories;
using ReadingIsGood.Domain.Entities;

namespace ReadingIsGood.Application.Services.Books;

public sealed class BookService(
    IBaseRepository<Book, Guid> bookRepository,
    IMapper mapper) : IBookService
{
    public async ValueTask<bool> CreateBookAsync(BookModel book)
    {
        var entity = mapper.Map<Book>(book);

        var exist = await bookRepository.IsExistAsync(x => x.ISBN == book.ISBN);
        if (exist) throw new CustomServiceException("Book with this ISBN already exists", StatusCodes.Status409Conflict);

        var result = await bookRepository.AddAsync(entity);

        if (result > 0)
            return true;

        throw new CustomServiceException($"Could not create book '{book.Title}'.", StatusCodes.Status500InternalServerError);
    }

    public async ValueTask<BookModel> GetBookByIdAsync(Guid bookId)
    {
        var bookEntity = await bookRepository.GetByIdAsync(bookId, tracking: false);
        if (bookEntity == null)
            throw new CustomServiceException("Book not found", StatusCodes.Status404NotFound);

        var bookModel = mapper.Map<BookModel>(bookEntity);
        return bookModel;
    }

    public async ValueTask<(ImmutableList<BookModel>, PaginationResult)> GetBookListAsync(int pageNumber = 1, int pageSize = 10)
    {
        var (entities, pagination) = await bookRepository.GetListAsync(pageNumber: pageNumber, pageSize: pageSize, tracking: false);

        var list = mapper.Map<List<BookModel>>(entities);
        return (list.ToImmutableList(), pagination);
    }

    public async ValueTask<bool> DecreaseStockAsync(Guid bookId, int quantity, uint rowVersion)
    {
        var existing = await bookRepository.GetByIdAsync(bookId, tracking: true);

        if (existing == null)
            throw new CustomServiceException("Book not found", StatusCodes.Status404NotFound);

        if (existing.StockQuantity < quantity)
            throw new CustomServiceException("Not enough stock", StatusCodes.Status400BadRequest);

        bookRepository.SetOriginalRowVersion(existing, rowVersion);

        existing.StockQuantity -= quantity;

        var updated = false;
        try
        {
            updated = await bookRepository.UpdateAsync(existing);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new CustomServiceException("This book was updated by another user. Please refresh and try again.", StatusCodes.Status409Conflict);
        }

        return updated;
    }
}