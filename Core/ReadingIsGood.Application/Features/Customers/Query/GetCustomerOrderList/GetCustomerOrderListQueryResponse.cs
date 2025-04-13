using System.Collections.Immutable;
using ReadingIsGood.Application.Models;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;

namespace ReadingIsGood.Application.Features.Customers.Query.GetCustomerOrderList;

public sealed record GetCustomerOrderListQueryResponse(ImmutableList<OrderModel> Orders, PaginationResult Pagination);