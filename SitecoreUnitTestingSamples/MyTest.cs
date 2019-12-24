using AutoFixture;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using NSubstitute;
using Sitecore.Data;
using Sitecore.Data.Items;
using Xunit;

namespace SitecoreUnitTestingSamples
{
    public class MyTest
    {
        [Theory, DefaultAutoData]
        public void CreateDatabase(Database database)
        {
            Assert.NotNull(database);
        }

        [Theory, DefaultAutoData]
        public void CreateItem(Item item)
        {
            Assert.NotNull(item);
        }

        [Theory, DefaultAutoData]
        public void GetItem(Database db, Item item)
        {
            item["Title"].Returns("Welcome!");
            db.GetItem("/sitecore/content/home").Returns(item);
            Assert.Equal("Welcome!", item["Title"]);
        }
    }

    internal class DefaultAutoDataAttribute : AutoDataAttribute
    {
        public DefaultAutoDataAttribute()
            : base(() => new Fixture()
                .Customize(new DatabaseCustomization())
                .Customize(new ItemCustomization()))
        {
        }
    }

    internal class DatabaseCustomization : AutoFixture.ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<Database>(x =>
                x.FromFactory(() => Substitute.For<Database>())
                    .OmitAutoProperties());
        }
    }

    internal class ItemCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<Item>(x =>
                x.FromFactory(() => CreateItem(fixture))
                    .OmitAutoProperties()
            );
        }

        private static Item CreateItem(ISpecimenBuilder fixture)
        {
            var item = Substitute.For<Item>(
                fixture.Create<ID>(),
                fixture.Create<ItemData>(),
                fixture.Create<Database>());

            return item;
        }
    }
}