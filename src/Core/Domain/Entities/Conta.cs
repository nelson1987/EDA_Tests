namespace EDA.Domain.Entities;

public record Conta : IEntity
{
    public Conta(string name, string document)
    {
        Name = name;
        Document = document;
    }

    public Conta(Guid id, string name, string document) : this(name, document)
    {
        Id = id;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Document { get; private set; }
}
