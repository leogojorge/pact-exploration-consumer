using Provider.Models;

namespace Provider
{
    public static class FakeDatabase
    {
        public static List<GetSomeDataByIdResponse> OurFakeData = FillOurFakeDatabase();

        private static List<GetSomeDataByIdResponse> FillOurFakeDatabase()
        {
            return new() {
                new()
                {
                    Id = new Guid("993a1ad5-7f7a-4a91-91fb-c0ee62755a2d"),
                    Name = "Name1",
                    LastName = "LastName1",
                    Age = "Age1",
                },
                new()
                {
                    Id = new Guid("8518eb80-c2c4-4c1f-8573-905af0d7f3e2"),
                    Name = "Name2",
                    LastName = "LastName2",
                    Age = "Age3",
                }
            };
        }
    }
}
