using System.Linq;
using AutoFixture;
using AutoFixture.Xunit2;
using NSubstitute;
using Sitecore.Data;
using Sitecore.Data.Items;
using Xunit;

namespace SitecoreUnitTestingSamples
{
    public class NavigationRootResolver
    {
        public static readonly ID NavRootTemplateId = ID.NewID;

        public Item GetNavigationRoot(Item contextItem)
        {
            if (contextItem == null)
            {
                return null;
            }

            return contextItem.DescendsFrom(NavRootTemplateId)
                ? contextItem
                : contextItem.Axes.GetAncestors()
                    .LastOrDefault(x =>
                        x.DescendsFrom(NavRootTemplateId));
        }
    }

    public class AxesTest
    {
        [Theory, AutoData]
        public void GetNavigationRootWithNullReturnsNull(
            NavigationRootResolver sut)
        {
            var actual = sut.GetNavigationRoot(null);
            Assert.Null(actual);
        }

        [Theory, DefaultAutoData]
        public void GetNavigationRootReturnsItemDescendantFromRootTemplate(
            NavigationRootResolver sut,
            Item expected)
        {
            expected.DescendsFrom(NavigationRootResolver.NavRootTemplateId).Returns(true);
            var actual = sut.GetNavigationRoot(expected);
            Assert.Same(expected, actual);
        }

        [Theory, DefaultAutoData]
        public void GetNavigationRootReturnsLastAncestorDescendantFromRootTemplate(
            NavigationRootResolver sut,
            Item contextItem,
            Item ancestor1,
            Item expected)
        {
            ancestor1.DescendsFrom(NavigationRootResolver.NavRootTemplateId).Returns(true);
            expected.DescendsFrom(NavigationRootResolver.NavRootTemplateId).Returns(true);
            contextItem.Axes.GetAncestors().Returns(new[] { ancestor1, expected });

            var actual = sut.GetNavigationRoot(contextItem);

            Assert.Same(expected, actual);
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

                return item;
            }
        }
    }
}