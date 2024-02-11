using Records;

List<Product> products = [
  new Product(1000, "Garden sheers", 14.60f),
  new Product(1001, "Common hoe", 89.99f),
];

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

WebApplication app = builder.Build();

app.UseCors(builder => builder
  .AllowAnyHeader()
  .AllowAnyMethod()
  .AllowAnyOrigin()
);
app.MapGet("/", () =>
{
  return products;
});
app.MapPost("/", (PartialProduct data) =>
{
  Product product = new(nextId(), data.Name, data.Price);
  products.Add(product);
  printAddition(product);
  return TypedResults.Created($"/{product.Id}", product);
});
app.MapPut("/{id}", (int id, Product data) =>
{
  Product? expired = products.FirstOrDefault(p => p.Id == id);
  if (expired == null) { return Results.NotFound(); }
  Product product = new(expired.Id, data.Name, data.Price);
  products.Remove(expired);
  products.Add(product);
  printUpdate(product);
  return TypedResults.Ok(product);
});
app.MapDelete("/{id}", (int id) =>
{
  Product? condemned = products.FirstOrDefault(p => p.Id == id);
  if (condemned == null) { return Results.NotFound(); }
  products.Remove(condemned);
  printRemoval();
  return TypedResults.NoContent();
});

app.Run();

void printStore()
{
  printSeparator();
  Console.WriteLine($"Valikoimassa on {products.Count} {(products.Count == 1 ? "tuote" : "tuotetta")}:");
  sortProducts();
  foreach (var product in products)
  {
    printProduct(product);
  }
  printSeparator();
}

void printAddition(Product product)
{
  printSeparator();
  Console.WriteLine("Uusi tuote lisätty:");
  printProduct(product);
  printStore();
}

void printUpdate(Product product)
{
  printSeparator();
  Console.WriteLine("Valikoimassa olevaa tuotetta on muutettu:");
  printProduct(product);
  printStore();
}

void printRemoval()
{
  printSeparator();
  Console.WriteLine("Tuote poistettu valikoimasta");
  printStore();
}

void printProduct(Product product)
{
  Console.WriteLine($"{product.Name}, hinta {product.Price} euroa");
}

void printSeparator()
{
  Console.WriteLine(new string('-', 20));
}

int nextId()
{
  return products.Max(p => p.Id) + 1;
}

void sortProducts()
{
  products.Sort((p1, p2) => p1.Id - p2.Id);
}

namespace Records
{
  public record Product(int Id, string Name, float Price);
  public record PartialProduct(string Name, float Price);
}