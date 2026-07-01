namespace InfiniteGambler.Factories;

using Bogus;
using InfiniteGambler.Entities;

public class PlayerFactory
{
    public static Player Create()
    {
        var faker = new Faker();
        return new Player { Name = faker.Name.FullName() };
    }
}
