using Xunit;
using Moq;
using ErronkaApi.Repositorioak;
using ErronkaApi.Modeloak;
using NHibernate;

namespace ErronkaApi.Testak
{
    public class MahaiaRepoTest
    {
        [Fact]
        public async void get_lortuMahaiaLibre_mahaia_libre_mahaiak_itzuli_200()
        {
            // var sessionMock = new Mock<ISession>();
            // var sessionMockFactory = new Mock<ISessionFactory>();

            // var mahaiak = new List<Mahaia>
            // {
            //     new Mahaia { id = 1, zenbakia = 5, egoera = "libre" }
            // }.AsQueryable();

            // sessionMock
            //     .Setup(s => s.Query<Mahaia>())
            //     .Returns(mahaiak);

            // MahaiaRepository repo = new MahaiaRepository(sessionMockFactory.Object);

            // var result = await repo.LortuMahaiLibreAsync();

            // Assert.Equal(200, result.Code);
            // Assert.NotNull(result.Datuak);
            // Assert.Single(result.Datuak);
        }
    }
}