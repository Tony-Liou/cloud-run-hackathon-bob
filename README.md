# Cloud Run Hackathon CSharp

To make changes, edit the lambda expression used by `app.MapPost()` in [Program.cs](https://github.com/Tony-Liou/cloud-run-hackathon-csharp/blob/d2f1c7af7048c176ababf59fc632bc4d4c1c7a41/Program.cs#L8-L15) file.

## Usage

Use `dotnet run` command to start the Kestrel server.
Copy the URL printed in the terminal (e.g., `http://localhost:8080`) and paste it into a web browser, then you will get "*Let the battle begin!*".

## Tips

For those who are not familiar with [record](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record) type.

The following line

```c#
record PositionalRecord([property: MyMagicAttribute] bool isAmaZin9, string Comment);
```

is very similar to

```c#
class PositionalRecord
{
    [MyMagicAttribute]
    public bool isAmaZin9 { get; init; }
    
    public string Comment { get; init; }
    
    // methods...
}
```

So **do not** modify the properties in `record` after the `record` object is instantiated.
