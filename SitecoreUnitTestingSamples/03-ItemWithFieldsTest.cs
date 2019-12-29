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
    public class ItemWithFieldsTest
    {
        [Theory, DefaultAutoData]
        public void GetItem(Database db, Item item, Field field)
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
                    x.FromFactory(() => CreateItem(fixture))
                        .OmitAutoProperties());
            }

            private static Item CreateItem(ISpecimenBuilder fixture)
            {
                var item = Substitute.For<Item>(
                    fixture.Create<ID>(),
                    fixture.Create<ItemData>(),
                    fixture.Create<Database>());

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