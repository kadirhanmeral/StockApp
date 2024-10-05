using System.Net;
using System.Security.Claims;
using App.Application.Contracts.Authorization;
using App.Application.Contracts.Persistence;
using App.Application.Contracts.Token;
using App.Application.Features.Users.Create;
using App.Application.Features.Users.Login;
using App.Application.Features.Users.Logout;
using App.Application.Features.Users.Profile;
using App.Application.Features.Users.Register;
using App.Domain.Entities;

namespace App.Application.Features.Users;

public class UserService(IUserRepository userRepository,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    IAuthorizationService authorizationService) : IUserService
{
    private string GenerateUsernameFromFullName(string fullName)
    {
        var nameParts = fullName.Split(' ');
        var firstName = nameParts[0];
        var lastName = nameParts.Length > 1 ? nameParts[1] : "";

        var guidPart = Guid.NewGuid().ToString().Substring(0, 8); 

        var username = $"{firstName}-{lastName}-{guidPart}";

        return username;
    }
    
    public async Task<ServiceResult> CreateUser(RegisterUserResponse user)
    {

        var existingUser = await userRepository.FindByEmailAsync(user.Email);
        if (existingUser.IsSuccess)
        {
            return  ServiceResult.Fail("Email address is already registered");
        }
        
        
        var userName = GenerateUsernameFromFullName(user.FullName);
        var nameExists = await userRepository.FindByNameAsync(userName);

        while (nameExists.IsSuccess)
        {
            userName = GenerateUsernameFromFullName(user.FullName);
            nameExists = await userRepository.FindByNameAsync(userName);
        }
        
        var appUser = new CreateNewUserRequest(
            UserName: userName,
            Email: user.Email,
            FullName: user.FullName
        );


        var result = await userRepository.CreateAsync(appUser, user.Password);
        
        if (result.IsFail)
        {
            return ServiceResult.Fail("Failed to create user");
        }

        return ServiceResult.Success(HttpStatusCode.Created);
    }
    
    public async Task<ServiceResult<object>> Login(LoginUserRequest model)
    {
        var user = await userRepository.FindByEmailAsync(model.Email);
        
        if (user.IsFail)
        {
            return ServiceResult<object>.Fail("Email address is invalid");
        }

        var result = await userRepository.CheckPasswordSignInAsync(model.Email, model.Password, false);
        
        if (result.IsSuccess)
        {
            return ServiceResult<object>.Success(new
            {
                token = tokenService.GenerateJwt(user.Data!)
            });
        }
        return ServiceResult<object>.Fail(result.ErrorMessages!);

    }
    
    public async Task<ServiceResult> Logout(TokenRequest token)
    {
        var tokenDb = await unitOfWork.BlackListedTokens.FirstOrDefaultAsync(x => x.Token == token.TokenStr);

        if (tokenDb != null)
        {
            return ServiceResult.Fail("Invalid token");
        }
        else
        {
            var tokenModel = new BlackListToken
            {
                Token = token.TokenStr,
                AddedDate = DateTime.Now
            };

            await unitOfWork.BlackListedTokens.AddAsync(tokenModel);
            await unitOfWork.SaveChangesAsync();
        }
        return ServiceResult.Success();
        

    }
    
    public async Task<ServiceResult<GetProfileResponse>> GetProfile()
    {

        var claim = authorizationService.FindFirstValue(ClaimTypes.NameIdentifier);
        
        
        if (claim.Result.Data == null)
        {
            return ServiceResult<GetProfileResponse>.Fail("Invalid token");
        }
        var userId = int.Parse(claim.Result.Data);

        var profile = new GetProfileResponse
        {
            FullName = authorizationService.FindFirstValue(ClaimTypes.Name).Result.Data,
            Email = authorizationService.FindFirstValue(ClaimTypes.Email).Result.Data,
            Data = new List<PortfolioDto>(),
            Transactions = new List<TransactionDto>()
        };
        var dataList = await unitOfWork.Portfolios.GetPortfolioByUserIdAsync(userId);

        var transactionList = await unitOfWork.TransactionHistories.GetTransactionsByUserIdAsync(userId);

        decimal totalProfit = 0;
        decimal historicalProfit = 0;
        decimal currentProfit = 0;

        decimal totalStockValue = 0;
        
        foreach (var stock in dataList)
        {
            decimal currentPrice;
            if (stock.Type=="Hisse")
            {
                currentPrice = await unitOfWork.StocksDetails.GetCurrentPriceAsync(stock.StockSymbol);
            }
            else
            {
                currentPrice = ((await unitOfWork.Currencies.FirstOrDefaultAsync(x=>x.Name == stock.StockSymbol))!).Sell;
            }

            if (stock.StockSymbol=="ons")
            {
                currentPrice *= ((await unitOfWork.Currencies.FirstOrDefaultAsync(x => x.Name == "usd"))!).Sell;
            }
            
            var profit = currentPrice * stock.Quantity - stock.BuyPrice * stock.Quantity;
            var value = currentPrice * stock.Quantity;
            var stockModel = new PortfolioDto
            {
                StockSymbol = stock.StockSymbol,
                BuyPrice = stock.BuyPrice,
                Quantity = stock.Quantity,
                CurrentPrice = currentPrice,
                StockProfit = profit,
                HistoricalStockProfit = stock.HistoricalProfit,
                StockValue = value
                
            };
            currentProfit += profit;
            historicalProfit += stock.HistoricalProfit;
            totalStockValue += value;
            profile.Data.Add(stockModel);
        }
        totalProfit = currentProfit+historicalProfit;
        
        foreach (var transaction in transactionList)
        {
            var tradeType = transaction.TradeType;
            string tradeTypeValue;

            if (tradeType == TradeType.Buy)
            {
                tradeTypeValue = "Buy";
            }
            else
            {
                tradeTypeValue = "Sell";
            }

            var transactionModel = new TransactionDto
            {
                StockSymbol = transaction.StockSymbol,
                Time = transaction.Time,
                Price = transaction.Price,
                Quantity = transaction.Quantity,
                TradeType = tradeTypeValue, // Using tradeTypeValue here
            };


            profile.Transactions.Add(transactionModel);
        }

        profile.TotalStockProfit = totalProfit;
        profile.TotalStockValue = totalStockValue;
        profile.CurrentStockProfit = currentProfit;
        profile.HistoricalStockProfit = historicalProfit;
        
        return  ServiceResult<GetProfileResponse>.Success(profile);

    }
    
}