namespace Catalog.API.Products.DeleteProduct
{
    public record DeleteProductRequest(Guid Id) : ICommand<DeleteProductResponse>;

    public record DeleteProductResponse(bool Success);

    public class DeleteProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/products/{id:guid}", async (Guid id, ISender sender) =>
            {
                var command = new DeleteProductCommand(id);
                var result = await sender.Send(command);
                var response = result.Adapt<DeleteProductResponse>();
                return Results.Ok(response);
            })
            .WithName("DeleteProduct")
            .Produces<DeleteProductResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithDescription("Deletes a product by its ID")
            .WithSummary("Deletes a product by its ID")
            .WithDisplayName("Delete Product");
        }
    }
}
