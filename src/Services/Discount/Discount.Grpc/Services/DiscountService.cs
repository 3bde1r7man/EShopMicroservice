using Discount.Grpc.Data;
using Discount.Grpc.Models;
using Grpc.Core;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Services
{
    // TODO: invoke the Database to get the coupon for the product
    public class DiscountService(DiscountContext dbContext, ILogger<DiscountService> logger)
        : DiscountProtoService.DiscountProtoServiceBase
    {

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await dbContext.Coupons.FirstOrDefaultAsync(c => c.ProductName == request.ProductName);
            if (coupon == null)
                coupon = new Coupon { ProductName = request.ProductName, Amount = 0, Description = "No discount available" };
            
            logger.LogInformation("Discount retrieved for ProductName: {ProductName}, Amount: {Amount}", coupon.ProductName, coupon.Amount);

            var couponModel = coupon.Adapt<CouponModel>();
            logger.LogInformation("the coupon modle data is ProductName: {ProductName}, Amount: {Amount}, Description: {Description}", couponModel.ProductName, couponModel.Amount, couponModel.Description);
            return couponModel; 

        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = request.Coupon.Adapt<Coupon>();
            if (coupon is null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid coupon data provided."));

            dbContext.Coupons.Add(coupon);
            await dbContext.SaveChangesAsync();

            var couponModel = coupon.Adapt<CouponModel>();

            logger.LogInformation("Discount created for ProductName: {ProductName}, Amount: {Amount}, Description: {Description}", couponModel.ProductName, couponModel.Amount, couponModel.Description);
            return couponModel;
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = request.Coupon.Adapt<Coupon>();
            var existingCoupon = await dbContext.Coupons.FirstOrDefaultAsync(c => c.ProductName == coupon.ProductName);
            if (existingCoupon != null)
            {
                existingCoupon.Amount = coupon.Amount;
                existingCoupon.Description = coupon.Description;
                await dbContext.SaveChangesAsync();

                logger.LogInformation("Discount updated for ProductName: {ProductName}, Amount: {Amount}, Description: {Description}", existingCoupon.ProductName, existingCoupon.Amount, existingCoupon.Description);
                
                return existingCoupon.Adapt<CouponModel>();
            }
            else
            {
                logger.LogWarning("Discount for ProductName: {ProductName} not found.", coupon.ProductName);
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount for ProductName: {coupon.ProductName} not found."));
            }

        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var coupon = await dbContext.Coupons.FirstOrDefaultAsync(c => c.ProductName == request.ProductName);
            if (coupon != null)
            {
                dbContext.Coupons.Remove(coupon);
                await dbContext.SaveChangesAsync();
                logger.LogInformation("Discount deleted for ProductName: {ProductName}", coupon.ProductName);
            }

            return new DeleteDiscountResponse { Success = true };
        }

    }
}
