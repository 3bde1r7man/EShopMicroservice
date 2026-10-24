namespace Catalog.API.Products.GetProductByCategory
{

    public record GetProductByCategoryRequest(string CategoryName);

    public record GetProductByCategoryResponse(IEnumerable<Product> Products);

    public class GetProductByCategoryEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products/category/{categoryName}", async (string categoryName, ISender sender) =>
            {
                var query = new GetProductByCategoryQuery(categoryName);
                var result = await sender.Send(query, CancellationToken.None);
                var response = result.Adapt<GetProductByCategoryResponse>();
                return Results.Ok(response);
            })
            .WithName("GetProductByCategory")
            .Produces<GetProductByCategoryResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get products by category")
            .WithDescription("Get products by category")
            .WithDisplayName("Get products by category");



        }
    }
}
