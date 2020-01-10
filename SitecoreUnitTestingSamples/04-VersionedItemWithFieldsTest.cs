using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using NSubstitute;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Xunit;

namespace SitecoreUnitTestingSamples
{
    public class VersionedItemWithFieldsTest
    {
        [Theory, DefaultAutoData]
        public void GetFieldValue(Database db, Item item, Field field)
        {
            field.Value.Returns("Welcome!");
            item.Fields["Title"].Returns(field);
            db.GetItem("/sitecore/content/home").Returns(item);
            Assert.Equal("Welcome!", item.Fields["Title"].Value);
        }

        internal class DefaultAutoDataAttribute : AutoDataAttribute
        {
            public DefaultAutoDataAttribute()
                : base(() => new Fixture()
                    .Customize(new DatabaseCustomization())
                    .Customize(new ItemCustomization())
                    .Customize(new FieldCustomization()))
            {
            }
        }

        internal class DatabaseCustomization : ICustomization
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
                    x.FromFactory<ID, ItemData, Database>(CreateItem)
                        .OmitAutoProperties());
            }

            private static Item CreateItem(ID id, ItemData itemData, Database database)
            {
                var item = Substitute.For<Item>(id, itemData, database);
                item.Axes.Returns(Substitute.For<ItemAxes>(item));
                item.Fields.Returns(Substitute.For<FieldCollection>(item));
                return item;
            }
        }

        internal class FieldCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Customize<Field>(x =>
                    x.FromFactory(
                        new MethodInvoker(
                            new NSubstituteMethodQuery())));
            }
        }
    }
}