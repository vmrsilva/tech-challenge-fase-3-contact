using Moq;
using Newtonsoft.Json;
using TechChallenge.Domain.Cache;
using TechChallenge.Infrastructure.Cache;

namespace TechChallenge.Tests.Infrastructure.Cache
{
    public class CacheTest
    {
        private readonly Mock<ICacheWrapper> _cacheMock;
        private readonly CacheRepository _cacheRepository;

        public CacheTest()
        {
            _cacheMock = new Mock<ICacheWrapper>();
            _cacheRepository = new CacheRepository(_cacheMock.Object);
        }

        //[Fact(DisplayName = "Should Return Value From Cache When Its Exist")]
        //public async Task ShouldReturnValueFromCacheWhenItsExist()
        //{
        //    var key = "test-key";
        //    var cachedValue = new RegionEntity("Test", "11");
        //    var serializedValue = JsonConvert.SerializeObject(cachedValue);

        //    var cacheWrapperMock = new Mock<ICacheWrapper>();
        //    cacheWrapperMock.Setup(x => x.GetStringAsync(key, default)).ReturnsAsync(serializedValue);

        //    var cacheRepository = new CacheRepository(cacheWrapperMock.Object);

        //    var result = await cacheRepository.GetAsync(key, () => Task.FromResult(default(RegionEntity)));

        //    Assert.NotNull(result);
        //    Assert.Equal(cachedValue.Name, result.Name);
        //    Assert.Equal(cachedValue.Ddd, result.Ddd);
        //    cacheWrapperMock.Verify(x => x.GetStringAsync(key, default), Times.Once);
        //    cacheWrapperMock.VerifyNoOtherCalls();
        //}

        //[Fact(DisplayName = "Should Query On Database And Set Value On Cache When Cache Is Empty")]
        //public async Task ShouldSearchOnDatabaseAndSetValueOnCacheWhenCacheIsEmpty()
        //{
        //    var key = "test-key";
        //    var expectedObject = new RegionEntity("Test", "11");

        //    var cacheWrapperMock = new Mock<ICacheWrapper>();
        //    cacheWrapperMock.Setup(x => x.GetStringAsync(key, default)).ReturnsAsync(string.Empty);

        //    var cacheRepository = new CacheRepository(cacheWrapperMock.Object);

        //    Func<Task<RegionEntity>> producer = async () =>
        //    {
        //        await Task.Delay(10);
        //        return expectedObject;
        //    };

        //    var result = await cacheRepository.GetAsync(key, producer);

        //    Assert.NotNull(result);
        //    Assert.Equal(expectedObject.Name, result.Name);
        //    Assert.Equal(expectedObject.Ddd, result.Ddd);
        //    cacheWrapperMock.Verify(x => x.GetStringAsync(It.IsAny<string>(), default), Times.Once);
        //    cacheWrapperMock.Verify(x => x.SetStringAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DistributedCacheEntryOptions>(), default), Times.Once);
        //}

    }
}
