namespace Catalog.API.Exceptions
{
    public class ProductNotFoundException : Exception
    {
        public ProductNotFoundException(Guid productId)
            : base($"Product with ID '{productId}' was not found.")
        {
        }
        public ProductNotFoundException(string message)
            : base(message)
        {
        }
        public ProductNotFoundException()
            : base("Product not found!")
        {
        }
    }
}
