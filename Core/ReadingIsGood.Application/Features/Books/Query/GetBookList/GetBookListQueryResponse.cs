using System.Collections.Immutable;
using ReadingIsGood.Application.Models;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;

namespace ReadingIsGood.Application.Features.Books.Query.GetBookList;

public sealed record GetBookListQueryResponse(ImmutableList<BookModel> Books, PaginationResult Pagination);