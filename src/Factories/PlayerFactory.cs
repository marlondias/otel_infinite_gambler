namespace InfiniteGambler.Factories;

using Bogus;
using InfiniteGambler.Entities;

public class PlayerFactory(Faker faker)
{
    private readonly Faker _faker = faker;

    public Player Create()
    {
        return new Player { Name = _faker.Name.FullName() };
    }
}
