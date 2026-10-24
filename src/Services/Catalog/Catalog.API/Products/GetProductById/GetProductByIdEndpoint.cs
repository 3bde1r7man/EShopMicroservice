namespace Catalog.API.Products.GetProductById
{
    public record GetProductByIdRequest(Guid ProductId) : IRequest<GetProductByIdResponse>;

    public record GetProductByIdResponse(Product Product);

    public class GetProductByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products/{productId:guid}", async (Guid productId, ISender sender) =>
            {
                var query = new GetProductByIdQuery(productId);
                try
                {
                    var result = await sender.Send(query);
                    var response = result.Adapt<GetProductByIdResponse>();
                    return Results.Ok(response);
                }
                catch (ProductNotFoundException e)
                {
                    return Results.NotFound(e.Message);
                }
            })
            .WithName("GetProductById")
            .Produces<GetProductByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithDescription("Retrieves a product by its ID")
            .WithSummary("Retrieves a product by its ID")
            .WithDisplayName("Get Product By ID");

        }
    }
}
