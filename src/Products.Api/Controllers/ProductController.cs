using Microsoft.AspNetCore.Mvc;
using Products.Application.Interfaces.Managers;
using Products.Application.Models.RequestModels;

namespace Products.Api.Controllers;

[Route("api/products")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;
    private readonly IProductManager _productManager;
    public ProductController(
        IProductManager productManager,
        ILogger<ProductController> logger
    )
    {
        _productManager = productManager;
        _logger = logger;
    }

    /// <summary>
    /// Get products
    /// </summary>
    /// <remarks>Get all products</remarks>
    /// <response code="200">Successful operation</response>
    /// <response code="400">Bad request</response>
    /// <response code="500">Internal server error</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet]
    public async Task<ActionResult> GetProducts(CancellationToken cancellationToken)
    {
        _logger.LogDebug("GetProducts called in ProductController");

        var result = await _productManager.GetAllProductsAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    /// <remarks>Get product based on ID</remarks>
    /// <param name="id">Product ID</param>
    /// <response code="200">Successful operation</response>
    /// <response code="400">Bad request</response>
    /// <response code="500">Internal server error</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet, Route("{id}")]
    public async Task<ActionResult> GetProductsById(int id, CancellationToken cancellationToken)
    {
        _logger.LogDebug("GetProductsById called in ProductController");

        var result = await _productManager.GetProductByIdAsync(id, cancellationToken);
        return Ok(result);            
    }

    /// <summary>
    /// Add products
    /// </summary>
    /// <remarks>Create products based on below details</remarks>
    /// <param name="products">Products details</param>
    /// <response code="200">Successful operation</response>
    /// <response code="400">Bad request</response>
    /// <response code="500">Internal server error</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost]
    public async Task<ActionResult> CreateProducts([FromBody] List<ProductRequestModel> products, CancellationToken cancellationToken)
    {
        _logger.LogDebug("CreateProducts called in ProductController");

        var result = await _productManager.AddProductsAsync(products, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Update product
    /// </summary>
    /// <remarks>Update product based on below details</remarks>
    /// <param name="id">Product ID</param>
    /// <param name="product">Product details</param>
    /// <response code="200">Successful operation</response>
    /// <response code="400">Bad request</response>
    /// <response code="500">Internal server error</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPut, Route("{id}")]
    public async Task<ActionResult> UpdateProduct(int id, [FromBody] ProductRequestModel product, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("UpdateProduct called in ProductController");

        var result = await _productManager.UpdateProductAsync(id, product, cancellationToken);
        return Ok(result);            
    }

    /// <summary>
    /// Decrement product stock
    /// </summary>
    /// <remarks>Decrement product stock based on quantity</remarks>
    /// <param name="id">Product ID</param>
    /// <param name="quantity">Product quantity</param>
    /// <response code="200">Successful operation</response>
    /// <response code="400">Bad request</response>
    /// <response code="500">Internal server error</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPut, Route("decrement-stock/{id}/{quantity}")]
    public async Task<ActionResult> DecrementProductStock(int id, int quantity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("DecrementProductStock called in ProductController");

        var result = await _productManager.DecrementProductStock(id, quantity, cancellationToken);
        return Ok(result);            
    }

    /// <summary>
    /// Add to stock for a product
    /// </summary>
    /// <remarks>Increment product stock based on quantity</remarks>
    /// <param name="id">Product ID</param>
    /// <param name="quantity">Product quantity</param>
    /// <response code="200">Successful operation</response>
    /// <response code="400">Bad request</response>
    /// <response code="500">Internal server error</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPut, Route("add-to-stock/{id}/{quantity}")]
    public async Task<ActionResult> IncrementProductStock(int id, int quantity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("IncrementProductStock called in ProductController");

        var result = await _productManager.IncrementProductStock(id, quantity, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    /// <remarks>Delete a product</remarks>
    /// <param name="id">Product ID</param>
    /// <response code="204">Successful operation</response>
    /// <response code="400">Bad request</response>
    /// <response code="500">Internal server error</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpDelete, Route("{id}")]
    public async Task<ActionResult> DeleteProduct(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("DeleteProduct called in ProductController");

        var result = await _productManager.DeleteProductAsync(id, cancellationToken);
        if (result)
            return NoContent();

        return NotFound();
    }
}
