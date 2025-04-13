﻿using System.Collections.Immutable;
using ReadingIsGood.Application.Models;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;

namespace ReadingIsGood.Application.Features.Orders.Query.GetOrdersByDateRange;

public sealed record GetOrdersByDateRangeQueryResponse(ImmutableList<OrderModel> Orders, PaginationResult Pagination);