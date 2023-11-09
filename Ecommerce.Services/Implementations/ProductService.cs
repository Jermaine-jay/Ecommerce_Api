using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Ecommerce.Data.Interfaces;
using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;
using Ecommerce.Models.Entities;
using Ecommerce.Models.Enums;
using Ecommerce.Services.Infrastructure;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace Ecommerce.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<ProductImage> _productImagesRepo;
        private readonly IRepository<ProductVariation> _productVariationRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly CloudinarySettings _cloudinary;


        public ProductService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, CloudinarySettings cloudinary)
        {
            _unitOfWork = unitOfWork;
            _categoryRepo = _unitOfWork.GetRepository<Category>();
            _productRepo = _unitOfWork.GetRepository<Product>();
            _productImagesRepo = _unitOfWork.GetRepository<ProductImage>();
            _productVariationRepo = _unitOfWork.GetRepository<ProductVariation>();
            _cloudinary = cloudinary;
        }


        public async Task<CreateProductResponse> CreateProduct(CreateProductRequest request)
        {
            var category = await _categoryRepo.GetSingleByAsync(p => p.Id.ToString() == request.CategoryId);
            if (category == null)
                throw new InvalidOperationException("Category doesn't exist");

            var product = await _productRepo.GetSingleByAsync(p => p.Name == request.Name.ToLower());
            if (product != null)
                throw new InvalidOperationException("Product Name already exist");


            var newProduct = new Product
            {
                Name = request.Name.ToLower(),
                Description = request.Description,
                CategoryId = category.Id,
            };

            await _productRepo.AddAsync(newProduct);
            return new CreateProductResponse
            {
                Message = "Product Created",
                Status = HttpStatusCode.Created,
                Data = newProduct,
            };
        }

        public async Task<ProductUpdateResponse> UpdateProduct(UpdateProductRequest request)
        {
            var product = await _productRepo.GetSingleByAsync(u => u.Id.ToString() == request.ProductId);
            if (product == null)
                throw new InvalidOperationException("Product does not exist");

            if(!request.CategoryName.IsNullOrEmpty() && request.CategoryName != "string")
            {
                var category = await _categoryRepo.GetSingleByAsync(p => p.Name == request.CategoryName.ToLower());
                if (category == null)
                    throw new InvalidOperationException("Category does not exist");

                product.CategoryId = category.Id;
            }


            product.Name = request.Name.ToLower() ?? product.Name;
            product.Description = request.Description ?? product.Description;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepo.UpdateAsync(product);
            return new ProductUpdateResponse
            {
                Name = product.Name, 
                Description = product.Description,
                CategoryId = product.CategoryId.ToString(),
                CategoryName = product.Category.Name
            };
        }

        public async Task<SuccessResponse> DeleteProduct(string productId)
        {
            var product = await _productRepo.GetSingleByAsync(u => u.Id.ToString() == productId, include: u => u.Include(p => p.ProductVariation));
            if (product == null)
                throw new InvalidOperationException("Product does not exist");

            var productVariation = await _productVariationRepo.GetAllAsync(include: u => u.Include(p => p.ProductImages));
            var allvar = productVariation.Where(u => u.ProductId.Equals(productId)).ToList();

            if (!allvar.Any())
                throw new InvalidOperationException("None Found");

            var cloudinary = new CloudinaryDotNet.Cloudinary(new Account(_cloudinary.CloudName, _cloudinary.ApiKey, _cloudinary.ApiSecret));

            var deletionParamsList = allvar.SelectMany(item => item.ProductImages.Select(image => new DeletionParams(image.PublicId))).ToList();
            await Task.WhenAll(deletionParamsList.Select(param => cloudinary.DestroyAsync(param)));

            await _productRepo.DeleteAsync(product);
            return new SuccessResponse { Success = true };
        }

        public async Task<SuccessResponse> UpdateStock(string productvariationId, int stock)
        {
            var productv = await _productVariationRepo.GetSingleByAsync(u => u.Id.ToString() == productvariationId);
            if (productv == null)
                throw new InvalidOperationException("Product does not exist");

            productv.StockQuantity = stock;

            await _productVariationRepo.UpdateAsync(productv);
            return new SuccessResponse
            {
                Success = true,
                Data = productv.StockQuantity
            };
        }

        public async Task<object> AddImages(Guid productId, List<IFormFile> files)
        {
            if (files == null || !files.Any())
                throw new InvalidOperationException("File cannot be empty");

            var cloudinary = new Cloudinary(new Account(_cloudinary.CloudName, _cloudinary.ApiKey, _cloudinary.ApiSecret));
            if (cloudinary == null)
                throw new InvalidOperationException("Invalid Cloud parameters");

            var uploadResults = new List<ImageUploadResult>();
            foreach (var file in files)
            {
                if (file.Length <= 0)
                    continue;

                await using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill")
                };
                var result = await cloudinary.UploadAsync(uploadParams);
                uploadResults.Add(result);

                var image = new ProductImage
                {
                    ProductVariationId = productId,
                    PublicId = result.PublicId,
                    Format = result.Format,
                    CreatedAt = result.CreatedAt,
                    Bytes = result.Bytes,
                    Url = result.Url.ToString(),
                    SecureUrl = result.SecureUrl.ToString()
                };

                await _productImagesRepo.AddAsync(image);
            }
            return uploadResults;
        }

        public async Task<SuccessResponse> AddVariations(ProductVarionRequest request)
        {
            var product = await _productRepo.GetSingleByAsync(p => p.Id.ToString() == request.ProductId, include: u=> u.Include(u=>u.ProductVariation))
                ??throw new InvalidOperationException("Product does not exist");

            var colour = Colour.AsDisplayed;
            switch (request.Colour)
            {
                case (int)Colour.Blue:
                    colour = Colour.Blue;
                    break;

                case (int)Colour.Yellow:
                    colour = Colour.Yellow;
                    break;

                case (int)Colour.Green:
                    colour = Colour.Green;
                    break;

                case (int)Colour.Red:
                    colour = Colour.Green;
                    break;

                case (int)Colour.White:
                    colour = Colour.White;
                    break;

                case (int)Colour.Purple:
                    colour = Colour.Purple;
                    break;
            }

            var newVar = new ProductVariation
            {
                Id = Guid.NewGuid(),
                Colour = colour,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                ProductId = product.Id,
            };

            await _productVariationRepo.AddAsync(newVar);
            if(request.files != null)
            {
                var images = await AddImages(newVar.Id, request.files);
            }
            return new SuccessResponse
            {
                Success = true,
                Data = newVar
            };
        }

        public async Task<SuccessResponse> DeleteImage(string publicId)
        {
            var image = await _productImagesRepo.GetSingleByAsync(image => image.PublicId == publicId, include: u => u.Include(p => p.ProductVariation));
            if (image != null)
                throw new InvalidOperationException("Image does not exist");

            var cloudinary = new CloudinaryDotNet.Cloudinary(new Account(_cloudinary.CloudName, _cloudinary.ApiKey, _cloudinary.ApiSecret));
            var param = new DeletionParams(image.PublicId) { };

            await cloudinary.DestroyAsync(param);
            await _productImagesRepo.DeleteAsync(image);

            return new SuccessResponse { Success = true };
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            var products = await _productRepo.GetAllAsync(include: u => u.Include(p => p.ProductVariation));
            if (products == null)
                throw new InvalidOperationException("No Product Found");


            return products.OrderByDescending(u => u.Name).Select(product => new Product
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                ProductVariation = product.ProductVariation.Select(pv => new ProductVariation
                {
                    Colour = pv.Colour,
                    StockQuantity = pv.StockQuantity,
                    Price = pv.Price,
                    ProductImages = pv.ProductImages.Select(pi => new ProductImage
                    {
                        Url = pi.Url,
                        SecureUrl = pi.SecureUrl,
                    }).ToList(),
                }).ToList(),
            });

        }

        public async Task<ProductDto> GetProductAsync(string productId)
        {
            var product = await _productRepo.GetSingleByAsync(pro => pro.Id.ToString() == productId, include: u => u.Include(p => p.ProductVariation))
                ??throw new InvalidOperationException("No Product Found");

            var variation = product.ProductVariation.FirstOrDefault()
                ??throw new InvalidOperationException("No Product variation Found");

            return new ProductDto
            {
                Name = product.Name,
                Description = product.Description,
                Price = variation.Price,
                StockQuantity = variation.StockQuantity,
                Colour = variation.Colour.GetStringValue(),
                ImageUrl = variation.ProductImages.First().Url,
                ImageSecureUrl = variation.ProductImages.First().SecureUrl
            };
        }

    }
}

