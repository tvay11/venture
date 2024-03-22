using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using stock.Model.DTO;
using stock.Model.Entity;

namespace stock.Controllers;

public partial class StockController
{
    [HttpGet("stockprices")]
    public async Task<ActionResult<IEnumerable<StockPrice>>> GetAllStockPrices()
    {
        var stockPrices = await _context.StockPrices.ToListAsync();
        if (stockPrices == null || !stockPrices.Any())
        {
            return NotFound("No stock prices found.");
        }

        return Ok(stockPrices);
    }

    [HttpGet("stocklist")]
    public async Task<ActionResult<IEnumerable<Stock>>> GetAllStocks()
    {
        var stocks = await _context.Stocks.ToListAsync();
        if (stocks == null || !stocks.Any())
        {
            return NotFound("No stocks found.");
        }

        return Ok(stocks);
    }

            [EnableCors()]
            [HttpGet("user/{userId}/profile")]
            public async Task<ActionResult<UserProfileDTO>> GetProfile(int userId)
            {
                var userProfile = await _context.UserProfiles
                    .Include(up => up.StockHoldings)
                    .ThenInclude(sh => sh.Stock)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(up => up.UserId == userId);

                if (userProfile == null)
                {
                    return NotFound();
                }

                var userProfileDTO = new UserProfileDTO
                {
                    UserId = userProfile.UserId,
                    Cash = userProfile.Cash,
                    NetWorth = userProfile.NetWorth,
                    CurrentDate = userProfile.CurrentDate,
                };

                return Ok(userProfileDTO);
            }

            [HttpGet("/stock/{id}")]
            public async Task<ActionResult<StockResponseDto>> GetStock(int id)
            {
                var stock = await _context.Stocks
                    .Include(s => s.StockPrices)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (stock == null)
                {
                    return NotFound();
                }

                var userProfile = await _context.UserProfiles.FindAsync(1);
                if (userProfile == null)
                {
                    return NotFound("User profile not found.");
                }

                var responseDto = _stockService.CreateStockResponseDto(stock, userProfile);
                return Ok(responseDto);
            }
            
            [HttpGet("user/{userId}/holdings")]
            public async Task<ActionResult<IEnumerable<StockHoldingDTO>>> GetUserStockHoldings(int userId)
            {
                var currentDate = GetCurrentDateForUser(1);

                var userHoldings = await _context.StockHoldings
                    .Where(sh => sh.UserId == userId)
                    .Include(sh => sh.Stock)
                    .ThenInclude(s => s.StockPrices)
                    .Select(sh => new StockHoldingDTO
                    {
                        StockName = sh.Stock.Name,
                        Quantity = sh.Quantity,
                        Price = sh.Stock.StockPrices
                            .Where(sp => sp.Date <= currentDate) 
                            .OrderByDescending(sp => sp.Date) 
                            .Select(sp => sp.Price)
                            .FirstOrDefault() 
                    })
                    .ToListAsync();

                if (!userHoldings.Any())
                {
                    return NotFound($"No stock holdings found for user with ID {userId}.");
                }

                return Ok(userHoldings);
            }
            
            [HttpGet("search/{keyword}")]
            public async Task<ActionResult<IEnumerable<StockSearchResultDto>>> SearchStocks(string keyword)
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return BadRequest("Search keyword is required.");
                }

                var currentDate = (await _context.UserProfiles.FindAsync(1))?.CurrentDate ?? DateTime.Today;

                var matchingStocks = await _context.Stocks
                    .Where(s => s.Symbol.Contains(keyword) || s.Name.Contains(keyword))
                    .Select(s => new
                    {
                        StockId = s.Id,
                        Symbol = s.Symbol,
                        Name = s.Name,
                        CurrentPrice = s.StockPrices
                            .Where(sp => sp.Date <= currentDate)
                            .OrderByDescending(sp => sp.Date)
                            .Select(sp => sp.Price)
                            .FirstOrDefault()
                    })
                    .ToListAsync();

                var searchResults = matchingStocks.Select(s => new StockSearchResultDto
                {
                    StockId = s.StockId,
                    Symbol = s.Symbol,
                    Name = s.Name,
                    CurrentPrice = s.CurrentPrice
                }).ToList();

                if (!searchResults.Any())
                {
                    return NotFound("No matching stocks found.");
                }

                return Ok(searchResults);
            }


            



            
            [HttpGet("user/{userId}/stockinfo")]
            public async Task<ActionResult<IEnumerable<StockInfoDto>>> GetStocksWithCurrentPrices(int userId)
            {
                var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId);
                if (userProfile == null) return NotFound("UserProfile not found.");

                var currentDate = userProfile.CurrentDate;

                var stocksWithPrices = await _context.StockHoldings
                    .Where(sh => sh.UserId == userId)
                    .Include(sh => sh.Stock)
                    .ThenInclude(s => s.StockPrices)
                    .Select(sh => new StockInfoDto
                    {
                        StockId = sh.Stock.Id, 
                        Symbol = sh.Stock.Symbol,
                        Name = sh.Stock.Name,
                        Quantity = sh.Quantity,
                        CurrentPrice = sh.Stock.StockPrices
                            .Where(sp => sp.Date <= currentDate)
                            .OrderByDescending(sp => sp.Date)
                            .FirstOrDefault().Price, 
                        CurrentDate = currentDate 
                    }).ToListAsync();

                return Ok(stocksWithPrices);
            }

            [HttpGet("user/{userId}/history")]
            public async Task<ActionResult<IEnumerable<UserHistoryDto>>> GetUserHistory(int userId)
            {
                var userHistory = await _context.UserHistories
                    .Where(uh => uh.UserId == userId)
                    .Select(uh => new UserHistoryDto 
                    {
                        Id = uh.Id,
                        UserId = uh.UserId,
                        Date = uh.Date,
                        NetWorth = uh.NetWorth
                    })
                    .ToListAsync();

                if (userHistory == null)
                {
                    return NotFound();
                }

                return userHistory;
            }
            
}