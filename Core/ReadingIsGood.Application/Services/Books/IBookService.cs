using System.Collections.Immutable;
using ReadingIsGood.Application.Models;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;

namespace ReadingIsGood.Application.Services.Books;

public interface IBookService
{
    ValueTask<bool> CreateBookAsync(BookModel book);
    ValueTask<BookModel> GetBookByIdAsync(Guid bookId);
    ValueTask<(ImmutableList<BookModel>, PaginationResult)> GetBookListAsync(int pageNumber = 1, int pageSize = 10);
    ValueTask<bool> DecreaseStockAsync(Guid bookId, int quantity, uint rowVersion);
}