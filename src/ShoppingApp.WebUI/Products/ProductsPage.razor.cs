using Microsoft.AspNetCore.Components;
using MudBlazor;
using ShoppingApp.Abstractions;
using ShoppingApp.WebUI.Extensions;
using ShoppingApp.WebUI.Services;

namespace ShoppingApp.WebUI.Products;

public sealed partial class ProductsPage
{
    private HashSet<ProductDetails>? _products;
    private ManageProductModal? _modal;

    [Parameter]
    public string? Id { get; set; }

    [Inject]
    public InventoryService InventoryService { get; set; } = null!;

    [Inject]
    public ProductService ProductService { get; set; } = null!;

    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    protected override async Task OnInitializedAsync() =>
        _products = await InventoryService.GetAllProductsAsync();

    private async Task CreateNewProduct()
    {
        if (_modal is not null)
        {
            var product = new ProductDetails();
            var faker = product.GetBogusFaker();
            var fake = faker.Generate();
            
            _modal.Product = product with
            {
                Id = fake.Id,
                ImageUrl = fake.ImageUrl,
                DetailsUrl = fake.DetailsUrl
            };
            
            await _modal.OpenAsync("Create Product", OnProductUpdated);
        }
    }

    private async Task OnProductUpdated(ProductDetails product)
    {
        await ProductService.CreateOrUpdateProductAsync(product);
        _products = await InventoryService.GetAllProductsAsync();

        _modal?.Close();

        StateHasChanged();
    }

    private Task OnEditProduct(ProductDetails product) => OnProductUpdated(product);
}