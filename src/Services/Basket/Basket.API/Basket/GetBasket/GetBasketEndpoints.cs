namespace Basket.API.Basket.GetBasket
{
    public record GetBasketRequest(string UserName) : IQuery<GetBasketResponse>;

    public record GetBasketResponse(ShoppingCart Cart);
    public class GetBasketEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("basket/{UserName}", async (string UserName, ISender sender) =>
            {
                var query = new GetBasketQuery(UserName);
                var result = await sender.Send(query);
                var response = result.Adapt<GetBasketResponse>();
                return Results.Ok(response);
            })
            .WithName("GetBasket")
            .Produces<GetBasketResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithDescription("Get the shopping cart for a specific user by their username.")
            .WithSummary("Retrieves the shopping cart for a user.")
            .WithDisplayName("Get User Basket");
        }
       
    }
}
